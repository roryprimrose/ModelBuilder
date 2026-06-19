# ModelBuilder
A library for easy generation of model classes

[![GitHub license](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/roryprimrose/ModelBuilder/blob/master/LICENSE)&nbsp;[![Nuget](https://img.shields.io/nuget/v/ModelBuilder.svg)&nbsp;![Nuget](https://img.shields.io/nuget/dt/ModelBuilder.svg)](https://www.nuget.org/packages/ModelBuilder)

[![Actions Status](https://github.com/roryprimrose/ModelBuilder/workflows/CI/badge.svg)](https://github.com/roryprimrose/ModelBuilder/actions)&nbsp;[![Coverage Status](https://coveralls.io/repos/github/roryprimrose/ModelBuilder/badge.svg?branch=master)](https://coveralls.io/github/roryprimrose/ModelBuilder?branch=master)

ModelBuilder fills your classes, structs and primitives with realistic pseudo-random data so your
tests can focus on behaviour instead of hand-built fixtures. A single call builds a whole object
graph — nested types, constructor arguments, collections, enums and nullable values included.

```csharp
var person = Model.Create<Person>();
```

The current version is a **source-generated, reflection-free rewrite**. Every type you build is
discovered at compile time and gets a dedicated builder, so ModelBuilder:

- runs under **Native AOT** and full trimming with **no IL warnings**,
- reports an **unbuildable type at build time** (a diagnostic) instead of throwing at runtime,
- is **dramatically faster with far fewer allocations** than the previous reflection engine
  (typically 2–14× faster and 6–50× fewer allocations — see
  [`ModelBuilder.BenchmarkTests/BASELINE.md`](ModelBuilder.BenchmarkTests/BASELINE.md)).

> [!IMPORTANT]
> This is a new major version with a deliberately smaller surface than the v8 reflection engine.
> If you are coming from v8, read the [migration guide](MIGRATION.md). Consumers that need runtime
> reflection, `dynamic`/runtime-only types, or the old extensibility pipeline should stay on the v8
> line.

- [Creating a model](#creating-a-model)
  * [Constructor arguments and parameter matching](#constructor-arguments-and-parameter-matching)
  * [Typed construction](#typed-construction)
  * [Ignoring members](#ignoring-members)
  * [Mapping abstract and interface types](#mapping-abstract-and-interface-types)
  * [Custom value sources](#custom-value-sources)
- [How types are discovered](#how-types-are-discovered)
  * [Build-time diagnostics](#build-time-diagnostics)
  * [GenerateModelBuilder](#generatemodelbuilder)
- [Populating a model](#populating-a-model)
- [Changing the model after creation](#changing-the-model-after-creation)
- [Logging the build process](#logging-the-build-process)
- [Configuration modules](#configuration-modules)
- [Built-in data](#built-in-data)
- [Target frameworks](#target-frameworks)
- [Migrating from v8](#migrating-from-v8)
- [Supporters](#supporters)

## Creating a model

The static `Model` class is the entry point. It builds classes, structs and primitive types, and
walks the whole object graph reachable from the type you ask for.

```csharp
var person = Model.Create<Person>();
```

You can also build by runtime `Type` when the type is only known at runtime:

```csharp
object person = Model.Create(typeof(Person));
```

This path still depends on compile-time discovery. A builder only exists for a type the generator
saw at compile time, so the runtime `Type` you pass must already be buildable through one of the
[discovery triggers](#how-types-are-discovered) — a `Model.Create<T>()`/`Model.Create(typeof(T))`
call somewhere in the build, a `Mapping<,>`, or a `[GenerateModelBuilder]` annotation. Passing a
`Type` that the generator never saw throws a `ModelBuildException` at runtime, because there is no
generated builder to dispatch to:

```csharp
// Throws at runtime: the concrete type came from reflection/config/a plugin, so the generator
// never saw it and produced no builder.
Type runtimeType = Type.GetType("MyApp.PluginPayload")!;
object instance = Model.Create(runtimeType);
```

To make such a type buildable, give the generator a compile-time anchor — most simply by annotating
the type (or naming it from your test assembly):

```csharp
[assembly: GenerateModelBuilder(typeof(MyApp.PluginPayload))]
```

Value-source types such as enums and primitives can be built as roots directly:

```csharp
var status = Model.Create<OrderStatus>();
var id = Model.Create<Guid>();
```

### Constructor arguments and parameter matching

You can supply constructor arguments for the top-level type being created:

```csharp
var person = Model.Create<Person>("Fred", "Smith");
```

When a type is built through a constructor, ModelBuilder matches constructor parameters to
properties of the same name and type so a freshly generated value does not overwrite a value the
constructor already assigned. Constructor selection prefers a parameterless constructor, then the
constructor with the fewest parameters, unless you supply arguments that match a specific
constructor.

### Typed construction

`Model.Create<T>(params object?[]? args)` matches its arguments at runtime, which boxes value types
and only fails when the build runs. When you want a specific constructor with **typed** arguments,
use `Model.Construct<T>().From(...)`:

```csharp
var person = Model.Construct<Person>().From("Fred", "Smith");
```

The generator emits a `From` overload for every public constructor of `T`, so the editor offers
exactly that type's constructors — with the real parameter names and the constructor's own
documentation copied across. Because the arguments are strongly typed:

- value-type arguments are **not boxed** and there is no `object?[]` array,
- a wrong argument type or count is a **compile-time error**, not a runtime exception,
- members assigned by the chosen constructor are **not** re-populated with random values (they keep
  the values you passed, and sibling-derived members such as `Email` stay consistent with them).

Every other member is still populated as usual, and the whole configuration chain composes with it:

```csharp
var order = Model.Ignoring<Order>(x => x.InternalId)
    .Construct<Order>()
    .From(customerId);
```

The `From` overloads are generated into the namespace of the call site, so they are available with
no extra `using`. They appear in the editor as soon as `Model.Construct<T>()` is written.

### Ignoring members

You can skip setting a member when building a type:

```csharp
var person = Model.Ignoring<Person>(x => x.FirstName).Create<Person>();
```

This also applies to members of types nested deeper in the graph:

```csharp
var person = Model.Ignoring<Address>(x => x.AddressLine1).Create<Person>();
```

### Mapping abstract and interface types

Abstract and interface members need a concrete type to build. Declare a mapping and that concrete
type is used wherever the source type appears:

```csharp
var payload = Model.Mapping<IShipment, GroundShipment>().Create<Parcel>();
```

There is no automatic assembly scan that guesses a concrete type — the mapping is explicit, which is
what keeps the build deterministic and reflection-free.

### Custom value sources

When the built-in data does not produce what your model needs, register a custom value source. A
value source is an `IValueSource<T>` that produces a `T` for a member or root; the simplest way to
supply one is `DelegateValueSource<T>` wrapping a lambda. A registered source takes precedence over
the built-in sources.

Register a source **by type** to control every value of that type:

```csharp
var order = Model.AddValueSource(new DelegateValueSource<OrderStatus>(c => OrderStatus.Pending))
    .Create<Order>();
```

Register a source **by member name** to control only members whose name matches (matched as a whole
PascalCase/camelCase word, the same way the built-in entity data is matched). A named source takes
precedence over a typed source for a matching member:

```csharp
var account = Model.AddValueSource(
        new DelegateValueSource<string>(c => "acct-" + c.Random.NextInt32(1000, 9999)),
        "AccountNumber")
    .Create<Account>();
```

The factory receives the `IBuildContext`, so a custom source can use the same seams the built-in
sources use — the random source, `GetSibling<T>` to stay consistent with other members already set on
the instance, and `GetOrAddScopedValue<T>` to share one cached data item across several related
members (the mechanism the built-in location and date-of-birth sources use):

```csharp
public sealed class AddressModule : IConfigurationModule
{
    private const string AddressKey = "MyApp.Address";

    public void Configure(IBuildConfiguration configuration)
    {
        // Both members draw from one cached address row, so they stay internally consistent.
        configuration.AddValueSource(
            new DelegateValueSource<string>(c => RowFor(c).Street),
            "Street", "AddressLine1");
        configuration.AddValueSource(
            new DelegateValueSource<string>(c => RowFor(c).City),
            "City");
    }

    private static AddressRow RowFor(IBuildContext context)
    {
        return context.GetOrAddScopedValue(AddressKey, _ => AddressRow.Random());
    }
}
```

Custom value sources can be registered through a configuration module's `IBuildConfiguration` exactly
as above, so they can be packaged and shared like mappings and ignore rules.

## How types are discovered

Every buildable type is found **at compile time** by a source generator. A type gets a generated
builder when it is reachable from one of these triggers:

1. **`Model.Create<T>()`, `Model.Populate<T>()` or `Model.Create(typeof(T))`** — `T` and everything
   reachable from it (constructor parameters and public settable properties) become buildable. This
   covers almost everything with no annotations.
2. **`Model.Mapping<TSource, TTarget>()`** — makes `TTarget` buildable for abstract/interface
   members.
3. **`[GenerateModelBuilder]`** — names a root that is only ever built polymorphically or via a
   runtime `Type`.

Only compile-time-discoverable types are buildable. `private`/`protected` types, constructors and
members cannot be built; `internal` types are buildable only when exposed to the building assembly
with `[InternalsVisibleTo]`.

### Build-time diagnostics

If a type you try to build is not reachable from one of the triggers above, you get a **compiler
diagnostic**, not a runtime exception:

| Diagnostic | Meaning |
| --- | --- |
| `MB1001` | The requested root type cannot have a builder generated (for example it is abstract, an interface, generic or has no accessible constructor). |
| `MB1002` | The root type is inaccessible (for example `private`) to the generated code. |
| `MB1005` | A `Model.Create(typeof(X))` root could not be resolved to a buildable type. |

### GenerateModelBuilder

Use `[GenerateModelBuilder]` to opt in a root that is never named directly in a `Model.Create<T>()`
call — for example a type you only ever build through a base type or a runtime `Type`.

```csharp
[GenerateModelBuilder]
internal sealed class AuditRecord
{
    public Guid Id { get; set; }

    public DateTimeOffset At { get; set; }
}
```

To opt a production type into generation without shipping the attribute in the production binary, use
the assembly-level form from your **test** assembly:

```csharp
[assembly: GenerateModelBuilder(typeof(MyApp.AuditRecord))]
```

## Populating a model

When you already have an instance, ModelBuilder can fill it in place:

```csharp
var person = new Person
{
    FirstName = "Jane"
};

// Fill every other member but keep the FirstName already set.
Model.Ignoring<Person>(x => x.FirstName).Populate(person);

var customer = Model.Populate(new Person());
```

## Changing the model after creation

`Set` tweaks an instance after it is built, and `SetEach` does the same across a sequence. Both are
plain extension methods, so they work on any object regardless of how it was created.

```csharp
var person = Model.Create<Person>()
    .Set(x => x.FirstName = "Joe")
    .Set(x => x.Email = null);

var other = Model.Create<Person>().Set(x =>
{
    x.FirstName = "Joe";
    x.Email = null;
});

var organisation = Model.Create<Organisation>();

organisation.Staff.SetEach(x => x.Email = null);
```

## Logging the build process

ModelBuilder can render a structured log of how a value was built. `WriteLog` enables logging and
sends the rendered log to your sink after the build completes. With xUnit you can route it to the
test output:

```csharp
public class WriteLogTests
{
    private readonly ITestOutputHelper _output;

    public WriteLogTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void WriteLogRendersBuildLog()
    {
        var actual = Model.WriteLog(_output.WriteLine).Create<Person>();

        actual.Should().NotBeNull();
    }
}
```

The log shows the build tree — each type entered, each member created, and any member skipped by an
ignore rule, a circular-reference guard or the depth guard.

## Configuration modules

An `IConfigurationModule` packages reusable build configuration so it can be shared across tests. A
module receives a `BuildConfiguration` and adds type mappings and ignore rules; the built-in value
sources are always applied, so a module only declares the deltas it needs.

```csharp
public sealed class TestModule : IConfigurationModule
{
    public void Configure(BuildConfiguration configuration)
    {
        // Use a concrete type wherever IShipment is needed.
        configuration.AddMapping<IShipment, GroundShipment>();

        // Never populate Request.Data.
        configuration.Ignore(typeof(Request), nameof(Request.Data));

        // Ignore any member matching a predicate, across all types.
        configuration.IgnoreAny(member => member.Name.EndsWith("Internal", StringComparison.Ordinal));
    }
}
```

Apply a module per build, and chain modules to layer configuration:

```csharp
var model = Model.UsingModule<TestModule>().Create<Account>();

var layered = Model.UsingModule<UnitTestModule>()
    .UsingModule<IntegrationTestModule>()
    .Create<Account>();
```

## Built-in data

ModelBuilder ships value sources for the common primitive and BCL types (`bool`, the numeric types,
`char`, `string`, `Guid`, `DateTime`, `DateTimeOffset`, `TimeSpan`, `Uri`, `Version`, `byte[]`),
plus enum, nullable and collection support that the generator wires up automatically.

### Entity-style data matched by member name

Members whose name matches a known entity field are filled with embedded reference data rather than
random noise, so the values read naturally. Matching is on a whole PascalCase/camelCase word and is
case-insensitive, so `ContactEmailAddress` matches `Email`, while a short field name like `PageCount`
is *not* treated as an `Age`.

| Member name (with aliases) | Generated value |
| --- | --- |
| `FirstName`, `GivenName` | A given name |
| `LastName`, `Surname`, `FamilyName` | A family name |
| `MiddleName` | A given name |
| `FullName`, `Name`, `DisplayName` | `First Last` |
| `UserName`, `Username`, `Login` | `first.last` with a numeric suffix, lower-cased |
| `Email` | `first.last@domain` |
| `Domain` | A domain |
| `Company`, `Business` | A company name |
| `Country` | A country |
| `State`, `Province` | A state or province |
| `City`, `Suburb`, `Town` | A city |
| `PostCode`, `ZipCode`, `Postcode`, `Zip` | A post/zip code |
| `Phone`, `Mobile`, `Cell`, `Fax` | A phone number |
| `TimeZone` | A time-zone identifier |
| `Age` | An integer in the human-plausible range 1–100 |
| `DateOfBirth`, `DOB`, `BirthDate`, `Birthday` | A UTC date for a 1–100 year old |

A member named like one of these but typed differently from what the source produces (for example an
`Age` that is a `string` rather than an `int`) falls through to the ordinary type-based sources, so
the name match never produces a type mismatch.

### Cross-field consistency

Related members on the same instance agree with each other instead of being sampled independently,
and the agreement is order-independent — it does not matter which related member the generator builds
first:

- **Names flow into email, username and full name.** `Email` is built from the `FirstName`,
  `LastName` and `Domain` already set on the object (falling back to fresh data when they are
  absent), and `FullName`/`UserName` are composed from the same name fields. A model that spells the
  members `GivenName`/`Surname` still gets a matching email.
- **One coherent location per instance.** `Country`, `State`, `City`, `PostCode` and `Phone` are
  drawn from a single address row, so an instance never produces an impossible combination such as
  *London, New South Wales, India*. A different instance elsewhere in the graph gets its own
  location.
- **Age follows date of birth.** `DateOfBirth` is the definitive value; `Age` is the number of
  completed years between it and a fixed reference date, so a generated `Age` of 18 can never sit
  next to a `DateOfBirth` that implies 35.

## Target frameworks

The library targets `netstandard2.0`, `net8.0`, `net9.0` and `net10.0`. The source generator and the
generated code use no reflection, so a consuming application can publish with Native AOT and full
trimming with no IL2xxx/IL3xxx warnings.

## Migrating from v8

The previous reflection-based extensibility pipeline (`IExecuteStrategy`, `IConstructorResolver`,
`ITypeCreator`, `IValueGenerator`, `IPostBuildAction`, the per-`ParameterInfo`/`PropertyInfo`
overloads and so on) has been removed in favour of the source-generated model and a small set of
supported seams (`IValueSource<T>`, mappings, ignore rules and `[GenerateModelBuilder]`). The
[migration guide](MIGRATION.md) maps every changed and removed v8 API to its vNext equivalent.

## Supporters

This project is supported by [JetBrains](https://www.jetbrains.com/?from=ModelBuilder)

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
    + [Writing a reusable value source](#writing-a-reusable-value-source)
    + [The build context seams](#the-build-context-seams)
- [How types are discovered](#how-types-are-discovered)
  * [Build-time diagnostics](#build-time-diagnostics)
  * [GenerateModelBuilder](#generatemodelbuilder)
- [Populating a model](#populating-a-model)
- [Changing the model after creation](#changing-the-model-after-creation)
- [Logging the build process](#logging-the-build-process)
  * [Inspecting the structured log](#inspecting-the-structured-log)
- [Handling build failures](#handling-build-failures)
- [Configuration modules](#configuration-modules)
  * [Default configuration](#default-configuration)
- [Tuning the build](#tuning-the-build)
- [Built-in data](#built-in-data)
  * [Entity-style data matched by member name](#entity-style-data-matched-by-member-name)
  * [Cross-field consistency](#cross-field-consistency)
  * [Reusable reference data sets](#reusable-reference-data-sets)
- [Generating random values](#generating-random-values)
- [Target frameworks](#target-frameworks)
- [Migrating from v8](#migrating-from-v8)
- [Supporters](#supporters)

## Creating a model

The static `Model` class is the entry point. It builds classes, structs and primitive types, and
walks the whole object graph reachable from the type you ask for.

```csharp
var person = Model.Create<Person>();
```

Value-source types such as enums and primitives can be built as roots directly:

```csharp
var status = Model.Create<OrderStatus>();
var id = Model.Create<Guid>();
```

You can also build by runtime `Type` when the type is only known at runtime:

```csharp
object person = Model.Create(typeof(Person));
```

This path still depends on compile-time discovery. A builder only exists for a type the generator
saw at compile time, so the runtime `Type` you pass must already be buildable through one of the
[discovery triggers](#how-types-are-discovered) — a `Model.Create<T>()`, `Construct<T>()`,
`Populate<T>()` or `Model.Create(typeof(T))` call somewhere in the build, a `Mapping<,>`, or a
`[GenerateModelBuilder]` annotation. Passing a `Type` that the generator never saw throws a
`ModelBuildException` at runtime, because there is no generated builder to dispatch to:

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

### Constructor arguments and parameter matching

`Model.Create<T>()` chooses a constructor and fills its arguments with generated values
automatically — you do not pass constructor arguments to `Create`. Constructor selection prefers a
parameterless constructor, then the constructor with the fewest parameters.

When a type is built through a constructor, ModelBuilder matches constructor parameters to
properties of the same name (case-insensitive) and type **at compile time**, and the matched members
are simply left out of the generated population code. A value the constructor assigned is therefore
never overwritten — there is no runtime value comparison.

Constructor arguments also become **sibling values** for the rest of the build, keyed by the
parameter name, so members derived from siblings stay consistent with them. This holds even when a
parameter is **never exposed as a property** — a type with `Contact(string firstName, string
lastName)` and only an `Email` property still gets an `Email` built from those constructor arguments,
because `firstName`/`lastName` are recorded as siblings (matched case-insensitively against the
`FirstName`/`LastName` the email source looks for).

To supply specific constructor arguments yourself, use [typed construction](#typed-construction).

### Typed construction

`Model.Create<T>()` always generates the constructor arguments for you. When you want to pass
specific constructor arguments, use `Model.Construct<T>().From(...)`:

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

The `From` overloads are generated as extension methods in the `ModelBuilder` namespace. Because you
already have `using ModelBuilder;` in scope wherever you write `Model.Construct<T>()`, they are
available with no extra `using`, and they appear in the editor as soon as `Model.Construct<T>()` is
written.

### Ignoring members

Ignoring a member skips it during population, leaving it at whatever value the constructor or the
type's own initializer set. There are three ways to declare an ignore rule, depending on whether you
target one member, one member on one type, or any member matching a condition.

`Model.Ignoring<T>(expression)` is the fluent per-build form. It selects a member with a strongly
typed expression and returns a configuration you continue building from:

```csharp
var person = Model.Ignoring<Person>(x => x.FirstName).Create<Person>();
```

The examples here finish with `Create<T>()`, but every fluent entry point on `Model` returns the same
configuration, and you can complete it with whichever build you need — `Create<T>()` for a new
instance, `Populate<T>(instance)` to fill one you already have, or `Construct<T>().From(...)` to supply
constructor arguments. The configuration also keeps composing, so you can chain further `Ignoring`,
`Mapping`, `AddValueSource`, `UsingModule` or `WriteLog` calls before the terminal build.

The rule applies wherever that member appears in the graph, so it also covers types nested deeper
than the root:

```csharp
var person = Model.Ignoring<Address>(x => x.AddressLine1).Create<Person>();
```

The remaining two forms are also fluent on `Model` (and on the configuration a [module](#configuration-modules)
receives), so they compose in the same chain as `Ignoring<T>`.

`Ignoring(Type declaringType, string memberName)` targets a single member on a specific type by name —
the non-generic equivalent of `Ignoring<T>`, useful when you only have the `Type` or want to name the
member as a string:

```csharp
var request = Model.Ignoring(typeof(Request), nameof(Request.Data)).Create<Request>();
```

`IgnoringAny(Func<MemberSignature, bool> predicate)` ignores every member, on any type, for which the
predicate returns `true`. The predicate receives a `MemberSignature` exposing the member's
`DeclaringType`, `Name` and `MemberType`, so you can match by naming convention, declaring type or
member type across the whole graph:

```csharp
// Ignore every member whose name ends in "Internal", regardless of which type declares it.
var account = Model.IgnoringAny(member => member.Name.EndsWith("Internal", StringComparison.Ordinal))
    .Create<Account>();

// Ignore every Stream-typed member anywhere in the graph.
var report = Model.IgnoringAny(member => typeof(Stream).IsAssignableFrom(member.MemberType))
    .Create<Report>();
```

Inside a [configuration module](#configuration-modules) the same two rules are declared on the
`IBuildConfiguration` as `Ignore(Type, memberName)` and `IgnoreAny(predicate)` (imperative names,
identical behaviour), so they can be packaged and reused.

All three forms compose — a build sees the union of every ignore rule from the fluent chain and from
each applied module.

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
value source is an `IValueSource<T>` that produces a `T` for a build target (a constructor parameter
or settable member) or a root; the simplest way to supply one is `DelegateValueSource<T>` wrapping a
lambda. A registered source takes precedence over the built-in sources.

Register a source **for every value of a type** to control all build targets of that type. The
following code generates every `OrderStatus` value in the graph as `OrderStatus.Pending`:

```csharp
var order = Model.AddValueSource(new DelegateValueSource<OrderStatus>(c => OrderStatus.Pending))
    .Create<Order>();
```

Register a source **scoped to specific member names** to narrow it to the constructor parameters or
settable members whose name matches one of the supplied member names (matched as a whole
PascalCase/camelCase word, the same way the built-in entity data is matched). The source still only
applies to build targets of its `T`, so it matches on both the member name and the type. A
member-name-scoped source takes precedence over a type-only source for a matching build target. The
following code uses this expression for every `string` parameter or property named `AccountNumber`:

```csharp
var account = Model.AddValueSource(
        new DelegateValueSource<string>(c => "acct-" + c.Random.NextInt32(1000, 9999)),
        "AccountNumber")
    .Create<Account>();
```

The factory receives the `IBuildContext`, so a custom source can use the same seams the built-in
sources use — the random source, `GetSibling<T>` to stay consistent with other members already set on
the instance, and `GetOrAddScopedValue<T>` to share one cached data item across several related
members (the mechanism the built-in location and date-of-birth sources use). In the following module
`AddressRow` is your own type and data set (not part of ModelBuilder); the module draws a
`Street`/`AddressLine1` and `City` from a single cached `AddressRow` so they stay consistent on each
instance:

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

    // AddressRow is your own type holding a related set of address fields. ModelBuilder does not
    // supply it; this example assumes an AddressRow.Random() factory you provide.
    private static AddressRow RowFor(IBuildContext context)
    {
        return context.GetOrAddScopedValue(AddressKey, _ => AddressRow.Random());
    }
}
```

Custom value sources can be registered through a configuration module's `IBuildConfiguration` exactly
as above, so they can be packaged and shared like mappings and ignore rules.

#### Writing a reusable value source

`DelegateValueSource<T>` is the quickest way to supply a source, and it has two factory shapes. The
first ignores where the value is going; the second also receives a `BuildTarget` describing the type
and (optional) member name being built, so one source can vary its output by member:

```csharp
// Same value everywhere.
var pending = new DelegateValueSource<OrderStatus>(context => OrderStatus.Pending);

// Vary the value by the member being built.
var labelled = new DelegateValueSource<string>((context, target) =>
    target.MemberName is null ? "value" : target.MemberName + "-" + context.Random.NextInt32(1, 999));
```

A lambda is ideal for a one-off source defined right where it is registered. When the source is worth
keeping as a reusable, testable unit, however, implement `IValueSource<T>` as a dedicated class.
Reach for a class when the source holds its own state or dependencies, when several tests or
[configuration modules](#configuration-modules) need to share the exact same logic, or simply when a
descriptive class name documents intent better than an inline lambda. A class also gives the source a
single place to unit-test.

The source's `Create` method receives the same `IBuildContext` and `BuildTarget` that
`DelegateValueSource<T>` passes to its factory:

```csharp
public sealed class SkuValueSource : IValueSource<string>
{
    public string Create(IBuildContext context, in BuildTarget target)
    {
        return "SKU-" + context.Random.NextInt32(10000, 99999);
    }
}

// "Sku" is the member name to match (as in the AccountNumber example above); it is unrelated to the
// SkuValueSource class name. Drop it to apply the source to every string build target instead.
var product = Model.AddValueSource(new SkuValueSource(), "Sku").Create<Product>();
```

`NullableValueSource<T>` wraps a value-type source so it produces `T?`. It returns `null` for a share
of values set by `BuildOptions.NullPercentage` (a 5% default; see [Tuning the build](#tuning-the-build))
and otherwise delegates to the underlying source — the mechanism that makes generated nullable members
occasionally `null`.

#### The build context seams

The `IBuildContext` passed to every value source exposes the same seams the engine and built-in
sources use, so a custom source can participate fully in a build:

| Member | Purpose |
| --- | --- |
| `Random` | The build's `IRandomSource` — seedable, thread-safe, typed (see [Generating random values](#generating-random-values)). |
| `GetSibling<T>(name)` / `GetSibling<T>(names...)` | Read a value already set on the instance being built (the first match across the candidate names), so derived members stay consistent. Returns `default` when there is no match. |
| `GetOrAddScopedValue<T>(key, factory)` | Get-or-create a value cached for the lifetime of the current instance, so several sources can share one data item (such as an address row). A different instance elsewhere in the graph gets its own scope. |
| `NextCount()` | A random collection length within the configured min/max bounds (a 1–10 default; tune with [SetOptions](#tuning-the-build)). |
| `Build<T>(declaringType, name)` | Recursively build a nested member value through the normal source/builder resolution, with circular-reference and depth guards applied. |
| `ShouldPopulate(declaringType, name, memberType)` | Whether a member passes the configured ignore rules. |
| `RecordSibling(name, value)` / `EnterSiblingScope()` | Record a value into, or open, the sibling scope that `GetSibling<T>` reads. |
| `Configuration` / `Log` | The active `IBuildConfiguration` and `IBuildLog` for the build. |

All numeric range methods on `Random` are inclusive of both bounds, and `GetSibling<T>` name matching
is the same whole-word, case-insensitive scheme used by the built-in entity data.

## How types are discovered

Every buildable type is found **at compile time** by a source generator. A type gets a generated
builder when it is reachable from one of these triggers:

1. **`Model.Create<T>()`, `Model.Populate<T>()`, `Model.Construct<T>()` or `Model.Create(typeof(T))`** —
   `T` and everything reachable from it (constructor parameters and public settable properties) become
   buildable. This covers almost everything with no annotations.
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

`SetEach` has overloads for the common collection and dictionary shapes (`IEnumerable<T>`, `IList<T>`,
`ICollection<T>`, `IReadOnlyList<T>`, `List<T>`, `Collection<T>`, `ReadOnlyCollection<T>`, the
`IDictionary`/`IReadOnlyDictionary`/`Dictionary`/`ReadOnlyDictionary` families) and returns the same
collection type it received, so it stays chainable. A second overload passes the **index** alongside
each item:

```csharp
var staff = Model.Create<Organisation>().Staff;

// The lambda receives the zero-based index and the item.
staff.SetEach((index, person) => person.EmployeeNumber = index + 1);
```

The `SetEach` overloads above cover the common collection and dictionary types by name. If your
variable's type is some other collection — a `Queue<T>`, a `HashSet<T>`, or a custom collection — none
of those overloads match it, so the compiler cannot pick one. `SetEachExplicit` is the fallback for
that case: it works on any `IEnumerable<TEntry>` (or `IEnumerable<KeyValuePair<TKey, TValue>>` for
dictionaries), runs the action over every item, and returns the collection unchanged. Because the
compiler cannot infer the type arguments from the receiver alone, you supply them explicitly — the
collection type first, then the item type — which is what gives the method its name. Index-aware
overloads are available too, matching `SetEach`:

```csharp
var queue = new Queue<Person>(Model.Create<List<Person>>());

// <Queue<Person>, Person> = <the collection type, the item type>.
queue.SetEachExplicit<Queue<Person>, Person>((index, person) => person.Email = null);
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

### Inspecting the structured log

`WriteLog(Action<string>)` renders the log to text, but the log is also a structured tree you can
inspect programmatically. An `IBuildLog` exposes `Entries`, a list of `BuildLogEntry` nodes that each
carry a `Kind` (`CreateInstance`, `PopulateInstance`, `CreateValue` or `SkipMember`), the `TargetType`,
an optional `MemberName` and `Reason`, and nested `Children`. This is the same tree captured on a
[`ModelBuildException`](#handling-build-failures), so you can assert on exactly what the builder did —
for example that a member was skipped for the expected reason — rather than parsing rendered text:

```csharp
static bool WasSkipped(IReadOnlyList<BuildLogEntry> entries, string memberName) =>
    entries.Any(e =>
        (e.Kind == BuildLogEntryKind.SkipMember && e.MemberName == memberName)
        || WasSkipped(e.Children, memberName));
```

## Handling build failures

When a build cannot complete, ModelBuilder throws a `ModelBuildException`. As well as the message, it
carries structured context so you can diagnose the failure without a debugger:

| Member | Description |
| --- | --- |
| `FailureKind` | A `FailureKind` enum categorising the cause: `InaccessibleConstructor`, `UnmappedAbstractType`, `NoValueSource`, `NoBuilderForType`, `CircularReference`, `DepthExceeded`, `ValueSourceThrew` or `Unspecified`. |
| `TargetType` | The type being built when the failure occurred, or `null`. |
| `TargetMember` | The member being built when the failure occurred, or `null`. |
| `BuildPath` | The `BuildFrame` chain from the root type down to the failing member. |
| `BuildLog` | The structured [build log](#inspecting-the-structured-log) captured up to the failure. |

Branch on `FailureKind` rather than the message so your handling survives wording changes:

```csharp
try
{
    var parcel = Model.Create<Parcel>();
}
catch (ModelBuildException ex) when (ex.FailureKind == FailureKind.UnmappedAbstractType)
{
    // A reachable abstract/interface member has no mapping — add a Model.Mapping<,> for it.
    Console.WriteLine($"Map {ex.TargetType} before building (at {ex.TargetMember}).");
}
```

## Configuration modules

An `IConfigurationModule` packages reusable build configuration so it can be shared across tests. A
module receives an `IBuildConfiguration` and adds type mappings, ignore rules and custom value
sources; the built-in value sources are always applied, so a module only declares the deltas it needs.

```csharp
public sealed class TestModule : IConfigurationModule
{
    public void Configure(IBuildConfiguration configuration)
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

### Default configuration

A build with no configuration — `Model.Create<T>()` with no module, ignore rule, mapping or option
override — starts from these defaults. A module, or a fluent call on `Model`, only adds deltas on top:

| Category | Default |
| --- | --- |
| Type mappings | None. Abstract and interface members are unbuildable until you add a [`Mapping<,>`](#mapping-abstract-and-interface-types). |
| Ignore rules | None. Every settable member and constructor parameter is populated. |
| Custom value sources | None registered, but the **built-in value sources always apply** (you never register them, and a custom source only overrides the built-in for its type/name). |
| Built-in typed sources | `bool`; every numeric type (`byte`/`sbyte`/`short`/`ushort`/`int`/`uint`/`long`/`ulong`/`float`/`double`/`decimal`); `char`; `string`; `Guid`; `DateTime`; `DateTimeOffset`; `TimeSpan`; `Uri`; `Version`; `byte[]`. Enums, nullables and collections are handled by the generator. |
| Built-in named sources | The entity-style member-name sources in [Built-in data](#entity-style-data-matched-by-member-name) (names, email, company, location fields, age, date of birth, …). |
| Build options | `MinCount` 1, `MaxCount` 10, `NullPercentage` 5, `MaxDepth` 50 — see [Tuning the build](#tuning-the-build). |

## Tuning the build

A few build-wide settings live on `BuildOptions`: the collection-size range (`MinCount`/`MaxCount`),
the chance a nullable value is produced as `null` (`NullPercentage`, 0–100), and the maximum graph
depth (`MaxDepth`). Tune them with `Model.SetOptions`, which returns the same fluent configuration as
the other entry points:

```csharp
// Always populate nullables, and make collections hold exactly three items.
var order = Model.SetOptions(x =>
    {
        x.NullPercentage = 0;
        x.MinCount = 3;
        x.MaxCount = 3;
    })
    .Create<Order>();
```

The same call is available on the `IBuildConfiguration` a [configuration module](#configuration-modules)
receives, so a module can set defaults that every build using it inherits:

```csharp
public sealed class CompactModule : IConfigurationModule
{
    public void Configure(IBuildConfiguration configuration)
    {
        configuration.SetOptions(x => x.MaxCount = 3);
    }
}
```

`MinCount`/`MaxCount` are coerced when a collection length is drawn — a negative minimum becomes zero
and a maximum below the minimum is raised to it — so setting `MaxCount` without `MinCount` cannot
throw. The defaults are `MinCount` 1, `MaxCount` 10, `NullPercentage` 5 and `MaxDepth` 50.

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
  drawn from a single built-in [`Location`](#reusable-reference-data-sets) row, so an instance never
  produces an impossible combination such as *London, New South Wales, India*. A different instance
  elsewhere in the graph gets its own location.
- **Age follows date of birth.** `DateOfBirth` is the definitive value; `Age` is the number of
  completed years between it and a fixed reference date, so a generated `Age` of 18 can never sit
  next to a `DateOfBirth` that implies 35.

### Reusable reference data sets

The same embedded data the built-in sources draw from is exposed through the static `ModelBuilder.Data.TestData`
class, so you can use it directly in your own value sources, fixtures or assertions. Each property is a
cached `IReadOnlyList<>` read once from embedded resources:

| `TestData` member | Contents |
| --- | --- |
| `MaleNames`, `FemaleNames` | Given names |
| `LastNames` | Family names |
| `Companies` | Company names |
| `Domains` | Email/web domains |
| `TimeZones` | Time-zone identifiers |
| `Locations` | `Location` rows pairing `City`, `State`, `Country`, `PostCode`, `Phone`, `StreetName` and `StreetSuffix` so a drawn location is internally consistent |

```csharp
var random = new RandomSource();
var location = TestData.Locations[random.NextInt32(0, TestData.Locations.Count - 1)];

// location.City, location.State, location.Country, location.PostCode all belong together.
```

`Location` (in the same `ModelBuilder.Data` namespace) is a plain mutable type. You can build your own
instances with an object initializer, or parse one from a CSV line with `Location.Parse`. The CSV
columns are comma-separated in this exact order:

```text
Country,State,City,PostCode,StreetName,StreetSuffix,Phone
```

```csharp
var custom = new Location { City = "Hobart", State = "Tasmania", Country = "Australia" };
var parsed = Location.Parse("Australia,Tasmania,Hobart,7000,Murray,Street,03 6200 0000");
```

These are for your own value sources and fixtures. The built-in `TestData` lists (including
`Locations`) are read-only, so there is no public API to add your own entries to the data the built-in
location source draws from — to inject custom location data into a build, register a [custom value
source](#custom-value-sources) for the relevant members.

## Generating random values

ModelBuilder's randomness comes from `IRandomSource`, implemented by `RandomSource`. It is the
`Random` seam on `IBuildContext`, but you can also use it standalone — for direct random values or to
make a build reproducible by seeding it. It is thread-safe, returns each value type directly without
boxing, and generates wide-integer and `decimal` ranges with their own arithmetic so there is no
precision loss through `double`:

```csharp
// A default source is independently seeded; pass a seed to reproduce a sequence.
var random = new RandomSource(seed: 1234);

int dice = random.NextInt32(1, 6);          // every range method is inclusive of both bounds
decimal price = random.NextDecimal(0m, 100m);
bool flag = random.NextBool();

var buffer = new byte[16];
random.NextBytes(buffer);
```

Typed methods cover the full numeric range — `NextByte`, `NextSByte`, `NextInt16`/`NextUInt16`,
`NextInt32`/`NextUInt32`, `NextInt64`/`NextUInt64`, `NextSingle`, `NextDouble`, `NextDecimal` — plus
`NextBool` and `NextBytes`. The integer and floating-point overloads default to the type's full range,
and the `Seed` property reports the seed in use so a failing test can be replayed.

## Target frameworks

The library targets `netstandard2.0`, `net8.0`, `net9.0` and `net10.0`. The source generator and the
generated code use no reflection, so a consuming application can publish with Native AOT and full
trimming with no IL2xxx/IL3xxx warnings.

## Migrating from v8

The previous reflection-based extensibility pipeline (`IExecuteStrategy`, `IConstructorResolver`,
`ITypeCreator`, `IValueGenerator`, `IPostBuildAction`, the per-`ParameterInfo`/`PropertyInfo`
overloads and so on) has been removed in favour of the source-generated model and a small set of
supported seams (`IValueSource<T>`, mappings, ignore rules and `[GenerateModelBuilder]`). The
[migration guide](MIGRATION.md) maps every changed and removed v8 API to its v9 equivalent.

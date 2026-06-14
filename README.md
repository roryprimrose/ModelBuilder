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
  * [Ignoring members](#ignoring-members)
  * [Mapping abstract and interface types](#mapping-abstract-and-interface-types)
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

Entity-style members are filled with embedded reference data rather than random noise, so the values
read naturally — names, companies, and address parts (country, state, city, postcode, street, phone)
come from curated data sets. Some values are derived from their siblings on the same instance: an
`Email` is built from the `FirstName`, `LastName` and `Domain` already set on the object so the parts
agree.

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

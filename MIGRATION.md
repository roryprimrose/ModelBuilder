# Migrating from ModelBuilder v8 to vNext

vNext is a source-generated, reflection-free, AOT- and trim-compatible rewrite of ModelBuilder. It
keeps the everyday API but removes the deep extensibility pipeline that depended on runtime
reflection. There is **no back-compat shim** — migration is manual, but mechanical. This guide maps
every changed API to its vNext equivalent so a human (or an AI agent) can apply it deterministically.

> [!IMPORTANT]
> vNext ships as a new major version. Consumers that need the reflection-based engine, runtime-only
> (`dynamic`) types, or the removed extensibility hooks should stay on the v8 line.

## How vNext works (the one thing to understand)

vNext discovers every type you build **at compile time** and generates a dedicated builder for it.
The triggers are:

1. **`Model.Create<T>()` / `Model.Populate<T>()` / `Model.Create(typeof(T))`** — `T` and everything
   reachable from it (constructor parameters, public settable properties) get a builder. This covers
   almost everything with zero annotations.
2. **`Model.Mapping<TAbstract, TConcrete>()`** — makes `TConcrete` buildable for abstract/interface
   members.
3. **`[GenerateModelBuilder]`** — names a root that is only ever built polymorphically or via a
   runtime `Type`. Declare it in the **test assembly** (use the assembly-level
   `[assembly: GenerateModelBuilder(typeof(X))]` form to opt a production type in without shipping
   the attribute).

If a type is not reachable from one of those, there is no builder for it and you get a **build-time
diagnostic** (`MB1001`/`MB1002`/`MB1005`), not a runtime surprise.

## Unchanged — no action

These keep their shape, so the bulk of a typical test project compiles unchanged:

- `Model.Create<T>()`, `Model.Create<T>(args)`, `Model.Create(typeof(T))`
- `.Set(...)`, `.SetEach(...)`
- `Model.Ignoring<T>(x => x.Member)`
- `Model.UsingModule<T>()`
- `Model.Mapping<TSource, TTarget>()`
- `Model.WriteLog(sink)`

## Mapped — mechanical replacement

| v8 API | vNext replacement | Notes |
| --- | --- | --- |
| `IValueGenerator` / `ValueGeneratorBase` | `IValueSource<T>` | One `Create` method; no `IsMatch`/`Generate(Type,…)`. Matching moves to registration. |
| `ITypeCreator` / `TypeCreatorBase` (constructor-only type) | `IValueSource<T>` | `Create` calls the constructor and returns the instance; the engine populates it afterwards. |
| `configuration.AddValueGenerator<G>()` | `configuration.AddValueSource<T>(source, "MemberName")` (named) or `configuration.AddValueSource<T>(source)` (typed) | The `T` fixes the type; the member name is declared at registration. |
| `configuration.AddTypeCreator<C>()` | `configuration.AddValueSource<T>(source)` | Same registration shape; `Create` calls the constructor. |
| `UpdateValueGenerator<G>(g => g.X = …)` / `UpdateTypeCreator<C>(c => c.X = …)` | `BuildContextOptions` (e.g. `MinCount`/`MaxCount`/`NullPercentage`/`MaxDepth`) | Strongly-typed tuning hooks instead of reflection-tuning arbitrary generators. |
| `AddIgnoreRule<T>(x => x.Member)` | `Model.Ignoring<T>(x => x.Member)` or `configuration.IgnoreAny(member => ...)` | Targeted unchanged; type-agnostic `IgnoreAny` is new. |
| `AddCreationRule(predicate, value, priority)` | `configuration.AddValueSource<T>(source, "MemberName")` | The fast-value shortcut becomes an ordinary named source. |
| `DefaultConfigurationModule` (called explicitly) | implicit default configuration | The built-in sources register automatically; a custom module only adds deltas. |
| `RandomGenerator` (`object NextValue(Type, …)`) | `IRandomSource` (`NextInt32`/`NextInt64`/`NextDouble`/…) | Typed, thread-safe, seedable, no boxing, no precision loss. |

## Removed — no equivalent

These extended the build pipeline directly. They are gone with no replacement; move to the supported
seams (`IValueSource<T>`, mappings, ignore rules, `[GenerateModelBuilder]`):

- `IExecuteStrategy`, `IBuildProcessor`, `IBuildAction`, `IBuildCapability`
- `IConstructorResolver`, `IParameterResolver`, `IPropertyResolver`, `ITypeResolver`
- `CacheLevel`
- the `Type` / `PropertyInfo` / `ParameterInfo` generator overload triplets

The pure source-generated model makes these unnecessary: construction, member discovery and ordering
are decided at compile time, so there is no runtime pipeline to extend.

## Worked example — a custom configuration module

**v8** (calls the default module, ignores a property, registers a type creator):

```csharp
public class TestModule : IConfigurationModule
{
    public void Configure(IBuildConfiguration configuration)
    {
        new DefaultConfigurationModule().Configure(configuration);
        configuration.AddIgnoreRule<Person>(x => x.Notes);
        configuration.AddTypeCreator<PaymentTypeCreator>();
    }
}
```

**vNext** (the defaults are implicit; the module only adds deltas):

```csharp
public sealed class TestModule : IConfigurationModule
{
    public void Configure(IBuildConfiguration configuration)
    {
        configuration.Ignore(typeof(Person), nameof(Person.Notes));
        // Replace a generator/type-creator with a value source:
        configuration.AddValueSource(new DelegateValueSource<PaymentType>(c => PaymentType.Card), "PaymentType");
    }
}
```

## What vNext no longer supports

Decide up front whether vNext fits:

- **No runtime reflection / no `dynamic` or runtime-only types.** A `Type` known only at runtime
  (loaded from a plugin, computed reflectively) cannot be built — there is no reflection fallback.
- **Only compile-time-discoverable types are buildable.** If a type is not reached from a
  `Model.Create<T>()`, a mapping, or `[GenerateModelBuilder]`, no builder exists for it (diagnostic
  `MB1001`/`MB1002`/`MB1005`).
- **Visibility limits.** `private`/`protected` types, constructors and members cannot be built;
  `internal` only via `[InternalsVisibleTo]` to the test assembly.
- **Removed extensibility pipeline** (see above) — custom `IExecuteStrategy`/`IBuildProcessor`/
  `IBuildAction`/resolvers have no equivalent.
- **`DefaultTypeResolver` automatic interface/abstract scanning is removed.** Abstract and interface
  targets need an explicit `Mapping<,>`; there is no assembly scan that guesses a concrete type.
- **Per-`ParameterInfo`/`PropertyInfo` generator overloads and `CacheLevel`** no longer exist.

## What you gain

- **No reflection, AOT/trim safe** — works under Native AOT with no trim warnings.
- **Compile-time errors instead of runtime surprises** — unbuildable types are diagnosed at build.
- **Dramatically faster, far fewer allocations** — typically 2–14× faster and 6–50× fewer
  allocations than the v8 reflection engine, with no Gen1 GC pressure (see the benchmark delta in
  `ModelBuilder.BenchmarkTests/BASELINE.md`).

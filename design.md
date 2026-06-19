# ModelBuilder vNext — High-Level Design

This document describes the high-level design of the current ModelBuilder library and proposes
the design for a complete rewrite. The rewrite targets a **source-generated, reflection-free,
AOT- and trim-compatible** implementation that keeps the everyday developer experience while
removing the layers of extensibility and indirection that make the current codebase hard to
understand and impossible to run under Native AOT.

---

## 1. Goals and non-goals

### Goals

- **No runtime reflection.** All type creation, property population, constructor selection, and
  member discovery is generated at compile time. No `Activator.CreateInstance`, `MakeGenericType`,
  `PropertyInfo.SetValue`, `Type.GetConstructors`, or `dynamic`/`ExpandoObject`.
- **Native AOT and trimming safe.** The shipped assembly carries no trim warnings and works in a
  fully trimmed/AOT app.
- **Keep the common API.** `Model.Create<T>()`, `Model.Create<T>(args)`, `.Set(...)`,
  `.SetEach(...)`, `Model.Ignoring<T>(...)`, and reusable configuration modules continue to work
  with minimal source changes for existing consumers.
- **Dramatically simpler internals.** Collapse the build pipeline from
  (ExecuteStrategy → BuildProcessor → BuildActions → BuildCapability → Resolvers → Rules) into a
  small, readable core.
- **Preserve realistic data quality.** Names, addresses, companies, emails, phone numbers, GUIDs,
  URIs, IP addresses, enums, and relative values (e.g. email derived from first/last name) still
  produce believable test data.

### Non-goals

- 100% binary or API back-compat with v8. Deep extensibility consumers (custom `IExecuteStrategy`,
  `IBuildAction`, `IBuildProcessor`, per-`ParameterInfo`/`PropertyInfo` generator overloads) will
  need to migrate. This is acceptable and intended — see §3.
- **No runtime reflection fallback of any kind.** vNext is a pure source-generator implementation.
  There is no hybrid mode and no reflection escape hatch. Consumers who need the reflection-based
  engine stay on the v8 line; the rewrite ships under a new major version. (Resolves §12.1.)
- **No support for runtime-only / `dynamic` types.** If a type, constructor, or member is not
  visible to the generated code at compile time, it cannot be built. Dropping `dynamic`/runtime
  type support is an accepted trade-off for the simplification.
- Generating values for a `Type` known only at runtime (e.g. via reflection). `Model.Create(Type)`
  is supported **only** for types the generator already emitted a builder for; an unknown runtime
  `Type` produces a clear exception, not a reflection fallback (see §6.1).
- **Changes confined to the ModelBuilder repository.** Implementing this design must not modify any
  other repository. Any consuming repositories stay on the v8 ModelBuilder package and are migrated
  separately, on their own schedule, using the README migration guide (§14) — not as part of this
  rewrite. All work lands under this repository only.

---

## 2. Current design (what we are replacing)

### 2.1 Entry point

The static `Model` class is the public surface. Each call builds a fresh mutable
`BuildConfiguration` (seeded by `DefaultConfigurationModule`) and runs it through a
`DefaultExecuteStrategy<T>`.

```
Model.Create<T>(args)
  └─ UsingDefaultConfiguration()                // new BuildConfiguration + DefaultConfigurationModule
       └─ UsingExecuteStrategy<DefaultExecuteStrategy<T>>()
            └─ strategy.Create(args)
```

### 2.2 Configuration

`IBuildConfiguration` is a mutable bag of **ten** extensibility collections/resolvers:

| Member | Role |
|--------|------|
| `ConstructorResolver` | Pick a constructor (reflection over `GetConstructors`) |
| `ParameterResolver` | Order constructor parameters |
| `PropertyResolver` | Discover + order properties to populate |
| `TypeResolver` | Map interface/abstract → concrete (assembly scan) |
| `CreationRules` | Fast value shortcuts by type/name/regex/expression |
| `ExecuteOrderRules` | Priority order for property/parameter population |
| `IgnoreRules` | Skip specific members |
| `PostBuildActions` | Mutate instance after build |
| `TypeCreators` | Create class/struct/array/enumerable instances |
| `TypeMappingRules` | Map source type → target type |
| `ValueGenerators` | Produce flat/leaf values |

### 2.3 Build pipeline

`DefaultExecuteStrategy` recursively builds a graph:

1. `BuildProcessor` orders a fixed list of `IBuildAction`s by priority:
   `CircularReferenceBuildAction → CreationRuleBuildAction → ValueGeneratorBuildAction →
   TypeCreatorBuildAction`.
2. Each action returns an `IBuildCapability` describing whether it can create/populate the
   requested `Type`/`ParameterInfo`/`PropertyInfo`.
3. The strategy invokes the winning capability, then **populates** the result:
   - Constructor parameters are built first. Values are stashed in an `ExpandoObject`
     (`dynamic`) so `RelativeValueGenerator`s can read sibling values by name, then re-ordered
     back to constructor order.
   - Properties are discovered + ordered by `DefaultPropertyResolver`, filtered by ignore rules
     and constructor-parameter-match heuristics, then each is built and assigned via
     `PropertyInfo.SetValue`.
4. `BuildHistory`/`IBuildChain` track the current ancestry for circular-reference detection,
   relative value lookups, and capability caching. `IBuildLog` records a nested trace.

### 2.4 Value generators and data

~30 `IValueGenerator`s derive from `ValueGeneratorBase` → `ValueGeneratorMatcher`, matching by
target `Type` plus an optional name string/regex. Entity-style generators read embedded `.txt`
resources (`MaleNames`, `FemaleNames`, `LastNames`, `Companies`, `Locations`, `Domains`,
`TimeZones`). `RelativeValueGenerator` derives a value from another member on the same instance
(e.g. `Email` from `FirstName`+`LastName`+`Domain`, `FirstName` constrained by `Gender`).

### 2.5 Type creators

`DefaultTypeCreator` (Activator), `StructTypeCreator`, `ArrayTypeCreator`, `EnumerableTypeCreator`
(`MakeGenericType(ICollection<T>)` + `GetMethod("Add")`), plus `FactoryTypeCreator` and
`SingletonTypeCreator`. All depend on reflection.

### 2.6 Reflection / AOT blockers (concrete)

- `Activator.CreateInstance(type, args)` — `DefaultTypeCreator`, `EnumerableTypeCreator`.
- `Type.MakeGenericType(...)` + `GetMethod("Add")` — `EnumerableTypeCreator`, `ArrayTypeCreator`.
- `type.GetConstructors(...)` + parameter inspection — `DefaultConstructorResolver`.
- `type.GetProperties(...)` + `PropertyInfo.GetValue/SetValue` — `DefaultPropertyResolver`,
  `DefaultExecuteStrategy`.
- `ExpandoObject` / `dynamic` parameter wrapper — `DefaultExecuteStrategy.CreateParameters`.
- Assembly scanning for concrete types — `DefaultTypeResolver` (also non-deterministic, issue #242).
- `Convert.ChangeType` and `MakeGenericType`-driven nullable handling — `RelativeValueGenerator`.

These are the exact points the source generator must replace.

---

## 3. Why a rewrite (problems with the current design)

1. **Too much indirection.** Creating one value flows through ExecuteStrategy → BuildProcessor →
   4 BuildActions → BuildCapability → a resolver → a rule/creator/generator. Following a single
   `Model.Create<T>()` requires reading a dozen types.
2. **Over-broad extensibility, mostly unused.** Ten configurable collections, each with
   `Type` / `PropertyInfo` / `ParameterInfo` overloads. Real-world usage typically only needs:
   create by type, ignore a property, add a custom type creator, and a reusable
   module. Issue #293 ("Remove explicit parameter and property API") confirms the maintainer
   already wants to shrink this surface.
3. **Reflection everywhere → no AOT.** See §2.6. The library cannot run under Native AOT and
   produces trim warnings.
4. **`dynamic`/`ExpandoObject`** for relative values defeats trimming/AOT and is hard to follow.
5. **Reliability gaps tied to reflection-time discovery:**
   - #347 `StackOverflowException` on self-referencing property.
   - #283 `StackOverflowException` creating `Exception.Data` (`IDictionary`).
   - #282 write-only properties throw.
   - #292 `TypeMapping` not always applied.
   - #242 `DefaultTypeResolver` non-deterministic when multiple concrete types match.
   - #188 setting `MaxCount` without `MinCount` throws.
   - #349 `WriteLog` loses messages when an exception is thrown.
   - #340 / #346 constructors with default parameter values are not honored.

A compile-time model lets us address most of these structurally (known graph shape, generated
populate code, deterministic candidate selection).

---

## 4. Design principles for vNext

- **Compile-time first.** If the type is referenced in `Model.Create<T>()` (or reachable from
  one), the generator emits a dedicated, allocation-light builder for it.
- **One obvious path.** A requested type resolves to exactly one generated builder. No priority
  auction across four build actions at runtime.
- **Small, sealed core.** A handful of public types. Extensibility is opt-in and narrow (§8).
- **Deterministic by default.** Same seed ⇒ same graph. Interface/abstract resolution is an
  explicit, compile-time-validated mapping rather than a runtime assembly scan.
- **Fail at build time, not run time.** Unbuildable members (write-only, no accessible
  constructor, unmapped interface) are diagnosed by the generator with actionable messages.

---

## 5. Proposed architecture

### 5.1 Components

```
┌─────────────────────────────────────────────────────────────┐
│ ModelBuilder (runtime, netstandard2.0 + net8.0/net9.0/net10)│
│   • Model (static facade)                                   │
│   • IModelBuilder<T> / generated TBuilder : IModelBuilder<T>│
│   • IBuildConfiguration (slim) + modules                    │
│   • IRandomSource, value primitives, embedded data readers  │
│   • Set / SetEach / Ignoring extension methods              │
└─────────────────────────────────────────────────────────────┘
            ▲ references                       ▲ analyzer ref
┌─────────────────────────────────────────────────────────────┐
│ ModelBuilder.Generator (Roslyn incremental source generator)│
│   • Discovers Model.Create<T> / [GenerateBuilder] targets   │
│   • Emits one builder per closed type in the object graph   │
│   • Emits the registry that maps Type → builder             │
│   • Emits diagnostics for unbuildable members               │
└─────────────────────────────────────────────────────────────┘
```

### 5.2 What the generator emits

For each concrete type `T` reachable from a build root, emit a `partial`/internal builder:

```csharp
// GENERATED
internal sealed class PersonBuilder : IModelBuilder<Person>
{
    public Person Create(BuildContext ctx, params object?[]? args)
    {
        // Constructor selection resolved at compile time (no GetConstructors at runtime).
        var instance = args is { Length: > 0 }
            ? CreateWithArgs(ctx, args)
            : new Person();

        Populate(ctx, instance, args);
        return instance;
    }

    public Person Populate(BuildContext ctx, Person instance, object?[]? args = null)
    {
        // Emitted in execute-order; each branch checks ignore rules + ctor-param-match.
        if (ctx.ShouldPopulate(BuilderIds.Person_Gender))
            instance.Gender = ctx.CreateGender(/* relative context */);
        if (ctx.ShouldPopulate(BuilderIds.Person_FirstName))
            instance.FirstName = ctx.CreateFirstName(genderOf: instance.Gender);
        if (ctx.ShouldPopulate(BuilderIds.Person_Email))
            instance.Email = ctx.CreateEmail(instance.FirstName, instance.LastName, instance.Domain);
        // ...
    }
}
```

Key properties of generated code:

- **Direct member access.** `new Person()`, `instance.FirstName = ...` — no `SetValue`.
- **Compile-time member order.** Execute-order rules are evaluated by the generator and baked into
  the emitted statement order, so there is no runtime sort.
- **Compile-time constructor choice.** The generator picks the constructor (smallest, or one
  matching supplied arg shapes) and emits typed `CreateWithArgs` overloads — replacing
  `DefaultConstructorResolver` reflection. Default parameter values are honored in generated code
  (fixes #340/#346).
- **Compile-time collection handling.** `List<T>`, arrays, `Dictionary<K,V>`, `ICollection<T>`
  get emitted `Add`/index code — no `MakeGenericType`/`GetMethod("Add")`.
- **Relative values without `dynamic`.** Sibling values are ordinary locals/parameters passed
  between generated calls — `ExpandoObject` is gone.

### 5.3 Runtime core

- `BuildContext` carries the random source, the ignore/mapping configuration, the current build
  chain (for circular-reference detection), an optional build log, and the builder registry.
- `IModelBuilder<T>` is the single generated abstraction: `Create` and `Populate`.
- The **registry** (also generated) maps a runtime `Type` → `IModelBuilder<>` for the graph,
  enabling polymorphic/nested builds without reflection.

### 5.4 Circular references and pathological types (fixes #347, #283)

The `BuildContext` build chain tracks types/instances under construction. The generator also
knows statically when a property's type re-enters an ancestor type and can emit a guarded build
(default to `null`/skip past a configurable depth). Self-referencing graphs and recursive
dictionaries (`Exception.Data`) terminate deterministically instead of overflowing the stack.

---

## 6. Public API (target surface)

Preserve the everyday API so existing tests compile largely unchanged:

```csharp
// Create
var p = Model.Create<Person>();
var p2 = Model.Create<Person>("Fred", "Smith");
var u = Model.Create<Uri>();

// Post-build tweaks (unchanged)
var x = Model.Create<Person>().Set(p => p.Email = null);
org.Staff.SetEach(s => s.Email = null);

// Ignore
var r = Model.Ignoring<Person>(p => p.FirstName).Create<Person>();

// Reusable modules (kept, slimmed)
var a = Model.UsingModule<TestModule>().Create<Account>();

// Mapping interface/abstract → concrete (now compile-time validated)
var payload = Model.Mapping<Stream, MemoryStream>().Create<RequestPayload>();
```

Common usage that **must** keep working:
`Model.Create<T>()`, `Model.Create(Type)`, ctor-arg create, `.Set`, `.SetEach`, `Ignoring`,
custom `IConfigurationModule` (calling the default module then adding ignore rules / a custom
creator), a custom type-creator for a hard-to-build type (one whose constructor takes a parameter
the builder can't fabricate), and `UpdateValueGenerator<T>` / `UpdateTypeCreator<T>` tuning (e.g.
`AgeValueGenerator.MinAge`, `EnumerableTypeCreator.MinCount/MaxCount`).

### 6.1 Build-graph discovery (how the generator finds what to emit)

`Model.Create<T>()` is the **root of the type tree**. The generator walks outward from every
build root and emits one builder per concrete type reachable through constructor parameters and
populated members. There are exactly two ways to introduce a root:

1. **Call-site discovery (primary).** Every `Model.Create<T>()` / `Model.Populate<T>()` /
   `...Create<T>(args)` call in the compilation contributes `T` as a root. `T` is a closed,
   compile-time type argument, so the generator knows it precisely and recurses from there. This
   covers the overwhelming majority of usage and needs no annotations.

2. **`[GenerateModelBuilder]` attribute (explicit roots).** Some roots are never named directly in
   a `Create<T>()` — for example a type only ever built polymorphically through a base type or
   interface, or a type built reflectively in a helper that the generator can't see through. The
   attribute names those roots so a builder is still emitted:

   ```csharp
   // Emit a builder for Manifest even though callers only ask for IManifest.
   [assembly: GenerateModelBuilder(typeof(Manifest))]

   // Or annotate the type itself.
   [GenerateModelBuilder]
   public sealed class Manifest : IManifest { /* ... */ }
   ```

   The assembly-level form takes a `typeof(...)` (works for types you don't own); the type-level
   form opts a type in wherever it is declared. A mapping (`Mapping<IManifest, Manifest>()`) also
   implicitly makes the target type a root, because the generator must be able to build it.

   > **Declare the attribute in the test project, not production code.** ModelBuilder is intended
   > for test-automation projects only. The assembly-level form
   > (`[assembly: GenerateModelBuilder(typeof(Manifest))]`) lets you opt a production type into
   > generation from **inside the test assembly** without touching the type's own source — so the
   > `[GenerateModelBuilder]` attribute never ships in deployed/production binaries. Prefer it over
   > the type-level `[GenerateModelBuilder]` on a production type, which would bake a
   > test-only attribute into the shipped assembly and leak information (what gets fabricated in
   > tests, internal type shapes) that has no business being in production metadata. Reserve the
   > type-level form for types that themselves only ever exist in the test project.

**Why an attribute is even needed under pure SG:** call-site discovery only sees the *static* type
argument. When code does `Model.Create<Shape>()` and `Shape` is abstract with a mapping to
`Circle`, the generator learns `Circle` from the mapping. But when a consumer wants builders for a
set of types that are only resolved at runtime (e.g. a polymorphic registry), the attribute is the
compile-time signal that says "emit these too". If a needed type is neither reachable from a
`Create<T>()` root nor attributed nor mapped, the generator raises a diagnostic at build time
rather than failing at runtime. (Resolves §12.2.)

### 6.2 Every buildable type is discovered at compile time

The consequence of §6.1 stated plainly: **the library can only build a type the generator
discovered while compiling the consumer's code.** There is no runtime registration call and no
reflection fallback. The emitted registry (§5.3) is a *product* of discovery, not a list anyone
maintains by hand. Discovery happens in three tiers, and only the third is manual:

| Tier | Trigger | Manual? | Typical case |
|------|---------|---------|--------------|
| 1. Reachability (primary) | `Model.Create<T>()` / `Populate<T>()` / `Create<T>(args)`, and everything reachable from `T` through constructor parameters and populated members | No | `Create<Order>()` automatically pulls in `Customer`, `List<OrderLine>`, `OrderStatus`, … — the whole subtree |
| 2. Mapping | `Mapping<TAbstract, TConcrete>()` makes `TConcrete` a root | No | `Mapping<Stream, MemoryStream>()` |
| 3. Attribute | `[GenerateModelBuilder]` on a type or `[assembly: GenerateModelBuilder(typeof(X))]` | Yes | A type built only polymorphically through a base/interface, or only ever referenced via a runtime `Type` |

The mental model is **"if the type is named somewhere in the compilation, it's covered"** — not
"keep a registration list current". For a typical test project this means essentially zero
annotations: the tests already name every type via `Create<T>()`.

> **Attributes belong in the test assembly.** Because ModelBuilder is a test-automation tool, the
> tier-3 attribute should be declared in the **test project**. Use the assembly-level
> `[assembly: GenerateModelBuilder(typeof(X))]` form to opt a production type in from the test
> assembly so no test-only attribute is compiled into deployed/production binaries (which would
> leak test-fabrication and internal-shape information into shipped metadata). See the callout in
> §6.1.

What is *not* discoverable, and therefore not supported (per §1 non-goals):

- A `Type` computed at runtime — e.g. loaded from a plugin assembly, or chosen reflectively in a
  test harness — that no `Create<T>()`, mapping, or attribute also names.
- Any type behind a visibility wall the generated code can't cross (§8.3).

#### 6.2.1 The non-generic `Model.Create(Type)` overload

`Model.Create(Type)` resolves the runtime `Type` to a pre-emitted builder via the registry — a
dictionary lookup plus a virtual call, never reflection:

```csharp
public static object Create(Type instanceType, params object?[]? args)
{
    if (!ModelBuilderRegistry.TryGet(instanceType, out var builder))
    {
        throw new ModelBuildException(
            $"No builder was generated for '{instanceType.FullName}'. Make it discoverable: " +
            $"call Model.Create<{instanceType.Name}>() somewhere, add a Mapping<,> to it, or " +
            $"annotate it with [GenerateModelBuilder].",
            FailureKind.NoBuilderForType);
    }

    return builder.Create(new BuildContext(/* config */), args);
}
```

Two behaviours make the common case painless:

- **`typeof(X)` constants are discovery roots.** When the argument is a compile-time `typeof(X)`
  literal — e.g. `Model.Create(typeof(OrderStatus))`, as a custom type-creator might call it — the
  generator sees `X` and emits a builder for it, so the call just works without any extra
  annotation.
- **Runtime `Type` values fall through to a clear exception.** A `Type` computed at runtime with no
  other discovery path has no registry entry and throws the actionable `ModelBuildException` above
  (§9.3), rather than silently reflecting.

---

## 7. Replacing each reflection point

| Current (reflection/dynamic) | vNext (generated) |
|------------------------------|-------------------|
| `Activator.CreateInstance(type,args)` | Emitted `new T(...)` with the chosen constructor |
| `GetConstructors` + arg matching | Compile-time constructor selection + typed overloads |
| `GetProperties` + `SetValue` | Emitted direct property assignments in execute order |
| `MakeGenericType(ICollection<T>)` + `GetMethod("Add")` | Emitted typed `Add`/array fill |
| `ExpandoObject` relative values | Sibling values passed as typed locals/parameters |
| `DefaultTypeResolver` assembly scan | Explicit compile-time `Mapping<TSource,TTarget>()` + diagnostics for ambiguity (#242) |
| `Convert.ChangeType` for nullable | Generated typed conversions |
| Runtime `ExecuteOrderRules` sort | Baked statement order |
| Write-only property throws (#282) | Generator skips write-only members + diagnostic |

---

## 8. Extensibility in vNext (intentionally smaller)

Keep only what real usage needs:

- **`IConfigurationModule`** — reusable configuration (ignore rules, value overrides, mappings).
  Kept; this is the main customization seam used in practice.
- **Ignore rules** — both targeted and type-agnostic (§8.1).
- **Type mapping** — `Mapping<TSource,TTarget>()`, validated at compile time.
- **Value overrides** — strongly-typed tuning hooks (e.g. age range, collection counts) replacing
  the open-ended `UpdateValueGenerator<T>`/`UpdateTypeCreator<T>` reflection-tuning.
- **Custom value source** — one unified, **generic** abstraction (`IValueSource<T>`) that supplies
  *and optionally populates* a strongly-typed value for a specific type/name (§8.2), avoiding the
  boxing a non-generic `object`-returning source would incur. Covers the custom-type-creator and
  ignore-a-problematic-property cases without exposing the old
  `ITypeCreator`/`IValueGenerator`/`IBuildAction` machinery.

Removed/retired: `IExecuteStrategy`, `IBuildProcessor`, `IBuildAction`, `IBuildCapability`,
`IConstructorResolver`, `IParameterResolver`, `IPropertyResolver`, `ITypeResolver`, the
`Type`/`PropertyInfo`/`ParameterInfo` generator overload triplets, and `CacheLevel` (caching is
unnecessary when there is no per-call reflection). This directly advances issue #293.

### 8.1 Ignore rules: targeted and type-agnostic

Two shapes, both evaluated by the generator at compile time (so they cost nothing at runtime):

```csharp
// Targeted: ignore a specific member on a specific type.
Model.Ignoring<Person>(x => x.FirstName).Create<Person>();

// Type-agnostic: ignore any member matching a predicate, across every type in the graph.
// e.g. "ignore any property named Description"
configuration.IgnoreAny(member => member.Name == "Description");
configuration.IgnoreAny(member => member.Name.EndsWith("Internal"));
```

Because the generator visits every member it emits population code for, a type-agnostic rule is
applied by simply **not emitting** the assignment for matching members. The predicate runs against
generator-side member metadata (name, declared type, accessibility), not at runtime. This makes
"ignore any property named `Description`" a first-class, zero-cost rule. (Resolves §12.3.)

> Constraint: type-agnostic predicates operate on information available to the generator —
> member name, type, and accessibility. They cannot inspect runtime instance state (that is what
> `Set`/`SetEach` post-build tweaks are for).

### 8.2 Unified value source (merging type creators and value generators)

The current library splits leaf-value creation (`IValueGenerator`) from object creation +
population (`ITypeCreator`). The two are nearly the same shape, and consumers only ever needed
"give me a value for this type/name". vNext collapses them into **one** opt-in abstraction. Because
everything is source-generated and the generator knows the concrete target type at each call site,
the abstraction is **generic** — `IValueSource<T>` — so value types are produced and assigned
without boxing:

```csharp
public interface IValueSource<T>
{
    // Produce a strongly-typed value. May call ctx.Create<TDependency>() to build dependencies.
    // Returns T directly — no boxing for value types (int, Guid, enums, DateTimeOffset, …).
    T Create(BuildContext ctx, in BuildTarget target);     // target = type + optional member name
}
```

**One method, `Create`, and that is the whole consumer-facing contract.** Two things that were
separate concepts in v8 are deliberately *not* on this interface:

- **No `Populate` on the source.** `Create` always returns a fully-built value. For an object
  result the **engine** runs the normal population pass over the new instance *after* `Create`
  returns — applying ignore rules, execute order, and relative values exactly as for any built
  type. The source author never calls populate and never re-implements that pass; they just hand
  back the constructed instance. (This mirrors v8, where `Create` already fell through to
  `Populate` internally.) The standalone `Model.Populate<T>(existing)` entry point is served by the
  generated `IModelBuilder<T>.Populate` — a builder concern, not a value-source concern — so it
  does not need to appear on `IValueSource<T>`.
- **No `Matches` on the source.** Whether a source applies to a target is declared at
  **registration** (§8.2.2), where the generator can read it at compile time, rather than as a
  runtime method the engine must call per candidate. This keeps the consumer's implementation to a
  single obvious method and moves matching into generated code (§8.2.8).

Rationale for a **single** interface rather than a leaf/object split: the only real difference
between "leaf value" and "created object" is *whether the engine populates afterwards*, and the
generator already knows that statically from `T` (primitive/enum/string ⇒ leaf; class/struct with
settable members ⇒ populate). Forcing the consumer to choose `IValueSource<T>` vs an
`IObjectSource<T>` would push a decision onto them that the generator can make correctly every time
— and getting it wrong (e.g. implementing the leaf interface for a type that should be populated)
would be a silent bug. One interface, one method, zero lifecycle choices for the author.

- A leaf source (`GuidValueSource : IValueSource<Guid>`, `FirstNameValueSource :
  IValueSource<string>`) returns the whole value — exactly today's `IValueGenerator` role, strongly
  typed.
- An object source (`ApiResultValueSource : IValueSource<ApiResult>`) calls the
  chosen constructor in `Create` and returns the instance; the engine populates it afterwards —
  exactly today's `ITypeCreator` role. To suppress that population, register the type's members as
  ignored (or mark the source as producing a complete value); it is a configuration choice, not a
  second interface.
- The built-in name/address/enum/collection sources are generated and registered automatically;
  custom sources are registered through a module.

**Why generic, and where boxing still (unavoidably) happens.** The generated hot path assigns a
value straight into a typed member — `instance.Age = ageSource.Create(ctx, target)` — so a
`IValueSource<int>` hands back an `int` with no allocation. The generator emits a call to the
closed-generic source for each target type, so there is no `object` in the value path. The **one**
place a box is unavoidable is the non-generic `Model.Create(Type)` boundary (§6.2.1): its signature
returns `object`, so a value-type result is boxed there — but that is an explicit, opt-in API
choice at a single call, not a per-member cost across the whole graph. The generic
`Model.Create<T>()` path never boxes.

> Registry note: the runtime `Type → builder` registry (§5.3) is keyed for the non-generic
> `Create(Type)` path and for polymorphic/nested dispatch, where an `object`-returning entry point
> is inherent. Builders still expose the strongly-typed `IModelBuilder<T>` surface for the generic
> path; the registry's `object` face is used only where a runtime `Type` forces it. Storage and
> dispatch are detailed in §8.2.9.

> Status: **resolved (§12.4).** Single generic `IValueSource<T>` with one `Create` method;
> population is an engine pass; matching is declared at registration and compiled in where possible
> (§8.2.8); storage/dispatch uses the typed-static + registry split (§8.2.9). The built-in type
> coverage is the fixed in-box list in §8.2.10. What remains is prototype validation, not design.

#### 8.2.1 The built-ins are not privileged

There is **no hard-coded list of "special" types**. `FirstName`, `Email`, `Country`, `Phone`,
etc. are simply value sources that ship in the box and are registered into the default
configuration the same way a consumer registers their own. A custom source competes on equal
footing and can match the same target a built-in matches. So "is the design rigid?" — no: the
built-ins are the *default registrations*, not a closed set.

#### 8.2.2 Registering a brand-new source (e.g. Planet names)

A consumer writes a source — one `Create` method — and declares *what it matches* at registration.
The `T` of `IValueSource<T>` already fixes the **type**; the registration adds the **name/predicate**
constraint, which the generator reads at compile time:

```csharp
public sealed class PlanetNameValueSource : IValueSource<string>
{
    private static readonly string[] _planets =
        { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" };

    public string Create(BuildContext ctx, in BuildTarget target) =>
        _planets[ctx.Random.Next(_planets.Length)];
}

public sealed class SpaceModule : IConfigurationModule
{
    public void Configure(IConfiguration configuration) =>
        configuration.AddValueSource<string, PlanetNameValueSource>()
                     .ForMembersNamed("Planet", "PlanetName", "HomeWorld");   // compile-time match
}

// Usage
var explorer = Model.UsingModule<SpaceModule>().Create<Explorer>();
```

The source itself carries no `Matches` method — the `.ForMembersNamed(...)` declaration is what the
generator binds. A source registered with no name constraint matches *any* member of type `T`
(priority-ordered against the built-ins per §8.2.3). See §8.2.8 for the full matching vocabulary and
which forms compile in vs. fall back to runtime.

#### 8.2.3 Overriding or specialising a built-in (priority)

Sources are ordered by **priority** (descending), and the first match wins. Consumer-registered
sources default to a priority above the built-ins, so registering a source that matches `string` +
`Country` simply **replaces** the built-in `Country` behaviour for that target — no need to remove
anything. A narrower match (more specific member-name/type predicate) plus higher priority is how
you specialise just one case while leaving the rest of the built-ins intact.

#### 8.2.4 Extending / composing an existing source

A custom source can **delegate to** the built-in instead of reimplementing it. Built-in sources are
public and resolvable from the `BuildContext`, so a consumer can wrap one and post-process its
output:

```csharp
// "Title-case planet" that reuses the built-in string/name machinery, or
// a source that starts from an existing generator's value and transforms it.
public sealed class PrefixedCompanyValueSource : IValueSource<string>
{
    public string Create(BuildContext ctx, in BuildTarget target)
    {
        // Reuse the built-in Company source rather than duplicating its data set.
        var company = ctx.CreateUsing<string, CompanyValueSource>(target);
        return $"ACME — {company}";
    }
}

// Registered with: .ForMembersNamed("TradingName")
```

This keeps the embedded data sets (names, companies, locations) as the single source of truth while
letting consumers layer behaviour on top.

#### 8.2.5 Sources that span many types (avoiding an implementation explosion)

A fair objection to `IValueSource<T>`: today a single class like `NumericValueGenerator` produces
`byte`, `short`, `int`, `long`, `float`, `double`, `decimal`, … (and their nullable variants) from
one body via `Generator.NextValue(type, …)`. A naive "one `IValueSource<T>` per type" reading would
explode that into a dozen classes. It does not have to. There are three tiers, chosen by how broad
the type set is:

**Tier A — built-in primitive families are generator-emitted, not expressed as public sources.**
The numeric/primitive/`enum`/`Guid`/`DateTime` families are a *closed, compile-time-known set*. The
generator owns them: for an `int` target it emits `ctx.Random.NextInt32(min, max)`, for a `double`
target `ctx.Random.NextDouble(min, max)`, and so on, against a **typed** `IRandomSource`
(`NextInt32`/`NextInt64`/`NextDouble`/… — no `object`). `NumericValueGenerator` therefore does not
become eleven `IValueSource<T>` classes; it disappears into generator codegen over the known
primitive set plus typed random methods. No public interface, no boxing, no explosion. This is how
the bulk of "many types in one class" is handled, because the broad-coverage generators in the
current library are exactly the built-ins. §8.2.7 generalises this: the generator *emits*
`IValueSource<T>` implementations per discovered type (enums, nullable, primitives) rather than
shipping them all as hand-written classes.

**Tier B — one class, many closed `IValueSource<T>` interfaces (consumer spanning a few types).**
A consumer who wants shared logic across a handful of types implements the interface several times
on a single class — the explosion is in interface declarations, not class count, and stays
boxing-free because each `Create` is type-specific:

```csharp
public sealed class ScaledNumberValueSource :
    IValueSource<int>, IValueSource<long>, IValueSource<double>
{
    int    IValueSource<int>.Create(BuildContext ctx, in BuildTarget t)    => Scale(ctx, 1);
    long   IValueSource<long>.Create(BuildContext ctx, in BuildTarget t)   => Scale(ctx, 1L);
    double IValueSource<double>.Create(BuildContext ctx, in BuildTarget t) => Scale(ctx, 1.0);

    private static T Scale<T>(BuildContext ctx, T unit) => /* shared logic */;
}

// Register the closed types it covers, each with the same member-name constraint:
configuration.AddValueSource<int,    ScaledNumberValueSource>().ForMembersMatching(n => n.EndsWith("Scaled"))
             .AddValueSource<long,   ScaledNumberValueSource>().ForMembersMatching(n => n.EndsWith("Scaled"))
             .AddValueSource<double, ScaledNumberValueSource>().ForMembersMatching(n => n.EndsWith("Scaled"));
```

On the modern TFM targets (`net8.0`+) the shared `Scale<T>` helper can use generic math
(`System.Numerics.INumber<T>`) so even the shared body is boxing-free; on `netstandard2.0` it falls
back to per-type branches internally. Either way the public shape is one class, and the hot path is
typed.

**Tier C — genuinely open / unbounded type set (rare escape hatch).** If a consumer truly needs one
logical source that handles an open set of types it can't enumerate at registration time, an opt-in
**non-generic** `IValueSource` (returns `object`, and therefore boxes value types) is available:

```csharp
public sealed class EverythingIsDefaultValueSource : IValueSource   // non-generic, boxes
{
    public object? Create(BuildContext ctx, in BuildTarget target) => Activator.CreateInstance(target.Type);
}

// Registered with a runtime predicate: .When(target => target.Type.IsValueType)
```

This is deliberately the *least* preferred tier: it reintroduces a box for value types and (if it
touches `Type` reflectively, as above) can pull reflection back in. It exists only so the model is
not a dead end for the unusual case. The built-ins never use it, so the common hot path stays typed
and reflection-free.

> Rule of thumb: **the broad, many-type generators that motivate this question are built-ins, and
> built-ins are Tier A (generator-emitted).** Consumers almost always want the single-type
> `IValueSource<T>`; Tier B covers "a few related types"; Tier C is the rarely-needed open escape
> hatch.

#### 8.2.6 Dependent (relative) values — e.g. Country-qualified phone number

The relative-value mechanism (today's `RelativeValueGenerator`) is exposed as a **first-class,
reflection-free capability** of `BuildContext`, not a privileged built-in. A custom source can read
sibling values that have **already been built on the current instance** and shape its output
accordingly. The execute-order rules (§5.2) guarantee the dependency is populated first.

```csharp
public sealed class CountryQualifiedPhoneValueSource : IValueSource<string>
{
    // Country dialing codes — could itself come from an embedded data set.
    private static readonly Dictionary<string, string> _dialingCodes =
        new() { ["Australia"] = "+61", ["United States"] = "+1", ["Japan"] = "+81" };

    public string Create(BuildContext ctx, in BuildTarget target)
    {
        // Read a sibling value already populated on the instance under construction.
        // Returns null if there is no Country in scope (top-level string, or not yet built).
        var country = ctx.GetSibling<string>("Country");

        var prefix = country is not null && _dialingCodes.TryGetValue(country, out var code)
            ? code
            : "+1";

        return $"{prefix} {ctx.Random.Next(100, 999)} {ctx.Random.Next(100000, 999999)}";
    }
}
```

Two pieces make this work without reflection or `dynamic`:

1. **`ctx.GetSibling<T>(name)`** reads from the value the generator is currently populating. Because
   members are assigned in generated statement order, and the generator emits the dependency
   (`Country`) before the dependent (`Phone`), the sibling is already set. This replaces the old
   `ExpandoObject` lookup with a typed, generated accessor.
2. **Ordering is configurable.** A consumer-supplied source can declare its dependency so the
   generator orders `Country` before `Phone`:

   ```csharp
   configuration.AddValueSource<string, CountryQualifiedPhoneValueSource>()
                .Build("Country").Before("Phone");   // build Country first, before Phone
   ```

   Read it left-to-right as a sentence: *build `Country` before `Phone`*. The subject of the
   ordering comes first, then the constraint.

   The built-in name/email/address ordering (§ DefaultConfigurationModule execute-order rules) is
   just the default set of these declarations and can be added to or overridden.

3. **Ordering cycles are a compile-time error.** The ordering declarations form a directed graph
   over members (an edge `Build("A").Before("B")` means *A must precede B*). The generator
   topologically sorts that graph to bake the population order (§5.2). If the declarations contain a
   cycle — `A` before `B` and `B` before `A`, directly or transitively through a chain
   `A → B → C → A` — there is no valid order, so the **generator emits a build error and
   compilation fails**, naming the members in the cycle:

   ```text
   error MB0012: Cyclic value ordering detected — no valid population order exists.
                 Country → Phone → Country
                 Remove or invert one of the .Before(...) declarations to break the cycle.
   ```

   This catches the mistake at build time, before any test runs, rather than producing a
   nondeterministic or partially-populated instance at runtime. It also covers the case where two
   *different* modules each contribute one edge that together close a loop — the generator sees the
   merged graph for the whole compilation.

   > Self-edges (`Build("A").Before("A")`) are likewise rejected, and a redundant-but-consistent
   > edge (declaring the same order twice) is collapsed, not flagged.

> Constraint (consistent with §8.3 and §6.1): a custom source can only read siblings the generator
> actually emitted population code for, and can only build dependency types that are visible to the
> generated code. If `Phone` depends on `Country` but `Country` is ignored or unmapped, the build
> log (§9.2) records that the sibling was absent and the phone fell back to its default prefix.

#### 8.2.7 The generator emits value sources — it doesn't only consume pre-written ones

The unifying insight behind §8.2.5 (and the answer to enums and nullable): **the source generator
writes `IValueSource<T>` implementations itself, per closed type it discovers, rather than relying
solely on hand-written sources shipped in the ModelBuilder binary.** A "value source" is partly a
codegen template the generator instantiates for a concrete `T`, and partly the consumer-authored
sources from the earlier sections. Both feed the same registry. This is what keeps the model from
either exploding into hand-written classes or falling back on reflection.

**Enums.** When the generator discovers an `enum` target `E`, it emits a closed
`IValueSource<E>` specialised to *that* enum's members — no `Enum.GetValues`/`Enum.Parse`/`Activator`
at runtime:

```csharp
// GENERATED for: enum Gender { Female, Male, NonBinary }
internal sealed class GenderValueSource : IValueSource<Gender>
{
    // Members captured at compile time from the enum declaration.
    private static readonly Gender[] _values = { Gender.Female, Gender.Male, Gender.NonBinary };

    public Gender Create(BuildContext ctx, in BuildTarget target) =>
        _values[ctx.Random.Next(_values.Length)];
}
```

*Flags* enums are detected at compile time (the generator sees `[Flags]` on the declaration) and emit
a bitwise-combine body instead — OR-ing a random subset of the defined members — again specialised
to the concrete enum, with the members known statically:

```csharp
// GENERATED for: [Flags] enum Access { None=0, Read=1, Write=2, Delete=4 }
internal sealed class AccessValueSource : IValueSource<Access>
{
    public Access Create(BuildContext ctx, in BuildTarget target)
    {
        // Combine a random subset of the non-zero members; emitted from the static member list.
        var result = Access.None;
        if (ctx.Random.NextBool()) result |= Access.Read;
        if (ctx.Random.NextBool()) result |= Access.Write;
        if (ctx.Random.NextBool()) result |= Access.Delete;
        return result;
    }
}
```

The empty-enum and single-member edge cases (today handled with `Activator.CreateInstance` /
`values.GetValue(0)`) become a compile-time decision: the generator emits `default(E)` or the single
member literal directly.

**Nullable value types — no interface duplication.** A consumer never writes `IValueSource<T?>`
alongside `IValueSource<T>`. The generator handles `Nullable<T>` as a *wrapper* over the underlying
`T` source. When it discovers a `T?` target, it emits a thin nullable adapter that reuses the
existing `IValueSource<T>` and applies the null-percentage policy:

```csharp
// GENERATED for any discovered T? where an IValueSource<T> exists (e.g. int?, Gender?)
internal sealed class NullableValueSource<T> : IValueSource<T?> where T : struct
{
    private readonly IValueSource<T> _inner;
    private readonly int _nullPercent;

    public T? Create(BuildContext ctx, in BuildTarget target) =>
        ctx.Random.Next(100) < _nullPercent
            ? null
            : _inner.Create(ctx, target);
}
```

So `int?`, `Gender?`, `DateTimeOffset?` all work by composing the underlying `int`/`Gender`/
`DateTimeOffset` source — one generic adapter, not a duplicated source per type, and no boxing
(`T?` is itself a value type). The `AllowNull`/`NullPercentageChance` knobs from the current
`INullableBuilder` become configuration on the adapter.

**Why this resolves §8.2.5's "explosion".** The dozen numeric types, every enum, and every nullable
variant are not hand-written classes in the binary — they are **generator output**, produced only
for the closed types a given compilation actually builds. A test that only ever creates `Gender` and
`int?` gets exactly `GenderValueSource` and `NullableValueSource<int>` emitted, nothing else. The
ModelBuilder binary ships the *templates and primitives* (typed `IRandomSource`, the nullable
adapter shape, the embedded data readers); the generator instantiates them per discovered type.
Hand-written `IValueSource<T>` (§8.2.2–8.2.6) and the non-generic escape hatch (§8.2.5 Tier C)
remain available for what the generator can't infer.

#### 8.2.8 Matching: compile it in wherever possible, runtime only as a last resort

A source no longer carries a `Matches` method (§8.2). Instead, *what it matches* is declared at
registration, and the generator evaluates as much of that declaration as it can at compile time —
emitting a **direct call** to the chosen source for a member, with no runtime selection step. The
matching vocabulary, ordered from "fully compiled in" to "runtime fallback":

| Declaration | Evaluated | Notes |
|-------------|-----------|-------|
| `T` of `IValueSource<T>` (the type itself) | **Compile time** | The generator binds each member to a source by its closed member type. This alone covers most leaf values. |
| `.ForMembersNamed("A", "B", …)` (literal set) | **Compile time** | Member names are in the syntax tree; the generator compares them and emits the winning source directly. |
| `.ForMembersMatching("regex-literal")` | **Compile time** | A **string-literal** regex is evaluated by the generator against each candidate member name. The emitted code contains no regex engine call. |
| `.ForMembersMatching(n => /* simple expression */)` | **Compile time when interpretable** | Common lambda shapes are read as an **expression** and translated into generated comparisons — `n => n.EndsWith("Scaled")`, `n => n.StartsWith("Is")`, `n => n.Contains("Id")`, `n => n == "X"`, and `&&`/`||`/`!` combinations of these. The generator recognises the pattern and compiles it in; it does **not** execute the consumer's compiled delegate. |
| `.When(target => /* arbitrary */)` (opaque delegate) | **Runtime** | The explicit escape hatch. The predicate is an opaque `Func` the generator can't read, so it is invoked at runtime against candidate members. Use only when the condition genuinely depends on runtime state. |

The design intent (your decision #2): **move every decision the generator can make into generated
code; only `.When(...)` falls through to runtime.** Two practical consequences:

- **Known patterns are interpreted from the expression, not executed.** `.ForMembersMatching(...)`
  accepts an `Expression<Func<string,bool>>` (not a `Func<>`), so the generator can inspect the
  expression tree and recognise the supported shapes above, lowering them to plain string
  comparisons in the emitted code. An unsupported shape inside `.ForMembersMatching(...)` is a
  build diagnostic ("this predicate can't be compiled in — use `.When(...)` for runtime
  evaluation"), so a consumer never *silently* gets a slow path.
- **Mixed priority needs a small runtime shim — and only then.** If two sources could match the
  same member and at least one used `.When(...)`, the generator can't fully resolve the winner at
  compile time, so it emits a tiny per-member selection shim that consults the runtime predicate.
  Members resolved entirely by type + declarative name/regex/expression get a direct, shim-free
  call. The slow path is contained to exactly the members that opted into it.

#### 8.2.9 Storage and dispatch (closed-generic sources without boxing)

"Closed-generic source" = `IValueSource<T>` with a concrete `T` (`IValueSource<int>`,
`IValueSource<Gender>`). The challenge is holding many *differently-typed* sources and dispatching to
the right one on two different paths without boxing the produced value. vNext uses a two-part scheme
(your decision #4 — keep no-reflection, high-performance, broad reach):

1. **Generic hot path — typed static slot (no dictionary, no box).** For the common
   `Model.Create<T>()` and every nested *statically-typed* build, the generator emits a direct call
   to a per-`T` static holder:

   ```csharp
   internal static class ValueSource<T> { public static IValueSource<T> Instance = null!; }
   // generated registration: ValueSource<int>.Instance    = new Int32ValueSource();
   //                         ValueSource<Gender>.Instance  = new GenderValueSource();

   // generated use (no lookup, no box — direct typed field access then a typed call):
   T value = ValueSource<T>.Instance.Create(ctx, target);
   ```

   The CLR gives one `Instance` slot per closed `T`, so this is a field read plus a virtual call —
   no dictionary, and the value never touches `object`.

2. **Runtime-`Type` path — heterogeneous registry, source boxed (value not).** For the non-generic
   `Model.Create(Type)` (§6.2.1) and polymorphic/nested dispatch where only a runtime `Type` is
   known, a `Dictionary<Type, object>` maps `typeof(T)` to the **source instance**:

   ```csharp
   // registry: _sources[typeof(int)] = new Int32ValueSource();   // storing a REFERENCE as object — no real box
   var src = (IValueSource<T>)_sources[typeof(T)];                // retrieve + cast at a generic call site
   T value = src.Create(ctx, target);                            // typed call — the value T is NOT boxed
   ```

   The key point: the *source object* is a reference type, so storing it as `object` is just a
   reference, not an allocation. The thing we care about — the produced value `T` — stays unboxed
   because `Create` is invoked through the typed `IValueSource<T>`. The only value box in the whole
   system remains the one `Model.Create(Type)` already concedes at its `object`-returning boundary.

This composes cleanly: the typed-static path serves the overwhelmingly common generic calls with
zero overhead; the registry serves the rare runtime-`Type` calls and accepts only the single,
already-documented boundary box. No reflection is used on either path — both are plain field/virtual
calls over generator-emitted registrations.

#### 8.2.10 Built-in type coverage (in-box only, strategic, no third-party dependencies)

The generator emits sources for a **fixed, curated set of .NET runtime types** (your decision #3).
The guiding rules: only types in the .NET runtime / `netstandard2.0`+ modern-TFM surface (so
ModelBuilder takes **zero** third-party dependencies), the type-matched set stays small and closed,
and breadth comes from *name*-matched string data rather than an ever-growing type list. TFM-specific
types are included in the corresponding target's output (§10).

**Tier 1 — primitives and core value types (always):**
`bool`, `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `float`, `double`,
`decimal`, `char`, `string`, `Guid`; every `enum` (emitted per §8.2.7, incl. `[Flags]`); and
`Nullable<T>` of any supported value type (adapter per §8.2.7).

**Tier 2 — ubiquitous BCL types that appear in models (default on):**
`DateTime`, `DateTimeOffset`, `TimeSpan`, `Uri`, `Version`, `IPAddress` (`System.Net.Primitives`,
in-box), `TimeZoneInfo`. **Modern TFM targets (`net8.0`/`net9.0`/`net10.0`) additionally:**
`DateOnly`, `TimeOnly`, `Int128`, `UInt128`, `Half` — these are in-box on the modern targets and
harmless to include even if rarely used in test models, and are simply absent from the
`netstandard2.0` output. Newly suggested beyond the v8 set: `Version`, `Int128`/`UInt128`/`Half`
(modern TFMs), and `byte[]` (random buffer) which is common on DTOs.

**Tier 3 — entity-style string data, matched by member *name* (default on):**
The embedded-data sources — `FirstName`, `MiddleName`, `LastName`, `FullName`/`Name`/`DisplayName`,
`UserName`, `Email`, `DomainName`, `Company`, `Country`, `State`, `City`, `Suburb`, `PostCode`,
`Phone`, `TimeZone`, plus `Age`/`DateOfBirth` and the gender-aware name relationships. These are all
`string`/numeric typed but **match on member name** (as a whole PascalCase/camelCase word, so short
aliases like `Age` never fire for `PageCount`), so they never fire for an arbitrary `string` unless
the name matches. This is where the "realistic data" breadth lives without expanding the type list.

**Cross-field consistency via scoped values.** Related members of one instance must agree, so the
built-in sources share data through `IBuildContext.GetOrAddScopedValue<T>(key, factory)` — a cache
whose lifetime is the instance currently being populated (the same lifetime as the sibling scope).
The first location-style member (`Country`/`State`/`City`/`PostCode`/`Phone`) caches one `Location`
row and the rest read it, so an instance never produces an impossible address like
`London, New South Wales, India`. `DateOfBirth` is the definitive value of the birth-date/age pair —
the scope caches one date of birth and `Age` is calculated as the completed years between it and the
reference date, so the two always agree regardless of which member is built first. A separate instance elsewhere in the
graph gets its own scope and therefore its own location and age. This method is also the public
extension point: a consumer adding several value sources around a shared data set can have the first
source cache a chosen data item under a key and the others read it for coherent, related values.

**Tier 4 — collections (structural, generator-emitted):**
`T[]`, `List<T>`, `IList<T>`, `IReadOnlyList<T>`, `IEnumerable<T>`, `ICollection<T>`,
`IReadOnlyCollection<T>`, `Dictionary<K,V>`, `IDictionary<K,V>`, `IReadOnlyDictionary<K,V>`,
`HashSet<T>`, `ISet<T>` — built structurally from the element/key/value sources, with the
`MinCount`/`MaxCount` controls (and the #188 `MaxCount`-without-`MinCount` validation).

**Explicitly out of scope — no built-in source (require a `Mapping<,>` or custom source):**
`Stream` and other abstract IO, `Type`, `Task`/`Task<T>`, `Expression`, delegates, `object`,
`dynamic`, `System.Text.Json`/`System.Xml` DOM types, and **all third-party types**. These produce
the clean §9.3 "no value source — add a mapping or a source" diagnostic rather than a guess, keeping
the dependency surface at zero.



### 8.3 Visibility constraint (what the generator can build)

The generator can only emit code that the **consuming compilation** is allowed to write by hand.
Concretely:

- **Public** types, constructors, and members: always supported.
- **Internal** types/constructors/members: supported **only** when the generated code can see them
  — i.e. they are declared in the compilation being built, or exposed to it via
  `[InternalsVisibleTo]`.
- **Private / protected** types, constructors, and members: **not supported.** There is no
  reflection path to reach them, and emitting code that references them would not compile.

When a build root requires an inaccessible constructor or member, the generator raises a build
diagnostic naming the type and the inaccessible member, instead of silently degrading. This
replaces the current library's reflection-based access to non-public constructors. (Resolves
§12.5.)

---

## 9. Data, value generation, and diagnostics

### 9.1 Data and value generation

- Keep the embedded `.txt` data sets (names, companies, locations, domains, time zones) and the
  relative-value behavior (gendered first names; email derived from name + domain). These move
  behind reflection-free readers and generated call sites.
- Keep the random source behind `IRandomSource` (seedable for deterministic tests).
- Address open data requests where cheap: alternative `Country` matches (#345), and validate
  collection-count configuration so `MaxCount` without `MinCount` cannot throw (#188).

#### 9.1.1 `IRandomSource` requirements (lessons from the v8 `RandomGenerator`)

The v8 `RandomGenerator` underpins every numeric/byte value in the library, and a review of it
surfaced concrete defects that the vNext `IRandomSource` must fix by construction. These are
acceptance criteria, not aspirations:

- **Thread-safe.** v8 shares one `static readonly Random` across all instances; `System.Random` is
  not thread-safe, and concurrent access (e.g. parallel xUnit collections) can corrupt its state so
  it returns `0.0`/min values or correlated garbage. `IRandomSource` must be safe under parallel use
  — `Random.Shared` on modern TFMs, a `ThreadLocal<Random>`/per-thread instance on `netstandard2.0`,
  or equivalent. No shared mutable `Random` touched without isolation.
- **Seedable and reproducible.** v8's `new Random()` is private, static, and unseeded — a failing
  test can't be reproduced. `IRandomSource` must accept a seed, expose the seed in play, and surface
  it in the build log and `ModelBuildException` (§9.2, §15.5) so any generated-value failure is a
  deterministic repro.
- **Typed, allocation-free API (no `object`, no reflection).** v8 routes everything through
  `object NextValue(Type, object, object)` with `Convert.ChangeType` and, for nullable,
  `Activator.CreateInstance` — boxing every value and using reflection on the hot path.
  `IRandomSource` must expose typed methods (`NextInt32`, `NextInt64`, `NextDouble`,
  `NextDecimal`, …) returning the value type directly, consistent with the generic value-source
  model (§8.2) and the no-reflection goal (§1).
- **No precision loss for wide types.** v8 computes in `double` then converts back, so `long`/`ulong`
  above 2^53 are quantized and `decimal` loses precision; the boundary clamps over-represent
  `MaxValue`. `IRandomSource` must generate integer and `decimal` ranges with their own arithmetic
  (e.g. 64-bit integer math, `decimal` math), not via `double`.
- **Efficient bulk bytes.** v8's `NextValue(byte[])` runs the full per-`Type` value path **per byte**
  (validation + two `Convert.ToDouble` + double math + `Convert.ChangeType` + a box, each byte).
  `IRandomSource` must fill buffers in one call (`Random.NextBytes`-style).
- **Boundary validation at the public edge.** v8 throws `min > max` deep inside a private double
  routine after conversions. `IRandomSource` must validate `min <= max` at the public method with a
  clear message, and uniformly cover the requested range (no coin-flip half-range fallback for the
  full-`double` case).

These criteria are exercised by the BenchmarkDotNet baseline (§13 step 2 — the v8 boxing/`double`
path is a key allocation source to measure) and by unit tests carried into vNext (thread-safety
stress test, seeded-reproducibility test, wide-integer distribution test).

### 9.2 Build log (troubleshooting why a value was/was not created)

Generated code has the shape of the object graph baked in, but a developer still needs to see
**what the builder actually did** at runtime to understand why a type came out wrong (e.g. a
property left at its default, a mapping that didn't fire, a value source that didn't match). vNext
keeps a structured build log, opt-in and reflection-free.

- **Always-available, opt-in capture.** `BuildContext` carries an `IBuildLog`. By default it is a
  no-op sink (zero allocation, zero cost) so production test runs are unaffected. A developer turns
  it on with `Model.WriteLog<T>(Console.WriteLine).Create()` (kept from v8) or by passing a sink
  into a module.
- **Nested, indented trace.** The generated `Create`/`Populate` methods emit log calls at decision
  points so the output mirrors the build tree:

  ```text
  Create Person  (ctor: Person(string firstName, string lastName), source: generated)
    Populate Person
      Gender      -> Female            (value source: EnumValueSource)
      FirstName   -> "Janet"           (value source: FirstNameValueSource, relative: Gender=Female)
      Email       -> "janet@…"         (value source: EmailValueSource, relative: FirstName,LastName,Domain)
      Notes       -> SKIPPED           (ignore rule: IgnoreAny name == "Notes")
      Manager     -> null              (circular-reference guard: Person already in build chain)
  ```

- **Records the *reason*, not just the value.** Each entry names the deciding factor: which value
  source/builder ran, which constructor was chosen, which ignore/mapping rule matched, whether a
  circular-reference or depth guard fired, and the relative values consulted. This is exactly the
  information needed to answer "why didn't my property get set?".
- **Structured, not just text.** The log is a tree of typed entries (`BuildLogEntry` with kind,
  target type, member name, reason, chosen source) that the default formatter renders to the
  indented text above. Tools/tests can assert against the structure instead of scraping strings.
- **Survives failures.** The log is flushed on the exception path as well as the success path, so a
  thrown build still produces the full trace up to the point of failure. (Closes #349, where
  `WriteLog` previously lost messages when an exception was thrown.)
- **Generator-assisted detail.** Because emission happens at compile time, the log can include
  facts that are expensive or impossible to recover via reflection at runtime — the exact
  constructor signature picked, the source location of the `Create<T>()` call that rooted the
  build, and which `[GenerateModelBuilder]`/mapping introduced a type.

### 9.3 Exceptions (enough detail to find and fix the problem)

When a build fails, the exception must let a developer locate the failure in the object graph
without a debugger. vNext standardises on a `ModelBuildException` that carries the **path from the
root type down to the failing member**, plus the captured build log.

- **Tree path to failure.** Every exception includes the build chain as a readable path, e.g.:

  ```text
  ModelBuildException: Failed to create a value for 'RequestPayload.Stream.Buffer'.

  Build path:
    Order                       (root, from Model.Create<Order>() at OrderTests.cs:42)
      └─ Order.Payload : RequestPayload
           └─ RequestPayload.Stream : Stream          (abstract; no mapping)
                └─ Stream.Buffer : byte[]             ← failed here

  Reason: 'Stream' is abstract and has no Mapping<Stream, …> registered, and no value source
          matched it. Add a mapping (e.g. Model.Mapping<Stream, MemoryStream>()) or a value
          source for 'Stream'.
  ```

- **Actionable reason + suggested fix.** The message states the concrete cause (inaccessible
  constructor, abstract type without mapping, no matching value source, depth/circular guard hit,
  value-source threw) and, where the generator can infer it, the remedy (add a mapping, add an
  ignore rule, expose an `[InternalsVisibleTo]`, widen accessibility).
- **Structured payload, not just a message.** `ModelBuildException` exposes:
  - `BuildPath` — the ordered list of `(declaringType, member, memberType)` frames from root to
    failure (programmatically inspectable, also rendered into `Message`).
  - `TargetType` / `TargetMember` — what was being built when it failed.
  - `FailureKind` — an enum (`InaccessibleConstructor`, `UnmappedAbstractType`, `NoValueSource`,
    `CircularReference`, `DepthExceeded`, `ValueSourceThrew`, …) for catch-site branching and
    test assertions.
  - `BuildLog` — the full build trace from §9.2 up to the failure.
  - `InnerException` — the original exception when a constructor or value source threw, chained so
    the stack trace is preserved.
- **Generated context, cheaply.** The build path frames are pushed/popped on `BuildContext` as the
  generated code descends, so constructing the path costs nothing on the success path and is
  already in hand when an exception is raised. No reflection is needed to reconstruct it.
- **Compile-time failures stay compile-time.** Problems the generator can prove (inaccessible
  required constructor per §8.3, unreachable root per §6.1, unmapped abstract reached statically)
  are reported as **build diagnostics** with the same path/reason quality, so they surface before
  the code ever runs. Runtime `ModelBuildException`s are reserved for conditions only knowable at
  runtime (a value source throwing, a runtime depth/circular limit).

---


## 10. AOT, trimming, and target frameworks

- **Target framework set.** The current v8 library targets `netstandard2.0` only. vNext keeps
  `netstandard2.0` for broad reach (retained per #246) **and** adds every currently-supported modern
  .NET TFM — at time of writing `net8.0`, `net9.0`, and `net10.0`. The full set is:

  ```xml
  <TargetFrameworks>netstandard2.0;net8.0;net9.0;net10.0</TargetFrameworks>
  ```

  The list is maintained to track the supported modern .NET releases: as a TFM reaches end of
  support it is dropped, and each new LTS/STS release is added. `netstandard2.0` stays regardless,
  as the compatibility floor for older consumers (e.g. .NET Framework test projects).
- **Why multi-target rather than `netstandard2.0` alone.** The modern TFMs unlock APIs the older
  surface lacks and let the generator emit better code where available — e.g. `DateOnly`/`TimeOnly`,
  `Int128`/`UInt128`/`Half`, and `System.Numerics.INumber<T>` generic math for boxing-free shared
  numeric logic (§8.2.5), plus first-class trimming/AOT annotations. TFM-specific built-in value
  sources (§8.2.10) are compiled only into the targets that have the type, and are simply absent
  from the `netstandard2.0` output.
- Zero trim/AOT warnings on the modern targets; validated with a Native AOT smoke-test app in CI.
- The generator is an incremental Roslyn source generator (analyzer package), emitting code into
  the consumer's compilation. It is independent of the runtime library's TFM set — it runs in the
  compiler and supports consumers on any of the targeted frameworks.
- **No reflection fallback ships at all** (§1 non-goals). The runtime library contains no
  `System.Reflection`-based build path, so there is nothing for the trimmer/AOT to warn about.
  Unknown-at-compile-time requests fail fast (§6.1) rather than degrading to reflection.

---

## 11. Known issues addressed by this design

| Issue | How vNext addresses it |
|-------|------------------------|
| #347 self-reference stack overflow | Compile-time-aware circular guard in `BuildContext` |
| #283 `Exception.Data` overflow | Generated recursion/depth guard + ignore-by-default for problematic BCL members |
| #282 write-only property throws | Generator skips write-only members |
| #292 type mapping not always applied | Mapping resolved at compile time, single path |
| #242 non-deterministic type resolution | Explicit compile-time mapping; ambiguity = diagnostic |
| #188 `MaxCount` without `MinCount` | Validated, coerced configuration |
| #340 / #346 constructor default values | Honored in generated constructor calls |
| #349 `WriteLog` drops messages on throw | Build log flushed on the exception path (§9.2) |
| #293 shrink explicit API | Whole extensibility surface reduced (§8) |

---

## 12. Resolved decisions and remaining open questions

### Resolved

- **12.1 — No reflection retained.** vNext is a pure source generator. There is no hybrid engine
  and no runtime reflection fallback. `dynamic`/runtime-only types are no longer supported.
  Consumers needing the reflection design stay on the v8 line. (See §1 non-goals, §10.)
- **12.2 — Discovery.** `Model.Create<T>()` is the root of the type tree, found via call-site
  discovery; `[GenerateModelBuilder]` (assembly-level `typeof` or on the type) names extra roots
  that are only reached polymorphically; mappings implicitly add their target as a root. Anything
  unreachable is a build-time diagnostic. (See §6.1.)
- **12.3 — Type-agnostic ignore.** Supported. `IgnoreAny(member => ...)` lets a rule like "ignore
  any property named `Description`" apply across all types; the generator simply omits the
  assignment. Predicates see generator-side member metadata only. (See §8.1.)
- **12.4 — Unified value source.** `IValueGenerator` and `ITypeCreator` merge into a single
  **generic** `IValueSource<T>` with one `Create` method (§8.2). Population is an engine pass, not a
  source method; matching is declared at registration and compiled in wherever possible, with
  `.When(...)` as the only runtime fallback (§8.2.8); breadth (many-type generators, enums +
  `[Flags]`, nullable) is handled by the generator **emitting** per-type sources and a nullable
  adapter (§8.2.5, §8.2.7); storage/dispatch uses a typed-static slot on the generic hot path plus a
  `Dictionary<Type, object>` for runtime-`Type` dispatch, boxing the source reference but never the
  produced value (§8.2.9); built-in type coverage is the fixed in-box list with TFM-specific
  additions (§8.2.10). Prototype validation remains, but the design is settled.
- **12.5 — Visibility.** Build support is limited to types/constructors/members visible to the
  generated code: public always; internal when in-compilation or via `[InternalsVisibleTo]`;
  private/protected never. Inaccessible requirements are a build diagnostic. (See §8.3.)
- **12.6 — No back-compat shim.** vNext is a substantial rewrite and **accepts breaking changes** to
  keep the offering simple. There is no compatibility layer mapping old
  `IConfigurationModule.Configure(IBuildConfiguration)` calls (`AddIgnoreRule`, `AddTypeCreator`,
  `DefaultConfigurationModule`, custom `ITypeCreator`/`IValueGenerator`, etc.) onto the new
  configuration. Instead, the package ships a **migration guide in the README** (§14) that maps each
  removed/renamed API to its vNext equivalent, so a human — or an AI agent — can perform the
  migration deterministically.
- **12.7 — Rich compile-time + runtime diagnostics.** Decided **yes**, and broadened: invest
  heavily in compilation and error reporting (§15). `Model.Create(typeof(X))` with a constant
  `typeof` that names an unbuildable type is an **analyzer warning at the call site** (in addition
  to the runtime `ModelBuildException`), and that is one entry in a full, documented diagnostic
  catalogue with stable IDs, actionable messages, suggested fixes, and code-fix providers where
  practical. Quality of diagnostics is treated as a first-class deliverable, not a nicety.

- **12.8 — Typed constructor invocation (`Model.Construct<T>().From(...)`). Resolved — implemented.**
  Today `Model.Create<T>(params object?[]? args)` is the only construction entry point: the
  args are loosely typed `object?[]`, which **boxes** value-type arguments and allocates an array,
  is matched at **runtime** (an unbuildable arg list throws `ModelBuildException` rather than failing
  to compile), and gives **no per-type IntelliSense** — the generic `Create<T>` shows only
  `params object?[]?`, never `T`'s actual constructor parameters or their xmldoc.

  **Goal.** A typed, per-constructor entry point so that, for a given `T`, the editor offers exactly
  `T`'s constructors (parameter names + copied ctor xmldoc), arguments are passed with their real
  types (no boxing, compile-time arity/type validation), and the existing `Model.Create<T>()` stays
  exactly as-is.

  **Why the obvious shapes do not work (the constraints that force the design):**
  - `Create<T>(string, string)` cannot have **per-`T`** parameters — a C# generic method has one
    shared parameter list across every `T` (no C++-style specialization).
  - Return-type-only overloads (`Person Create(string,string)` / `Address Create(string,string)`)
    are illegal — overload resolution ignores return type, so two target types with the same ctor
    signature collide (CS0111). The same `(string,string)` / single-`int` ctors are extremely
    common, so this is the normal case, not a corner case.
  - Overloads differing **only by generic constraint** (`where T : FirstType` vs `where T :
    SecondType`) are also illegal — constraints are not part of the signature (CS0111). A constraint
    *does* filter IntelliSense, but it cannot **multiply** one `(string)` overload to cover two
    types separately.
  - Therefore the disambiguating information (which concrete type) must live in a **type-specific
    receiver symbol**, not in a type argument or a return type.

  **Chosen shape: `Model.Construct<T>().From(ctorArgs)`.**
  - `Construct<T>()` is a hand-written core method returning a `readonly struct Construction<T>` that
    **builds nothing**. It is complete and bound on its own, so it is a valid **live-IDE generator
    trigger** with no bootstrap gap (the generator keys off the closed `Construct<T>()` call exactly
    as it keys off `Create<T>()` today; Roslyn runs generators in the editor, so the `.From`
    overloads exist by the time the caret reaches `.From(`). This is the property the rejected
    `Model.Create<T>(args)` (args are the unbound expression being typed) and `Model.<Type>.Create(`
    (receiver unresolved until generated) both lacked.
  - `.From(...)` is a **generated extension method keyed on the receiver** `Construction<T>` — one
    overload per **public** constructor of `T`, emitted with the constructor's xmldoc copied via
    `IMethodSymbol.GetDocumentationCommentXml()`. Because the receiver is `Construction<ThirdType>`,
    completion offers **only** `ThirdType`'s constructors; `FirstType`'s
    `From(this Construction<FirstType>, …)` cannot appear. This resolves both collision cases:
    different types with the same ctor signature differ by **receiver type**; multiple ctors on one
    type differ by their (definitionally distinct) parameter lists. Because the typed args are
    supplied by the caller, the **non-selected** constructors' parameter types do not need to be
    buildable, so collecting all public constructors for emission does **not** expand the build
    graph.
  - **Discoverability (resolved: per-calling-namespace emission).** An extension method is only
    offered when its containing static class's namespace is imported. Rather than require the
    consumer to add a `using` (poor discovery) or emit a `global using` (would force the consumer to
    compile with C# 10+), the generator captures the **namespace of each `Construct<T>()` call site**
    and emits a `From` extension class **into that same namespace**, so the overloads are in scope
    with no consumer import and **no `LangVersion` floor** — the broadest, safest compatibility. The
    extension classes are emitted into the **single** generated file (additional namespace blocks),
    preserving the one-file invariant.
  - Rejected verbs: `With` (reads as mutation; near C# `with` expressions), `Using` (near `using`
    directives/statements), `WithParams`. `From` reads front-to-back ("construct T **from** these
    values"), has no keyword proximity.
  - The complexity is **entirely generator-side**; the runtime core needs no new build machinery
    **except** one public seam — `.From` is the first generated code that *starts* a build from the
    consumer assembly, and context creation / `EnterRoot` / log rendering are `internal` today. A
    single `public` helper, `Construction<T>.Execute(Func<IBuildContext, T> build)`, wraps context
    creation + `EnterRoot` + the `try/finally` log render, keeping the widening contained to one
    method.

  **Extensibility is preserved.** Every seam (mappings, typed + named custom value sources, ignore
  rules, sibling/scoped-value consistency, logging) is consulted **inside `BuildContext`**, not at
  the entry point, so they all apply automatically to the constructed object and its whole subgraph
  — *provided* `Construction<T>` carries the `BuildConfiguration` + log sink. To compose with the
  fluent chain (`Model.UsingModule<M>().Ignoring<…>().Construct<T>().From(…)`),
  `IModelConfiguration` gains `Construct<T>()` seeded from the accumulated configuration;
  `Model.Construct<T>()` returns an unconfigured one. A top-level mapping or root value source for
  `T` itself is moot under `Construct` (you named a concrete `T` and chose construction) but still
  applies to every nested member.

  **Constructor-assigned member suppression (resolved, applied to both paths).** The generated
  population must not re-randomise a member the constructor just set. The clean vNext rule is
  compile-time, not v8's runtime value-comparison: for a given constructor the generator knows its
  parameters, so it **omits from that construction's population** any settable member whose name
  matches a constructor parameter (case-insensitive) **and** whose type matches. This is applied
  **per construction path**:
  - each `.From` overload populates the complement of *its* constructor's members;
  - the existing `Create<T>()` path populates the complement of the **selected** constructor's
    members (this also corrects the long-standing README claim that ctor-assigned members are not
    overwritten — previously false);
  - the standalone `Model.Populate(instance)` path is unchanged and still assigns **all** settable
    members, because no construction happened there.

  Constructor-set members are still **recorded as siblings** (under their property name) before the
  complement is populated, so sibling-derived values (e.g. `Email` from `FirstName`/`LastName`) stay
  consistent with the supplied constructor arguments. Interaction to record: an explicit `.From(...)`
  argument therefore **overrides** any custom/built-in value source for that member (the member is
  skipped in population); nested members are unaffected.

---

### Remaining open

*None. All design questions are resolved.*

---



## 13. Suggested build-out order

1. **Modernize the existing repo first (before any new project).** Bring the current solution up to
   current conventions so every project added afterwards (benchmarks, generator, tests) inherits a
   consistent, pinned build:
   - **Central Package Management.** Add `Directory.Packages.props` with
     `<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>`, move every
     `PackageReference` version out of the `.csproj` files into `<PackageVersion>` entries, and
     enable `CentralPackageTransitivePinning` so transitive versions are pinned too.
   - **Version pinning.** Pin exact package versions (no floating ranges); pin analyzer and
     `Microsoft.CodeAnalysis.NetAnalyzers` versions; keep `Dependabot`/renovate updating the central
     file.
   - **SDK pinning.** Add a `global.json` pinning the .NET SDK (with `rollForward`) so local and CI
     builds use the same toolchain.
   - **Upgrade dependencies.** The current package versions are well out of date (e.g. test/analyzer
     packages from the v8 era). Bring every dependency to its current stable release as part of
     centralising them \u2014 test SDK, xUnit, the assertion/mock frameworks, `NetAnalyzers`, etc. \u2014 and
     re-run the existing test suite to confirm no behavioural regression from the upgrades before
     building on top.
   - **Shared build props.** Add `Directory.Build.props` to hoist the common settings currently
     duplicated across `.csproj` (`LangVersion`, `Nullable`, `TreatWarningsAsErrors`,
     `NeutralLanguage`, analyzer ruleset wiring), so projects stay thin.
   - **Migrate to `.slnx`.** Replace the legacy `ModelBuilder.sln` with the modern XML `.slnx`
     solution format.
   - This is purely infrastructure — no behavioural change to the v8 library — and it makes the
     baseline benchmark, the generator project, and the new test projects drop in cleanly.
2. **Baseline benchmarks (BenchmarkDotNet).** Before any rewrite work, add a BenchmarkDotNet
   project that measures the **current** library so we have a hard "before" to compare against.
   - Multi-target the benchmark host so the same suite runs on `net472`, `net8.0`, `net9.0`, and
     `net10.0` (net472 exercises the `netstandard2.0` consumption path; the modern TFMs cover the
     AOT-capable runtimes).
   - Measure `Model.Create<T>()` and `Model.Populate(instance)` across a spread of model shapes and
     **tree sizes/complexities**: a flat primitive POCO, a small nested graph, a deep/wide graph,
     one with collections, one with enums/nullable, and one with constructor-arg matching — so the
     numbers show how cost scales with graph size, not just a single type.
   - Capture **execution time, allocated bytes, and GC counts** (`[MemoryDiagnoser]`,
     `[GcServer]`/Gen0–2 columns); a seeded `IRandomSource` keeps runs comparable.
   - Run it against the current library and **record the results in markdown** (committed), as the
     reflection-based baseline.
   - **Retain this project in vNext.** It re-runs unchanged against the new library, and the
     old-vs-new delta becomes documentation in the new library's README/perf page — the concrete
     reflection-vs-source-generated/AOT improvement (faster `Create`, far fewer allocations, lower
     GC pressure) we expect to demonstrate.
3. Runtime core: `Model` facade, `BuildContext`, `IModelBuilder<T>`, `IRandomSource`, slim
   `IBuildConfiguration` + module model, `Set`/`SetEach`/`Ignoring`.
4. Generator MVP: discover `Model.Create<T>()`, emit builders for POCOs with parameterless and
   selected constructors + direct property population. Registry emission.
5. Collections/arrays/dictionaries, enums, nullable, structs.
6. Built-in value sources + embedded data + relative values (gender/name/email) on the generic
   `IValueSource<T>` (§8.2): the in-box type coverage (§8.2.10), generator-emitted enum/nullable
   sources (§8.2.7), declarative matching with the compile-in expression interpreter (§8.2.8), and
   the typed-static + registry storage scheme (§8.2.9).
7. Type mapping, targeted + type-agnostic ignore rules (§8.1), circular-reference guard,
   visibility diagnostics (§8.3). Build the value-ordering topological sort with **cyclic-ordering
   build error** (§8.2.6) so contradictory `.Before(...)` declarations fail compilation.
8. Build log + structured `ModelBuildException` with tree path (§9.2, §9.3); wire push/pop of build
   path frames into the generated descent.
9. **Diagnostics catalogue (§15)** as a first-class workstream, developed alongside steps 4–8 rather
   than after: the `MB####` IDs, analyzer for `Model.Create(typeof(X))` (§12.7) and the
   attribute-placement/ambiguity/dead-registration warnings, help links, and code-fix providers
   where practical. Each generator feature lands with its diagnostics, not without them.
10. AOT/trim validation app + CI; port the existing unit-test corpus as behavioral parity tests, and
    add diagnostic-snapshot tests asserting each `MB####` fires with the right location and message.
    Re-run the step-2 benchmarks against vNext and publish the old-vs-new delta.
11. README migration guide (§14) and the "not supported" section, written to be followed by a human
    or an AI agent.

---

## 14. Migration guide (v8 → vNext, README content)

There is **no back-compat shim** (§12.6). Migration is manual, but mechanical: the README ships a
mapping from each removed or changed v8 API to its vNext equivalent, structured so an AI agent can
apply it deterministically across a consumer's test project. The guide must cover at least:

**Unchanged — no action.** `Model.Create<T>()`, `Model.Create<T>(args)`, `Model.Create(typeof(T))`,
`.Set(...)`, `.SetEach(...)`, `Model.Ignoring<T>(x => x.Member)`, `Model.UsingModule<T>()`,
`Model.Mapping<TSource, TTarget>()`. These keep their shape, so the bulk of a typical test project
compiles unchanged.

**Mapped — mechanical replacement.**

| v8 API | vNext replacement | Notes |
|--------|-------------------|-------|
| `IValueGenerator` / `ValueGeneratorBase` | `IValueSource<T>` (§8.2) | One `Create` method; no `IsMatch`/`Generate(Type,…)`; matching moves to registration. |
| `ITypeCreator` / `TypeCreatorBase` (e.g. a creator for a constructor-only type) | `IValueSource<T>` (§8.2) | `Create` calls the constructor; the engine populates afterwards. |
| `configuration.AddValueGenerator<G>()` | `configuration.AddValueSource<T, S>().ForMembersNamed(...)` | Type fixed by `T`; name/predicate at registration (§8.2.2). |
| `configuration.AddTypeCreator<C>()` | `configuration.AddValueSource<T, S>()` | Same registration shape. |
| `UpdateValueGenerator<G>(g => g.X = …)` / `UpdateTypeCreator<C>(c => c.X = …)` | strongly-typed value-override hooks (§8 value overrides) | e.g. age range, collection `MinCount`/`MaxCount`. |
| `AddIgnoreRule<T>(x => x.Member)` | `Model.Ignoring<T>(x => x.Member)` or module `IgnoreAny(...)` (§8.1) | Targeted unchanged; type-agnostic is new. |
| `AddCreationRule(predicate, value, priority)` | a small `IValueSource<T>` registered with `.When(...)`/`.ForMembers...` | The fast-value shortcut becomes an ordinary source. |
| `DefaultConfigurationModule` (called explicitly in a custom module) | implicit default configuration | The default sources are registered automatically; a custom module only adds deltas. |

**Removed — no equivalent (the guide states the rationale and the alternative).**
`IExecuteStrategy`, `IBuildProcessor`, `IBuildAction`, `IBuildCapability`, `IConstructorResolver`,
`IParameterResolver`, `IPropertyResolver`, `ITypeResolver`, `CacheLevel`, and the
`Type`/`PropertyInfo`/`ParameterInfo` generator overload triplets (§8). Consumers using these were
extending the build pipeline directly; the guide points them at `IValueSource<T>`, mappings, ignore
rules, and `[GenerateModelBuilder]` as the supported seams, and notes the pure-SG constraints
(visibility §8.3, compile-time discovery §6.2) that make the old hooks unnecessary.

**Worked migration example.** The guide includes at least one full before/after for a realistic
consuming module: a custom `IConfigurationModule` that calls the default module, adds a couple of
ignore rules (e.g. ignoring a problematic framework property), and registers a custom type-creator
for a constructor-only type. That is the realistic shape of a consuming module and the clearest
template for an agent to pattern-match against.

**Not supported in vNext — call it out explicitly.** Beyond the per-API table, the README must carry
a plain "What this version no longer supports" section so a reader (or agent) can decide up front
whether vNext fits, rather than discovering a gap mid-migration. At minimum it states:

- **No runtime reflection / no `dynamic` or runtime-only types** (§1, §10). A `Type` known only at
  runtime — loaded from a plugin, computed reflectively — cannot be built; there is no reflection
  fallback. Such consumers stay on v8.
- **Only compile-time-discoverable types are buildable** (§6.2). If a type is not reached from a
  `Model.Create<T>()`, a mapping, or a `[GenerateModelBuilder]` attribute, no builder exists for it.
- **Visibility limits** (§8.3): `private`/`protected` types, constructors, and members cannot be
  built; `internal` only via `[InternalsVisibleTo]` to the test assembly.
- **Removed extensibility pipeline** (§8): custom `IExecuteStrategy`, `IBuildProcessor`,
  `IBuildAction`, `IBuildCapability`, and the `IConstructorResolver`/`IParameterResolver`/
  `IPropertyResolver`/`ITypeResolver` resolvers are gone with no equivalent. Code that swapped these
  out cannot be ported one-to-one and must move to the supported seams (`IValueSource<T>`, mappings,
  ignore rules, `[GenerateModelBuilder]`).
- **`DefaultTypeResolver` automatic interface/abstract scanning** (§7) is removed: abstract and
  interface targets need an explicit `Mapping<,>`; there is no assembly scan that guesses a concrete
  type.
- **Per-`ParameterInfo`/`PropertyInfo` generator overloads and `CacheLevel`** (§8) no longer exist.

Each bullet links the affected v8 surface to the design section that explains why it was removed and
what to use instead, so "not supported" always comes with the supported alternative or an explicit
"stay on v8" pointer.

---

## 15. Diagnostics and error reporting (a first-class deliverable)

A source-generated, compile-time-first library can move most failures from "mysterious runtime
exception" to "precise build error at the offending line". vNext treats that as a primary feature,
not an afterthought: a developer should rarely need a debugger to understand why a model didn't
build the way they expected. Diagnostics come in two layers — **compile-time** (Roslyn analyzer +
generator diagnostics, surfaced in the IDE and build output) and **runtime** (the structured
`ModelBuildException` and build log from §9.2–§9.3) — and both are held to the same quality bar.

### 15.1 Quality bar for every diagnostic

Every diagnostic (compile-time or runtime) must provide:

- **A stable ID** in a reserved prefix (`MB####`) so it can be searched, suppressed, or referenced
  in docs and PRs. IDs never get reused for a different meaning.
- **A one-line summary** that names the type and member involved.
- **The cause**, in plain language, not just a symptom.
- **A concrete fix** — the specific call to add (`Mapping<Stream, MemoryStream>()`), attribute to
  apply (`[InternalsVisibleTo]`, `[GenerateModelBuilder]`), or rule to register — phrased as an
  imperative the reader can act on.
- **A help link** (`helpLinkUri`) to the matching README/design section, so the IDE lightbulb and
  the build log both lead somewhere useful.
- **A precise location.** Compile-time diagnostics point at the exact `Model.Create<T>()` call,
  `typeof(X)` argument, member, or registration that caused them. Runtime exceptions carry the build
  path (§9.3) and, where available, the source location of the rooting call (§9.2).

### 15.2 Compile-time diagnostics (catalogue)

Reported by the analyzer/generator so they appear as you type and break the build before any test
runs. Indicative catalogue (IDs illustrative, severities tunable via `.editorconfig`):

| ID | Severity | Raised when | Suggested fix in the message |
|----|----------|-------------|------------------------------|
| `MB1001` | Error | A discovered build target is an abstract/interface type with no `Mapping<,>` and no value source | "Add `Model.Mapping<{abstract}, {concrete}>()` or register an `IValueSource<{abstract}>`." |
| `MB1002` | Error | A required constructor/member is inaccessible (`private`/`protected`, or `internal` without `[InternalsVisibleTo]`) | "Make `{member}` accessible to the test assembly, or add `[InternalsVisibleTo]`." |
| `MB1003` | Error | A value-ordering cycle (`.Before(...)`) has no valid topological order (§8.2.6) | "Remove or invert one `.Before(...)` in the cycle `{a → b → … → a}`." |
| `MB1004` | Error | `.ForMembersMatching(expr)` uses a lambda shape the generator can't interpret (§8.2.8) | "Use a supported expression (`EndsWith`/`StartsWith`/`Contains`/`==`/`&&`/`||`/`!`) or switch to `.When(...)` for runtime evaluation." |
| `MB1005` | Warning | `Model.Create(typeof(X))` names a constant type that can't be built (unbuildable/inaccessible/unmapped) (§12.7) | "`{X}` has no generated builder — make it discoverable (`Create<{X}>()`, a mapping, or `[GenerateModelBuilder]`)." |
| `MB1006` | Warning | A `[GenerateModelBuilder]` attribute is declared on a **production** type rather than in the test assembly (§6.1) | "Move to `[assembly: GenerateModelBuilder(typeof({X}))]` in the test project to avoid shipping the attribute." |
| `MB1007` | Warning | Two sources match the same target with equal priority (ambiguous selection) | "Give one source a higher priority, or narrow its `.ForMembers...` constraint." |
| `MB1008` | Warning | A registered `IValueSource<T>` is never selected (dead registration) | "Remove the registration or widen its match; it currently matches no discovered member." |
| `MB1009` | Info | A member falls back to runtime matching via `.When(...)` on a hot path | "Express the condition with `.ForMembersNamed/Matching(...)` to compile it in, if possible." |
| `MB1010` | Error | A write-only or init-only member is targeted in a way that can't be satisfied (§7, #282) | "Remove the member from population, or provide it via constructor args." |

The generator also emits the **cycle** and **ambiguity** errors against the *merged* configuration
for the whole compilation, so a conflict introduced by combining two modules is caught (§8.2.6).

### 15.3 Runtime diagnostics (when a build can only fail at run time)

Conditions that can't be proven at compile time — a custom `IValueSource<T>.Create` throwing, a
runtime depth/circular limit, a `.When(...)` predicate that excludes every candidate — surface as
the structured `ModelBuildException` from §9.3, carrying `BuildPath`, `FailureKind`, the captured
`BuildLog`, and a chained `InnerException`. The same quality bar (§15.1) applies: the message states
the cause and the fix, and the `FailureKind` enum lets tests assert on the category. Runtime
failures that have a compile-time analogue (e.g. an unmapped abstract only reached through a
`.When(...)` runtime branch) reuse the *same wording and help link* as their `MB####` compile-time
counterpart, so a developer learns one explanation regardless of when it fires.

### 15.4 The build log as a debugging aid

For "it built, but not how I expected" (a property left default, the wrong source chosen), the
opt-in build log (§9.2) is the primary tool: a nested, indented trace that records, per member, the
chosen source/builder, the constructor picked, the rules that matched, and the relative values
consulted — including **why a member was skipped** (ignore rule, circular guard, no setter). It is
structured so tests can assert on it and human-readable so it can be pasted into a bug report. This
closes the loop with #349 (the log survives exceptions) and makes the common "why didn't my property
get set?" question answerable without a debugger.

### 15.5 Determinism and reproducibility in reports

Because builds are seedable (`IRandomSource`, §9.1), every diagnostic and build log can include the
**seed** in play, so a failing test can be reproduced exactly. Runtime exception messages and the
build log surface the seed alongside the build path, turning an intermittent "it sometimes generates
a bad value" into a deterministic repro.

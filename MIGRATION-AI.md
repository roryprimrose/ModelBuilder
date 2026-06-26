# ModelBuilder v8 → v9 — AI agent migration playbook

This file is written for an AI/agent performing the upgrade of a **consuming** project from
ModelBuilder v8 to v9. It is deliberately procedural. The human guide is [MIGRATION.md](MIGRATION.md);
the deterministic data is [migration-map.json](migration-map.json). Both ship inside the NuGet
package alongside this file.

## The one mental model to load first

v9 deleted v8's **runtime reflection build pipeline** and replaced it with **compile-time
source-generated builders**. Consequences that shape the whole migration:

- Every removed v8 type still exists as a **throwing `[Obsolete(error: true)]` stub**, so any v8 usage
  is a **compile error**, not a runtime failure.
- Unbuildable types are reported as **build-time diagnostics** (`MB1001`/`MB1002`/`MB1005`), never as
  runtime surprises.
- Therefore **migration is compile-error driven**: the compiler hands you an exact, enumerable
  worklist. You do not need to guess what to change — build, read the errors, fix each, repeat.

Do **not** try to recreate v8 extensibility (`IExecuteStrategy`, resolvers, `IBuildAction`, …). It is
gone by design. Move to the four supported seams: `IValueSource<T>`, `Model.Mapping<,>`, ignore rules,
and `[GenerateModelBuilder]`.

## Procedure

1. **Bump the package** to the v9 major version.
2. **Build** (`dotnet build -c Release`). Treat the resulting errors and `MB####` diagnostics as the
   worklist.
3. **For each error**, find the symbol in [migration-map.json](migration-map.json):
   - It's in `unchanged` → no change needed.
   - It's in `mappings` → apply the `v9` replacement. Entries with `"automatable": true` are safe
     mechanical rewrites (e.g. `Model.Create<T>(args)` → `Model.Construct<T>().From(args)`); the rest
     need a small semantic edit described in `notes`.
   - It's in `removed` → there is **no drop-in**. Read `guidance` and re-express the intent with a
     supported seam. If the code genuinely needs runtime reflection / `dynamic` types, v9 is not a fit
     — surface that to the user rather than forcing it.
4. **Re-build** until clean. Resolve each `MB####` diagnostic using its `fix` in the map (most are
   "add a `Model.Mapping<,>`" or "make the type discoverable").
5. **Run the tests** (`dotnet test -c Release`). Green build + green tests is the definition of done.

## High-frequency rewrites (apply directly)

| You see (v8) | Change to (v9) |
| --- | --- |
| `Model.Create<T>(arg1, arg2)` | `Model.Construct<T>().From(arg1, arg2)` |
| `configuration.AddValueGenerator<G>()` | `configuration.AddValueSource<T>(source, "MemberName")` |
| `configuration.AddTypeCreator<C>()` | `configuration.AddValueSource<T>(source)` |
| `configuration.AddIgnoreRule<T>(x => x.M)` | `configuration.Ignore(typeof(T), nameof(T.M))` (in a module) or `Model.Ignoring<T>(x => x.M)` |
| `UpdateValueGenerator<G>(g => g.X = …)` | `Model.SetOptions(x => …)` (`MinCount`/`MaxCount`/`NullPercentage`/`MaxDepth`) |
| custom `IValueGenerator` / `ITypeCreator` class | a class implementing `IValueSource<T>` (single `Create(IBuildContext, in BuildTarget)`), or a `DelegateValueSource<T>` |
| `new DefaultConfigurationModule().Configure(c)` inside a module | delete it — defaults are implicit |

## Things that block the build with no auto-fix

These have **no equivalent**; re-express the intent, or report to the user that v9 may not fit:

- `IExecuteStrategy`, `IBuildProcessor`, `IBuildAction`, `IBuildCapability`
- `IConstructorResolver`, `IParameterResolver`, `IPropertyResolver`, `ITypeResolver`
- `CacheLevel`, per-`ParameterInfo`/`PropertyInfo` overloads
- automatic interface/abstract resolution (now requires an explicit `Model.Mapping<,>`)

## Reference

- Canonical v9 usage for every scenario: the runnable
  [`ModelBuilder.Examples`](ModelBuilder.Examples/ModelBuilder.Examples.csproj) project, also bundled
  in the package. Pattern-match against it.
- Full prose and worked module example: [MIGRATION.md](MIGRATION.md).
- API surface and behaviour: [README.md](README.md).

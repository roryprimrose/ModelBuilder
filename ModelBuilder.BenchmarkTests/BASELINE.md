# ModelBuilder v8 baseline benchmarks

This document records the **reflection-based v8 baseline** captured before the v9 rewrite
(see [`design.md`](../design.md) §13 step 2). The same BenchmarkDotNet suite is retained into v9
and re-run unchanged against the source-generated library so the old-vs-new delta — faster `Create`,
far fewer allocations, lower GC pressure — can be published as concrete evidence.

> [!NOTE]
> These are **default-job** numbers (BenchmarkDotNet's standard warmup/iteration counts with
> statistical stabilisation), a publication-grade baseline. Absolute timings are machine-specific,
> so the **ratios and allocation columns** are the durable signal for the old-vs-new comparison.
> A faster `--job short` run is available for quick local iteration.

## How to run

```pwsh
cd ModelBuilder.BenchmarkTests
# Full suite across every supported runtime (default job — the committed baseline):
dotnet run -c Release -f net8.0 -- --filter * --runtimes net472 net8.0 net9.0 net10.0
# Faster, less stable local iteration (short job):
dotnet run -c Release -f net8.0 -- --filter * --runtimes net472 net8.0 net9.0 net10.0 --job short
```

Generated reports land under `ModelBuilder.BenchmarkTests/BenchmarkDotNet.Artifacts/` (git-ignored).

## Method and model shapes

The suite measures the two public entry points across a spread of graph sizes/complexities so the
numbers show how cost scales with the object graph, not just a single type:

| Model | Shape |
| --- | --- |
| `FlatPoco` | Flat POCO, primitive properties only |
| `SmallNested` | Root with a couple of nested references, one level deep |
| `DeepWideGraph` | Three levels deep, three children per node |
| `WithCollections` | Arrays, lists and dictionaries of nested items |
| `WithEnumsNullable` | Enums and nullable value types |
| `CtorArgMatching` | Constructor parameters matched to property names |

- `CreateBenchmarks` — `Model.Create<T>()` (construct + populate a fresh graph).
- `PopulateBenchmarks` — `Model.Populate(instance)` (fill a pre-allocated graph), isolating
  value-generation/member-walking cost from construction.

`[MemoryDiagnoser]` captures allocated bytes and Gen0/Gen1 collections. The `.NET Framework 4.7.2`
job is the baseline (`Ratio` = 1.00) because it exercises the `netstandard2.0` consumption path;
the modern TFMs cover the AOT-capable runtimes.

## Environment

```text
BenchmarkDotNet v0.15.2, Windows 11 (10.0.26200.8655)
.NET SDK 10.0.301
  [Host]    : .NET 8.0.28, X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Default job: BenchmarkDotNet standard warmup/iteration counts
Runtimes  : .NET Framework 4.7.2 (4.8.1 runtime), .NET 8.0.28, .NET 9.0.16, .NET 10.0.9
```

## Results — `Model.Create<T>()`

| Method | Runtime | Mean | Ratio | Gen0 | Gen1 | Allocated | Alloc Ratio |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: |
| FlatPocoCreate | .NET 10.0 | 20.22 μs | 0.40 | 4.2725 | 0.2441 | 71.22 KB | 0.81 |
| SmallNestedCreate | .NET 10.0 | 65.83 μs | 1.32 | 6.8359 | 0.2441 | 113.67 KB | 1.29 |
| DeepWideGraphCreate | .NET 10.0 | 155.98 μs | 3.12 | 30.2734 | 1.9531 | 498.29 KB | 5.66 |
| WithCollectionsCreate | .NET 10.0 | 171.55 μs | 3.43 | 29.2969 | 1.9531 | 480.35 KB | 5.45 |
| WithEnumsNullableCreate | .NET 10.0 | 20.76 μs | 0.42 | 3.6621 | 0.1221 | 61.62 KB | 0.70 |
| CtorArgMatchingCreate | .NET 10.0 | 13.23 μs | 0.26 | 2.6855 | 0.1221 | 44.78 KB | 0.51 |
| FlatPocoCreate | .NET 8.0 | 27.03 μs | 0.54 | 4.8828 | 0.2441 | 80.91 KB | 0.92 |
| SmallNestedCreate | .NET 8.0 | 78.84 μs | 1.58 | 7.8125 | 0.4883 | 134.94 KB | 1.53 |
| DeepWideGraphCreate | .NET 8.0 | 211.34 μs | 4.23 | 37.1094 | 1.9531 | 621.23 KB | 7.05 |
| WithCollectionsCreate | .NET 8.0 | 227.53 μs | 4.55 | 35.1563 | 2.9297 | 590.04 KB | 6.70 |
| WithEnumsNullableCreate | .NET 8.0 | 24.99 μs | 0.50 | 4.2725 | 0.1221 | 70.91 KB | 0.81 |
| CtorArgMatchingCreate | .NET 8.0 | 17.67 μs | 0.35 | 3.0518 | 0.1221 | 49.88 KB | 0.57 |
| FlatPocoCreate | .NET 9.0 | 21.53 μs | 0.43 | 4.7607 | 0.2441 | 77.98 KB | 0.89 |
| SmallNestedCreate | .NET 9.0 | 62.58 μs | 1.25 | 7.5684 | 0.4883 | 124.07 KB | 1.41 |
| DeepWideGraphCreate | .NET 9.0 | 161.73 μs | 3.24 | 36.1328 | 1.9531 | 595.83 KB | 6.77 |
| WithCollectionsCreate | .NET 9.0 | 178.84 μs | 3.58 | 33.2031 | 2.9297 | 557.33 KB | 6.33 |
| WithEnumsNullableCreate | .NET 9.0 | 21.09 μs | 0.42 | 4.1504 | 0.1221 | 68.13 KB | 0.77 |
| CtorArgMatchingCreate | .NET 9.0 | 14.23 μs | 0.28 | 2.9297 | 0.1221 | 48.54 KB | 0.55 |
| FlatPocoCreate | .NET Framework 4.7.2 | 50.05 μs | 1.00 | 14.2822 | 0.7935 | 88.06 KB | 1.00 |
| SmallNestedCreate | .NET Framework 4.7.2 | 141.92 μs | 2.84 | 23.1934 | 1.2207 | 143.85 KB | 1.63 |
| DeepWideGraphCreate | .NET Framework 4.7.2 | 373.98 μs | 7.48 | 110.8398 | 7.8125 | 683.61 KB | 7.76 |
| WithCollectionsCreate | .NET Framework 4.7.2 | 409.62 μs | 8.19 | 103.5156 | 8.7891 | 638.13 KB | 7.25 |
| WithEnumsNullableCreate | .NET Framework 4.7.2 | 51.88 μs | 1.04 | 12.8174 | 0.6104 | 79.11 KB | 0.90 |
| CtorArgMatchingCreate | .NET Framework 4.7.2 | 33.44 μs | 0.67 | 8.8501 | 0.4883 | 54.40 KB | 0.62 |

## Results — `Model.Populate(instance)`

| Method | Runtime | Mean | Ratio | Gen0 | Gen1 | Allocated | Alloc Ratio |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: |
| FlatPocoPopulate | .NET 10.0 | 19.95 μs | 0.40 | 4.2725 | 0.2441 | 70.49 KB | 0.81 |
| SmallNestedPopulate | .NET 10.0 | 66.10 μs | 1.33 | 6.8359 | 0.4883 | 112.95 KB | 1.31 |
| WithEnumsNullablePopulate | .NET 10.0 | 20.18 μs | 0.41 | 3.6621 | 0.1221 | 60.89 KB | 0.70 |
| FlatPocoPopulate | .NET 8.0 | 25.59 μs | 0.52 | 4.7607 | 0.2441 | 79.61 KB | 0.92 |
| SmallNestedPopulate | .NET 8.0 | 72.04 μs | 1.45 | 7.8125 | 0.4883 | 133.64 KB | 1.54 |
| WithEnumsNullablePopulate | .NET 8.0 | 23.55 μs | 0.48 | 4.1504 | 0.1221 | 69.61 KB | 0.80 |
| FlatPocoPopulate | .NET 9.0 | 20.94 μs | 0.42 | 4.6387 | 0.2441 | 76.68 KB | 0.89 |
| SmallNestedPopulate | .NET 9.0 | 68.18 μs | 1.38 | 7.3242 | 0.2441 | 122.80 KB | 1.42 |
| WithEnumsNullablePopulate | .NET 9.0 | 20.60 μs | 0.42 | 4.0283 | 0.1221 | 66.83 KB | 0.77 |
| FlatPocoPopulate | .NET Framework 4.7.2 | 49.59 μs | 1.00 | 14.0381 | 0.7324 | 86.53 KB | 1.00 |
| SmallNestedPopulate | .NET Framework 4.7.2 | 139.06 μs | 2.81 | 22.9492 | 1.2207 | 142.29 KB | 1.64 |
| WithEnumsNullablePopulate | .NET Framework 4.7.2 | 47.44 μs | 0.96 | 12.5732 | 0.6104 | 77.58 KB | 0.90 |

## Baseline observations (what v9 must beat)

- **Allocations dominate and scale super-linearly with graph size.** A flat POCO allocates ~71–88 KB
  to build; the deep/wide and collection graphs allocate **~480–684 KB** — 5–8× the flat case. This
  is the reflection + boxing + per-build configuration overhead the source-generated build is
  designed to remove.
- **GC pressure is heavy on the `netstandard2.0`/net472 path**: Gen0 ~100+ and non-trivial Gen1 on
  the deep/collection graphs. The modern runtimes already roughly halve Gen0 for the same work.
- **`Create` and `Populate` cost are comparable** for equivalent shapes, confirming the dominant cost
  is value generation and member walking (reflection), not instance construction.
- **Constructor-arg matching is not a hotspot** relative to property population at the same graph
  size — useful to know before optimising the generator's constructor selection.

These numbers are the reflection-based "before". The v9 target is materially lower `Mean`,
**substantially lower Allocated/Alloc Ratio**, and fewer Gen0/Gen1 collections across every shape,
with the largest wins on the deep/wide and collection-heavy graphs.

## v9 delta (source-generated vs reflection)

`CreateComparisonBenchmarks` runs the v8 reflection engine and the v9 source-generated engine on
the same shapes side by side (the v8 method is the baseline).

> [!IMPORTANT]
> **This is an accumulating record — retain every run.** The v8-vs-v9 comparison below (Run 1) is a
> **frozen historical snapshot**: the v8 reflection engine and its `CreateComparisonBenchmarks` harness
> were removed in the cutover, so that exact side-by-side can never be regenerated. Keep it as-is.
> Ongoing tracking is **v9-only** from the live `CreateBenchmarks`/`PopulateBenchmarks` suite — when
> you capture a new v9 run, **append** it as a new dated run below and leave both the v8 baseline
> tables further up this document and every earlier run in place. Never overwrite or delete old data;
> the point is to keep the full history visible so drift between runs stays trackable.

### Run 1 — initial v8-vs-v9 comparison (short job, .NET 8.0) — frozen

First and final side-by-side capture, taken when the comparison benchmark landed (design.md §10, step
10) while the v8 engine still existed. The v8 engine has since been removed, so this snapshot cannot be
re-run; it is retained here permanently as the "before/after" evidence.

| Shape | Engine | Mean | Ratio | Gen0 | Gen1 | Allocated | Alloc Ratio |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: |
| FlatPoco | v8 | 32.22 μs | 1.00 | 4.8828 | 0.2441 | 80.91 KB | 1.00 |
| FlatPoco | **v9** | **14.44 μs** | **0.45** | 0.2747 | - | **4.50 KB** | **0.06** |
| SmallNested | v8 | 98.83 μs | 1.00 | 7.8125 | 0.4883 | 134.96 KB | 1.00 |
| SmallNested | **v9** | **7.19 μs** | **0.07** | 0.2670 | - | **4.45 KB** | **0.03** |
| WithCollections | v8 | 285.32 μs | 1.00 | 35.1563 | 1.9531 | 589.34 KB | 1.00 |
| WithCollections | **v9** | **19.52 μs** | **0.07** | 0.7324 | - | **11.99 KB** | **0.02** |
| WithEnumsNullable | v8 | 34.92 μs | 1.00 | 4.2725 | 0.1221 | 70.91 KB | 1.00 |
| WithEnumsNullable | **v9** | **2.99 μs** | **0.09** | 0.1488 | - | **2.47 KB** | **0.03** |

> Ratios are v9 relative to the v8 baseline **for the same shape** (the table above uses each v8
> method as its own baseline, so the v8 rows read 1.00).

**The headline result the rewrite set out to demonstrate (Run 1):**

- **Faster everywhere** — v9 is 2.2× faster on the flat POCO and 11–14× faster on the nested,
  collection and enum/nullable shapes, because there is no per-build reflection, constructor
  resolution, or `ExpandoObject`.
- **Allocations collapse** — v9 allocates **6–50× less** (e.g. `WithCollections` 589 KB → 12 KB,
  `SmallNested` 135 KB → 4.5 KB). The reflection metadata, boxing and per-call configuration objects
  are simply gone.
- **No Gen1 pressure** — v9 records **zero Gen1 collections** across every shape, versus
  non-trivial Gen1 on the v8 path; Gen0 drops by an order of magnitude too.

<!--
### Run N — <date>, <job>, <runtime> (v9-only)

Append the next v9 run here, ABOVE the older runs (most-recent-first), leaving every prior run and
the frozen Run 1 in place. Capture it from the live suite:

    dotnet run -c Release -f net8.0 -- --filter *Create* *Populate* --runtimes net8.0 --job short

Copy the table + observations format and note the date, job and runtime so runs stay comparable.
-->


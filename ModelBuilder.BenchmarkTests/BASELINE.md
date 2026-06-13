# ModelBuilder v8 baseline benchmarks

This document records the **reflection-based v8 baseline** captured before the vNext rewrite
(see [`design.md`](../design.md) §13 step 2). The same BenchmarkDotNet suite is retained into vNext
and re-run unchanged against the source-generated library so the old-vs-new delta — faster `Create`,
far fewer allocations, lower GC pressure — can be published as concrete evidence.

> [!NOTE]
> These are **short-job** numbers (3 warmup, 3 measured iterations, 1 launch) intended as an
> indicative, reproducible baseline rather than a publication-grade measurement. Re-run with the
> default job (`--job default`) on a quiet machine before quoting absolute figures externally. The
> absolute timings are also machine-specific; the **ratios and allocation columns** are the durable
> signal.

## How to run

```pwsh
cd ModelBuilder.BenchmarkTests
# Full suite across every supported runtime (short job):
dotnet run -c Release -f net8.0 -- --filter * --runtimes net472 net8.0 net9.0 net10.0 --job short
# Single runtime, default (longer, more stable) job:
dotnet run -c Release -f net8.0 -- --filter *CreateBenchmarks* --runtimes net8.0
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
  [Host]   : .NET 8.0.28, X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  ShortRun : IterationCount=3  LaunchCount=1  WarmupCount=3
Runtimes : .NET Framework 4.7.2 (4.8.1 runtime), .NET 8.0.28, .NET 9.0.16, .NET 10.0.9
```

## Results — `Model.Create<T>()`

| Method | Runtime | Mean | Ratio | Gen0 | Gen1 | Allocated | Alloc Ratio |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: |
| FlatPocoCreate | .NET 10.0 | 19.26 μs | 0.31 | 4.2725 | 0.2441 | 71.22 KB | 0.81 |
| SmallNestedCreate | .NET 10.0 | 92.16 μs | 1.47 | 6.8359 | - | 113.70 KB | 1.29 |
| DeepWideGraphCreate | .NET 10.0 | 155.51 μs | 2.48 | 30.2734 | 1.9531 | 504.16 KB | 5.73 |
| WithCollectionsCreate | .NET 10.0 | 214.88 μs | 3.43 | 29.2969 | 1.9531 | 488.29 KB | 5.54 |
| WithEnumsNullableCreate | .NET 10.0 | 25.13 μs | 0.40 | 3.6621 | 0.1221 | 61.62 KB | 0.70 |
| CtorArgMatchingCreate | .NET 10.0 | 19.15 μs | 0.31 | 2.6855 | 0.1221 | 44.78 KB | 0.51 |
| FlatPocoCreate | .NET 8.0 | 32.46 μs | 0.52 | 4.8828 | 0.2441 | 80.91 KB | 0.92 |
| SmallNestedCreate | .NET 8.0 | 92.97 μs | 1.48 | 7.8125 | 0.4883 | 134.91 KB | 1.53 |
| DeepWideGraphCreate | .NET 8.0 | 264.69 μs | 4.22 | 37.1094 | 1.9531 | 621.24 KB | 7.05 |
| WithCollectionsCreate | .NET 8.0 | 295.56 μs | 4.72 | 35.1563 | 1.9531 | 585.26 KB | 6.65 |
| WithEnumsNullableCreate | .NET 8.0 | 31.22 μs | 0.50 | 4.2725 | 0.1221 | 70.91 KB | 0.81 |
| CtorArgMatchingCreate | .NET 8.0 | 21.01 μs | 0.34 | 3.0518 | 0.1221 | 49.89 KB | 0.57 |
| FlatPocoCreate | .NET 9.0 | 27.28 μs | 0.44 | 4.6387 | 0.2441 | 77.98 KB | 0.89 |
| SmallNestedCreate | .NET 9.0 | 81.38 μs | 1.30 | 7.3242 | - | 124.03 KB | 1.41 |
| DeepWideGraphCreate | .NET 9.0 | 220.46 μs | 3.52 | 35.1563 | 1.9531 | 595.83 KB | 6.77 |
| WithCollectionsCreate | .NET 9.0 | 257.68 μs | 4.11 | 33.2031 | 1.9531 | 571.17 KB | 6.49 |
| WithEnumsNullableCreate | .NET 9.0 | 26.24 μs | 0.42 | 4.1504 | 0.1221 | 68.13 KB | 0.77 |
| CtorArgMatchingCreate | .NET 9.0 | 18.68 μs | 0.30 | 2.9297 | 0.1221 | 48.54 KB | 0.55 |
| FlatPocoCreate | .NET Framework 4.7.2 | 62.67 μs | 1.00 | 14.2822 | 0.7324 | 88.06 KB | 1.00 |
| SmallNestedCreate | .NET Framework 4.7.2 | 153.36 μs | 2.45 | 23.1934 | 1.2207 | 143.91 KB | 1.63 |
| DeepWideGraphCreate | .NET Framework 4.7.2 | 383.09 μs | 6.11 | 110.8398 | 7.8125 | 683.61 KB | 7.76 |
| WithCollectionsCreate | .NET Framework 4.7.2 | 395.24 μs | 6.31 | 103.5156 | 8.7891 | 639.43 KB | 7.26 |
| WithEnumsNullableCreate | .NET Framework 4.7.2 | 52.08 μs | 0.83 | 12.8174 | 0.6104 | 79.11 KB | 0.90 |
| CtorArgMatchingCreate | .NET Framework 4.7.2 | 33.18 μs | 0.53 | 8.8501 | 0.4883 | 54.40 KB | 0.62 |

## Results — `Model.Populate(instance)`

| Method | Runtime | Mean | Ratio | Gen0 | Gen1 | Allocated | Alloc Ratio |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: |
| FlatPocoPopulate | .NET 10.0 | 28.79 μs | 0.45 | 4.2725 | 0.2441 | 71.11 KB | 0.82 |
| SmallNestedPopulate | .NET 10.0 | 77.85 μs | 1.21 | 6.8359 | 0.4883 | 112.97 KB | 1.31 |
| WithEnumsNullablePopulate | .NET 10.0 | 23.43 μs | 0.37 | 3.6621 | 0.1221 | 60.89 KB | 0.70 |
| FlatPocoPopulate | .NET 8.0 | 25.12 μs | 0.39 | 4.7607 | 0.2441 | 79.61 KB | 0.92 |
| SmallNestedPopulate | .NET 8.0 | 80.66 μs | 1.26 | 7.8125 | 0.4883 | 133.46 KB | 1.54 |
| WithEnumsNullablePopulate | .NET 8.0 | 24.13 μs | 0.38 | 4.1504 | 0.1221 | 69.61 KB | 0.80 |
| FlatPocoPopulate | .NET 9.0 | 20.74 μs | 0.32 | 4.6387 | 0.2441 | 76.68 KB | 0.89 |
| SmallNestedPopulate | .NET 9.0 | 73.64 μs | 1.15 | 7.3242 | 0.4883 | 122.85 KB | 1.42 |
| WithEnumsNullablePopulate | .NET 9.0 | 26.69 μs | 0.42 | 4.0283 | 0.1221 | 66.83 KB | 0.77 |
| FlatPocoPopulate | .NET Framework 4.7.2 | 64.14 μs | 1.00 | 14.0381 | 0.7324 | 86.53 KB | 1.00 |
| SmallNestedPopulate | .NET Framework 4.7.2 | 184.92 μs | 2.88 | 22.9492 | 1.2207 | 142.34 KB | 1.64 |
| WithEnumsNullablePopulate | .NET Framework 4.7.2 | 51.48 μs | 0.80 | 12.5732 | 0.6104 | 77.58 KB | 0.90 |

## Baseline observations (what vNext must beat)

- **Allocations dominate and scale super-linearly with graph size.** A flat POCO allocates ~71–88 KB
  to build; the deep/wide and collection graphs allocate **~490–680 KB** — 5–8× the flat case. This
  is the reflection + boxing + per-build configuration overhead the source-generated build is
  designed to remove.
- **GC pressure is heavy on the `netstandard2.0`/net472 path**: Gen0 ~100+ and non-trivial Gen1 on
  the deep/collection graphs. The modern runtimes already roughly halve Gen0 for the same work.
- **`Create` and `Populate` cost are comparable** for equivalent shapes, confirming the dominant cost
  is value generation and member walking (reflection), not instance construction.
- **Constructor-arg matching is not a hotspot** relative to property population at the same graph
  size — useful to know before optimising the generator's constructor selection.

These numbers are the reflection-based "before". The vNext target is materially lower `Mean`,
**substantially lower Allocated/Alloc Ratio**, and fewer Gen0/Gen1 collections across every shape,
with the largest wins on the deep/wide and collection-heavy graphs.

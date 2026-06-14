namespace ModelBuilder.BenchmarkTests
{
    using BenchmarkDotNet.Attributes;
    using ModelBuilder.BenchmarkTests.Models;
    using V8 = ModelBuilder.Model;
    using VNext = ModelBuilder.vNext.Model;

    /// <summary>
    /// Compares the v8 reflection engine against the vNext source-generated engine for
    /// <c>Create&lt;T&gt;()</c> on the same model shapes, so the old-vs-new delta (time, allocations,
    /// GC) is visible side by side. The v8 method for each shape is the baseline.
    /// </summary>
    [MemoryDiagnoser]
    public class CreateComparisonBenchmarks
    {
        [Benchmark(Baseline = true)]
        [BenchmarkCategory("FlatPoco")]
        public FlatPoco FlatPocoV8()
        {
            return V8.Create<FlatPoco>();
        }

        [Benchmark]
        [BenchmarkCategory("FlatPoco")]
        public FlatPoco FlatPocoVNext()
        {
            return VNext.Create<FlatPoco>();
        }

        [Benchmark]
        [BenchmarkCategory("SmallNested")]
        public SmallNested SmallNestedV8()
        {
            return V8.Create<SmallNested>();
        }

        [Benchmark]
        [BenchmarkCategory("SmallNested")]
        public SmallNested SmallNestedVNext()
        {
            return VNext.Create<SmallNested>();
        }

        [Benchmark]
        [BenchmarkCategory("WithCollections")]
        public WithCollections WithCollectionsV8()
        {
            return V8.Create<WithCollections>();
        }

        [Benchmark]
        [BenchmarkCategory("WithCollections")]
        public WithCollections WithCollectionsVNext()
        {
            return VNext.Create<WithCollections>();
        }

        [Benchmark]
        [BenchmarkCategory("WithEnumsNullable")]
        public WithEnumsNullable WithEnumsNullableV8()
        {
            return V8.Create<WithEnumsNullable>();
        }

        [Benchmark]
        [BenchmarkCategory("WithEnumsNullable")]
        public WithEnumsNullable WithEnumsNullableVNext()
        {
            return VNext.Create<WithEnumsNullable>();
        }
    }
}

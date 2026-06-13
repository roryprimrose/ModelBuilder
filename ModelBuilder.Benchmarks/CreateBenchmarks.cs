namespace ModelBuilder.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using ModelBuilder.Benchmarks.Models;

    /// <summary>
    /// Measures <c>Model.Create&lt;T&gt;()</c> across model shapes of increasing size and
    /// complexity. Captures execution time, allocated bytes and GC counts so the cost of the
    /// reflection-based v8 build can be compared against the source-generated vNext build.
    /// </summary>
    [MemoryDiagnoser]
    public class CreateBenchmarks
    {
        [Benchmark(Baseline = true)]
        public FlatPoco FlatPocoCreate()
        {
            return Model.Create<FlatPoco>();
        }

        [Benchmark]
        public SmallNested SmallNestedCreate()
        {
            return Model.Create<SmallNested>();
        }

        [Benchmark]
        public DeepWideGraph DeepWideGraphCreate()
        {
            return Model.Create<DeepWideGraph>();
        }

        [Benchmark]
        public WithCollections WithCollectionsCreate()
        {
            return Model.Create<WithCollections>();
        }

        [Benchmark]
        public WithEnumsNullable WithEnumsNullableCreate()
        {
            return Model.Create<WithEnumsNullable>();
        }

        [Benchmark]
        public CtorArgMatching CtorArgMatchingCreate()
        {
            return Model.Create<CtorArgMatching>();
        }
    }
}

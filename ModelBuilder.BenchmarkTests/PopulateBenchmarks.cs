namespace ModelBuilder.BenchmarkTests
{
    using BenchmarkDotNet.Attributes;
    using ModelBuilder.BenchmarkTests.Models;

    /// <summary>
    /// Measures <c>Model.Populate(instance)</c> on pre-allocated instances across model shapes.
    /// Populate fills an existing object graph, isolating the value-generation and member-walking
    /// cost from instance construction.
    /// </summary>
    [MemoryDiagnoser]
    public class PopulateBenchmarks
    {
        [Benchmark(Baseline = true)]
        public FlatPoco FlatPocoPopulate()
        {
            return Model.Populate(new FlatPoco());
        }

        [Benchmark]
        public SmallNested SmallNestedPopulate()
        {
            return Model.Populate(new SmallNested());
        }

        [Benchmark]
        public WithEnumsNullable WithEnumsNullablePopulate()
        {
            return Model.Populate(new WithEnumsNullable());
        }
    }
}

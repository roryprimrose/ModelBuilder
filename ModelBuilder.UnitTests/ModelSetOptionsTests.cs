namespace ModelBuilder.UnitTests
{
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class ModelSetOptionsTests
    {
        [Fact]
        public void SetOptionsFlowsCollectionCountIntoTheBuild()
        {
            ModelBuilderSlot<CountProbe>.Instance = new CountProbeBuilder();

            try
            {
                var actual = Model.SetOptions(x =>
                    {
                        x.MinCount = 4;
                        x.MaxCount = 4;
                    })
                    .Create<CountProbe>();

                actual.Count.Should().Be(4);
            }
            finally
            {
                ModelBuilderSlot<CountProbe>.Instance = null;
            }
        }

        [Fact]
        public void DefaultCollectionCountIsUsedWhenOptionsAreNotSet()
        {
            ModelBuilderSlot<CountProbe>.Instance = new CountProbeBuilder();

            try
            {
                var actual = Model.Create<CountProbe>();

                actual.Count.Should().BeInRange(BuildOptions.DefaultMinCount, BuildOptions.DefaultMaxCount);
            }
            finally
            {
                ModelBuilderSlot<CountProbe>.Instance = null;
            }
        }

        private sealed class CountProbe
        {
            public int Count { get; set; }
        }

        private sealed class CountProbeBuilder : IModelBuilder<CountProbe>
        {
            public CountProbe Create(IBuildContext context)
            {
                return new CountProbe { Count = context.NextCount() };
            }

            public CountProbe Populate(IBuildContext context, CountProbe instance)
            {
                return instance;
            }
        }
    }
}

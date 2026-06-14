namespace ModelBuilder.UnitTests.vNext
{
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class TypedStaticSlotTests
    {
        [Fact]
        public void ModelBuilderSlotRoundTripsInstance()
        {
            var builder = new SampleBuilder();

            ModelBuilderSlot<Sample>.Instance = builder;

            try
            {
                ModelBuilderSlot<Sample>.Instance.Should().BeSameAs(builder);
            }
            finally
            {
                ModelBuilderSlot<Sample>.Instance = null;
            }
        }

        [Fact]
        public void ValueSourceSlotIsIndependentPerClosedType()
        {
            var intSource = new ConstantInt32Source();

            ValueSource<int>.Instance = intSource;

            try
            {
                ValueSource<int>.Instance.Should().BeSameAs(intSource);
                ValueSource<long>.Instance.Should().BeNull();
            }
            finally
            {
                ValueSource<int>.Instance = null;
            }
        }

        private sealed class ConstantInt32Source : IValueSource<int>
        {
            public int Create(IBuildContext context, in BuildTarget target)
            {
                return 1;
            }
        }

        private sealed class Sample
        {
        }

        private sealed class SampleBuilder : IModelBuilder<Sample>
        {
            public Sample Create(IBuildContext context, params object?[]? args)
            {
                return new Sample();
            }

            public Sample Populate(IBuildContext context, Sample instance, object?[]? args = null)
            {
                return instance;
            }
        }
    }
}

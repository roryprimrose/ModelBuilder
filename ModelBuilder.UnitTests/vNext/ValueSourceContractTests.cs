namespace ModelBuilder.UnitTests.vNext
{
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class ValueSourceContractTests
    {
        [Fact]
        public void GenericValueSourceProducesTypedValueFromContext()
        {
            var context = new BuildContext(new RandomSource(5));
            var target = new BuildTarget(typeof(int), "Age");
            IValueSource<int> sut = new AgeValueSource();

            var actual = sut.Create(context, target);

            actual.Should().BeInRange(18, 65);
        }

        [Fact]
        public void ModelBuilderCreatesAndPopulatesInstance()
        {
            var context = new BuildContext(new RandomSource(5));
            IModelBuilder<Widget> sut = new WidgetBuilder();

            var actual = sut.Create(context);

            actual.Should().NotBeNull();
            actual.Serial.Should().NotBe(0);
        }

        [Fact]
        public void NonGenericValueSourceReturnsBoxedValue()
        {
            var context = new BuildContext(new RandomSource(5));
            var target = new BuildTarget(typeof(int));
            IValueSource sut = new ConstantValueSource();

            var actual = sut.Create(context, target);

            actual.Should().Be(42);
        }

        private sealed class AgeValueSource : IValueSource<int>
        {
            public int Create(BuildContext context, in BuildTarget target)
            {
                return context.Random.NextInt32(18, 65);
            }
        }

        private sealed class ConstantValueSource : IValueSource
        {
            public object Create(BuildContext context, in BuildTarget target)
            {
                return 42;
            }
        }

        private sealed class Widget
        {
            public int Serial { get; set; }
        }

        private sealed class WidgetBuilder : IModelBuilder<Widget>
        {
            public Widget Create(BuildContext context, params object?[]? args)
            {
                return Populate(context, new Widget(), args);
            }

            public Widget Populate(BuildContext context, Widget instance, object?[]? args = null)
            {
                instance.Serial = context.Random.NextInt32(1, int.MaxValue);

                return instance;
            }
        }
    }
}

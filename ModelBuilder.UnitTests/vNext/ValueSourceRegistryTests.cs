namespace ModelBuilder.UnitTests.vNext
{
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class ValueSourceRegistryTests
    {
        [Fact]
        public void RegisterStoresSourceWithoutBoxingProducedValue()
        {
            var sut = new ValueSourceRegistry();
            sut.Register<int>(new ConstantInt32Source());

            sut.TryGet<int>(out var source).Should().BeTrue();

            var context = new BuildContext(new RandomSource(1));
            var actual = source!.Create(context, new BuildTarget(typeof(int)));

            actual.Should().Be(7);
        }

        [Fact]
        public void RegisteredTypesExposesKeys()
        {
            var sut = new ValueSourceRegistry();

            sut.Register<int>(new ConstantInt32Source());

            sut.RegisteredTypes.Should().Contain(typeof(int));
        }

        [Fact]
        public void TryGetReturnsFalseForUnregisteredType()
        {
            var sut = new ValueSourceRegistry();

            sut.TryGet<int>(out var source).Should().BeFalse();
            source.Should().BeNull();
        }

        private sealed class ConstantInt32Source : IValueSource<int>
        {
            public int Create(BuildContext context, in BuildTarget target)
            {
                return 7;
            }
        }
    }
}

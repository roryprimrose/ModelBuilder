namespace ModelBuilder.UnitTests.vNext
{
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using ModelBuilder.vNext;
    using Xunit;

    public class BuildContextCountTests
    {
        [Fact]
        public void NextCountReturnsValueWithinConfiguredRange()
        {
            var options = new BuildContextOptions
            {
                MinCount = 3,
                MaxCount = 7
            };
            var sut = new BuildContext(new RandomSource(1), null, options);

            var counts = Enumerable.Range(0, 200).Select(_ => sut.NextCount()).ToList();

            counts.Should().OnlyContain(count => count >= 3 && count <= 7);
        }

        [Fact]
        public void NextCountDoesNotThrowWhenMaxBelowMin()
        {
            var options = new BuildContextOptions
            {
                MinCount = 10,
                MaxCount = 2
            };
            var sut = new BuildContext(new RandomSource(1), null, options);

            var actual = sut.NextCount();

            actual.Should().Be(10);
        }

        [Fact]
        public void NextCountCoercesNegativeMinToZero()
        {
            var options = new BuildContextOptions
            {
                MinCount = -5,
                MaxCount = -1
            };
            var sut = new BuildContext(new RandomSource(1), null, options);

            var actual = sut.NextCount();

            actual.Should().Be(0);
        }
    }
}

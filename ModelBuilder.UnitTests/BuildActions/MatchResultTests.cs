namespace ModelBuilder.UnitTests.BuildActions
{
    using FluentAssertions;
    using ModelBuilder.BuildActions;
    using Xunit;

    public class MatchResultTests
    {
        [Fact]
        public void DefaultsToNoMatch()
        {
            var sut = new MatchResult();

            sut.IsMatch.Should().BeFalse();
            sut.AutoPopulate.Should().BeFalse();
            sut.RequiresActivator.Should().BeFalse();
            sut.SupportsPopulate.Should().BeFalse();
        }

        [Fact]
        public void NoMatchIsMatchReturnsFalse()
        {
            var sut = MatchResult.NoMatch;

            sut.IsMatch.Should().BeFalse();
        }

        [Fact]
        public void NoMatchReturnsCachedInstance()
        {
            var expected = MatchResult.NoMatch;

            var actual = MatchResult.NoMatch;

            actual.Should().BeSameAs(expected);
        }
    }
}
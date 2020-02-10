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

            sut.SupportsCreate.Should().BeFalse();
            sut.AutoPopulate.Should().BeFalse();
            sut.AutoDetectConstructor.Should().BeFalse();
            sut.SupportsPopulate.Should().BeFalse();
        }

        [Fact]
        public void NoMatchIsMatchReturnsFalse()
        {
            var sut = MatchResult.NoMatch;

            sut.SupportsCreate.Should().BeFalse();
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
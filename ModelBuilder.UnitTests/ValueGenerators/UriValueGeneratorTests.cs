namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.Data;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class UriValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsUri()
        {
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(Uri), null!, executeStrategy).As<Uri>();

            TestData.Domains.Any(x => actual.AbsoluteUri.Contains(x, StringComparison.OrdinalIgnoreCase)).Should()
                .BeTrue();
        }

        [Theory]
        [InlineData("Uri")]
        [InlineData("URI")]
        [InlineData("uri")]
        [InlineData("Url")]
        [InlineData("URL")]
        [InlineData("url")]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(string referenceName)
        {
            var sut = new Wrapper();

            var actual = (string)sut.RunGenerate(typeof(string), referenceName, null!);

            TestData.Domains.Any(x => actual.Contains(x, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(bool), "Uri", false)]
        [InlineData(typeof(string), "Uri", true)]
        [InlineData(typeof(string), "URI", true)]
        [InlineData(typeof(string), "uri", true)]
        [InlineData(typeof(string), "Url", true)]
        [InlineData(typeof(string), "URL", true)]
        [InlineData(typeof(string), "url", true)]
        [InlineData(typeof(Uri), null, true)]
        public void IsMatchReturnsWhetherScenarioIsValidTest(Type type, string? referenceName, bool supported)
        {
            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, referenceName!, null!);

            actual.Should().Be(supported);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullType()
        {
            var sut = new Wrapper();

            Action action = () => sut.RunIsMatch(null!, null!, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityIsHigherThanStringValueGeneratorPriority()
        {
            var sut = new Wrapper();
            var other = new StringValueGenerator();

            sut.Priority.Should().BeGreaterThan(other.Priority);
        }

        private class Wrapper : UriValueGenerator
        {
            public object RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return Generate(executeStrategy, type, referenceName)!;
            }

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(buildChain, type, referenceName);
            }
        }
    }
}
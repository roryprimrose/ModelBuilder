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
        public void GenerateReturnsUriTest()
        {
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var actual = target.RunGenerate(typeof(Uri), null, executeStrategy).As<Uri>();

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
            var target = new Wrapper();

            var actual = (string) target.RunGenerate(typeof(string), referenceName, null);

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
        public void IsMatchReturnsWhetherScenarioIsValidTest(Type type, string referenceName, bool supported)
        {
            var target = new Wrapper();

            var actual = target.RunIsMatch(type, referenceName, null);

            actual.Should().Be(supported);
        }

        [Fact]
        public void PriorityIsHigherThanStringValueGeneratorPriorityTest()
        {
            var target = new Wrapper();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        private class Wrapper : UriValueGenerator
        {
            public object RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return Generate(type, referenceName, executeStrategy);
            }

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(type, referenceName, buildChain);
            }
        }
    }
}
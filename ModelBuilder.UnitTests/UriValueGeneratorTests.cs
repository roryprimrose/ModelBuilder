namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.Data;
    using NSubstitute;
    using Xunit;

    public class UriValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsStringForUriParameterNameTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new UriValueGenerator();

            var actual = target.Generate(typeof(string), "uri", executeStrategy).As<string>();

            TestData.Domains.Any(x => actual.Contains(x, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsStringForUrlParameterNameTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new UriValueGenerator();

            var actual = target.Generate(typeof(string), "url", executeStrategy).As<string>();
            
            TestData.Domains.Any(x => actual.Contains(x, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsUriTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new UriValueGenerator();

            var actual = target.Generate(typeof(Uri), null, executeStrategy).As<Uri>();
            
            TestData.Domains.Any(x => actual.AbsoluteUri.Contains(x, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(string), (string) null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(bool), "Uri", false)]
        [InlineData(typeof(string), "Uri", true)]
        [InlineData(typeof(string), "URI", true)]
        [InlineData(typeof(string), "uri", true)]
        [InlineData(typeof(string), "Url", true)]
        [InlineData(typeof(string), "URL", true)]
        [InlineData(typeof(string), "url", true)]
        [InlineData(typeof(Uri), (string) null, true)]
        public void GenerateValidatesUnsupportedScenariosTest(Type type, string referenceName, bool supported)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new UriValueGenerator();

            Action action = () => target.Generate(type, referenceName, executeStrategy);

            if (supported)
            {
                action.Should().NotThrow();
            }
            else
            {
                action.Should().Throw<NotSupportedException>();
            }
        }

        [Theory]
        [InlineData(typeof(string), (string) null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(bool), "Uri", false)]
        [InlineData(typeof(string), "Uri", true)]
        [InlineData(typeof(string), "URI", true)]
        [InlineData(typeof(string), "uri", true)]
        [InlineData(typeof(string), "Url", true)]
        [InlineData(typeof(string), "URL", true)]
        [InlineData(typeof(string), "url", true)]
        [InlineData(typeof(Uri), (string) null, true)]
        public void IsSupportedReturnsWhetherScenarioIsValidTest(Type type, string referenceName, bool supported)
        {
            var target = new UriValueGenerator();

            var actual = target.IsSupported(type, referenceName, null);

            actual.Should().Be(supported);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var buildChain = new BuildHistory();

            buildChain.Push(Guid.NewGuid().ToString());

            var target = new UriValueGenerator();

            Action action = () => target.IsSupported(null, Guid.NewGuid().ToString(), buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityIsHigherThanStringValueGeneratorPriorityTest()
        {
            var target = new UriValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }
    }
}
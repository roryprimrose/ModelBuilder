namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Linq;
    using System.Net;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class IPAddressValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsIPAddressTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var actual = target.RunGenerate(typeof(IPAddress), null, executeStrategy);

            actual.Should().NotBeNull();
            actual.As<IPAddress>().GetAddressBytes().Any(x => x != 0).Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsStringTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var actual = target.RunGenerate(typeof(string), "IpAddress", executeStrategy);

            actual.Should().NotBeNull();
            actual.As<string>().Should().MatchRegex(@"\d+(\.\d+){3}");
        }

        [Theory]
        [InlineData(typeof(string), (string) null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(bool), "IPAddress", false)]
        [InlineData(typeof(string), "IPAddress", true)]
        [InlineData(typeof(string), "ipaddress", true)]
        [InlineData(typeof(string), "IPADDRESS", true)]
        [InlineData(typeof(IPAddress), (string) null, true)]
        public void IsMatchReturnsWhetherScenarioIsValidTest(Type type, string referenceName, bool supported)
        {
            var target = new Wrapper();

            var actual = target.RunIsMatch(type, referenceName, null);

            actual.Should().Be(supported);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullType()
        {
            var target = new Wrapper();

            Action action = () => target.RunIsMatch(null, null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityIsHigherThanStringValueGeneratorPriorityTest()
        {
            var target = new Wrapper();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        private class Wrapper : IPAddressValueGenerator
        {
            public object RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return Generate(executeStrategy, type, referenceName);
            }

            public object RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(buildChain, type, referenceName);
            }
        }
    }
}
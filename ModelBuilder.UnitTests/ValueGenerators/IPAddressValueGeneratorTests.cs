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
        public void GenerateReturnsIPAddress()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(IPAddress), null!, executeStrategy);

            actual.Should().NotBeNull();
            actual.As<IPAddress>().GetAddressBytes().Any(x => x != 0).Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsString()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(string), "IpAddress", executeStrategy);

            actual.Should().NotBeNull();
            actual.As<string>().Should().MatchRegex(@"\d+(\.\d+){3}");
        }

        [Theory]
        [InlineData(typeof(string), (string) null!, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(bool), "IPAddress", false)]
        [InlineData(typeof(string), "IPAddress", true)]
        [InlineData(typeof(string), "ipaddress", true)]
        [InlineData(typeof(string), "IPADDRESS", true)]
        [InlineData(typeof(IPAddress), (string) null!, true)]
        public void IsMatchReturnsWhetherScenarioIsValidTest(Type type, string referenceName, bool supported)
        {
            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, referenceName, null!);

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

        private class Wrapper : IPAddressValueGenerator
        {
            public object RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return Generate(executeStrategy, type, referenceName)!;
            }

            public object RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(buildChain, type, referenceName);
            }
        }
    }
}
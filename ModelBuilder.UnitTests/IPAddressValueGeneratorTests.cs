namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using System.Net;
    using FluentAssertions;
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

            var target = new IPAddressValueGenerator();

            var actual = target.Generate(typeof(IPAddress), null, executeStrategy);

            actual.Should().NotBeNull();
            actual.As<IPAddress>().GetAddressBytes().Any(x => x != 0).Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsStringTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new IPAddressValueGenerator();

            var actual = target.Generate(typeof(string), "IpAddress", executeStrategy);

            actual.Should().NotBeNull();
            actual.As<string>().Should().MatchRegex(@"\d+(\.\d+){3}");
        }

        [Theory]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(bool), "IPAddress", false)]
        [InlineData(typeof(string), "IPAddress", true)]
        [InlineData(typeof(string), "IpAddress", true)]
        [InlineData(typeof(string), "ipaddress", true)]
        [InlineData(typeof(string), "IPADDRESS", true)]
        [InlineData(typeof(IPAddress), null, true)]
        public void GenerateValidatesUnsupportedScenariosTest(Type type, string referenceName, bool supported)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new IPAddressValueGenerator();

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
        [InlineData(typeof(string), (string)null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(bool), "IPAddress", false)]
        [InlineData(typeof(string), "IPAddress", true)]
        [InlineData(typeof(string), "ipaddress", true)]
        [InlineData(typeof(string), "IPADDRESS", true)]
        [InlineData(typeof(IPAddress), (string)null, true)]
        public void IsSupportedReturnsWhetherScenarioIsValidTest(Type type, string referenceName, bool supported)
        {
            var target = new IPAddressValueGenerator();

            var actual = target.IsSupported(type, referenceName, null);

            actual.Should().Be(supported);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var buildChain = new BuildHistory();

            buildChain.Push(Guid.NewGuid().ToString());

            var target = new IPAddressValueGenerator();

            Action action = () => target.IsSupported(null, Guid.NewGuid().ToString(), buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityIsHigherThanStringValueGeneratorProprityTest()
        {
            var target = new IPAddressValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }
    }
}
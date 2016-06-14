namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using FluentAssertions;
    using Xunit;

    public class IPAddressValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsIPAddressTest()
        {
            var target = new IPAddressValueGenerator();

            var actual = target.Generate(typeof(IPAddress), null, null);

            actual.Should().NotBeNull();
            actual.As<IPAddress>().GetAddressBytes().Any(x => x != 0).Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsStringTest()
        {
            var target = new IPAddressValueGenerator();

            var actual = target.Generate(typeof(string), "IpAddress", null);

            actual.Should().NotBeNull();
            actual.As<string>().Should().MatchRegex(@"\d+(\.\d+){3}");
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullTypeTest()
        {
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(Guid.NewGuid().ToString());

            var target = new IPAddressValueGenerator();

            Action action = () => target.Generate(null, Guid.NewGuid().ToString(), buildChain);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(string), (string)null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(bool), "IPAddress", false)]
        [InlineData(typeof(string), "IPAddress", true)]
        [InlineData(typeof(string), "IpAddress", true)]
        [InlineData(typeof(string), "ipaddress", true)]
        [InlineData(typeof(string), "IPADDRESS", true)]
        [InlineData(typeof(IPAddress), (string)null, true)]
        public void GenerateValidatesUnsupportedScenariosTest(Type type, string referenceName, bool supported)
        {
            var target = new IPAddressValueGenerator();

            Action action = () => target.Generate(type, referenceName, null);

            if (supported)
            {
                action.ShouldNotThrow();
            }
            else
            {
                action.ShouldThrow<NotSupportedException>();
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
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(Guid.NewGuid().ToString());

            var target = new IPAddressValueGenerator();

            Action action = () => target.IsSupported(null, Guid.NewGuid().ToString(), buildChain);

            action.ShouldThrow<ArgumentNullException>();
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
namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Xunit;

    public class UriValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsStringForUriParameterNameTest()
        {
            var target = new UriValueGenerator();

            var actual = target.Generate(typeof(string), "uri", null);

            actual.Should().NotBeNull();
            actual.As<string>().Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GenerateReturnsStringForUrlParameterNameTest()
        {
            var target = new UriValueGenerator();

            var actual = target.Generate(typeof(string), "url", null);

            actual.Should().NotBeNull();
            actual.As<string>().Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GenerateReturnsUriTest()
        {
            var target = new UriValueGenerator();

            var actual = target.Generate(typeof(Uri), null, null);

            actual.Should().NotBeNull();
            actual.As<Uri>().ToString().Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullTypeTest()
        {
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(Guid.NewGuid().ToString());

            var target = new UriValueGenerator();

            Action action = () => target.Generate(null, Guid.NewGuid().ToString(), buildChain);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(string), (string)null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(bool), "Uri", false)]
        [InlineData(typeof(string), "Uri", true)]
        [InlineData(typeof(string), "URI", true)]
        [InlineData(typeof(string), "uri", true)]
        [InlineData(typeof(string), "Url", true)]
        [InlineData(typeof(string), "URL", true)]
        [InlineData(typeof(string), "url", true)]
        [InlineData(typeof(Uri), (string)null, true)]
        public void GenerateValidatesUnsupportedScenariosTest(Type type, string referenceName, bool supported)
        {
            var target = new UriValueGenerator();

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
        [InlineData(typeof(bool), "Uri", false)]
        [InlineData(typeof(string), "Uri", true)]
        [InlineData(typeof(string), "URI", true)]
        [InlineData(typeof(string), "uri", true)]
        [InlineData(typeof(string), "Url", true)]
        [InlineData(typeof(string), "URL", true)]
        [InlineData(typeof(string), "url", true)]
        [InlineData(typeof(Uri), (string)null, true)]
        public void IsSupportedReturnsWhetherScenarioIsValidTest(Type type, string referenceName, bool supported)
        {
            var target = new UriValueGenerator();

            var actual = target.IsSupported(type, referenceName, null);

            actual.Should().Be(supported);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(Guid.NewGuid().ToString());

            var target = new UriValueGenerator();

            Action action = () => target.IsSupported(null, Guid.NewGuid().ToString(), buildChain);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PriorityIsHigherThanStringValueGeneratorProprityTest()
        {
            var target = new UriValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }
    }
}
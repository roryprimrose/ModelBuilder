using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class DomainValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomDomainTest()
        {
            var target = new DomainValueGenerator();

            var first = target.Generate(typeof(string), "domain", null);

            first.Should().BeOfType<string>();
            first.As<string>().Should().NotBeNullOrWhiteSpace();

            var second = target.Generate(typeof(string), "domain", null);

            first.Should().NotBe(second);
        }

        [Theory]
        [InlineData(typeof(string), "domain", true)]
        [InlineData(typeof(string), "Domain", true)]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName, bool expected)
        {
            var target = new DomainValueGenerator();

            var actual = (string) target.Generate(type, referenceName, null);

            actual.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(typeof(Stream), "domain")]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(string), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var target = new DomainValueGenerator();

            Action action = () => target.Generate(type, referenceName, null);

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullTypeTest()
        {
            var target = new DomainValueGenerator();

            Action action = () => target.Generate(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void HasHigherPriorityThanStringValueGeneratorTest()
        {
            var target = new DomainValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Theory]
        [InlineData(typeof(Stream), "domain", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "domain", true)]
        [InlineData(typeof(string), "Domain", true)]
        public void IsSupportedTest(Type type, string referenceName, bool expected)
        {
            var target = new DomainValueGenerator();

            var actual = target.IsSupported(type, referenceName, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new DomainValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
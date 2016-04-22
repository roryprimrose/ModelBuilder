using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using ModelBuilder.Data;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class CountryValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomCountryTest()
        {
            var target = new CountryValueGenerator();

            var first = target.Generate(typeof(string), "country", null);

            first.Should().BeOfType<string>();
            first.As<string>().Should().NotBeNullOrWhiteSpace();

            var second = target.Generate(typeof(string), "country", null);

            first.Should().NotBe(second);
        }
        
        [Theory]
        [InlineData(typeof(string), "country", true)]
        [InlineData(typeof(string), "Country", true)]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName, bool expected)
        {
            var target = new CountryValueGenerator();

            var actual = (string) target.Generate(type, referenceName, null);

            actual.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(typeof(Stream), "country")]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(string), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var target = new CountryValueGenerator();

            Action action = () => target.Generate(type, referenceName, null);

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullTypeTest()
        {
            var target = new CountryValueGenerator();

            Action action = () => target.Generate(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void HasHigherPriorityThanStringValueGeneratorTest()
        {
            var target = new CountryValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Theory]
        [InlineData(typeof(Stream), "country", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "country", true)]
        [InlineData(typeof(string), "Country", true)]
        public void IsSupportedTest(Type type, string referenceName, bool expected)
        {
            var target = new CountryValueGenerator();

            var actual = target.IsSupported(type, referenceName, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new CountryValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class CityValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomCityTest()
        {
            var target = new CityValueGenerator();

            var first = target.Generate(typeof(string), "city", null);

            first.Should().BeOfType<string>();
            first.As<string>().Should().NotBeNullOrWhiteSpace();

            var second = target.Generate(typeof(string), "city", null);

            first.Should().NotBe(second);
        }

        [Theory]
        [InlineData(typeof(string), "city", true)]
        [InlineData(typeof(string), "City", true)]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName, bool expected)
        {
            var target = new CityValueGenerator();

            var actual = (string) target.Generate(type, referenceName, null);

            actual.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(typeof(Stream), "city")]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(string), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var target = new CityValueGenerator();

            Action action = () => target.Generate(type, referenceName, null);

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullTypeTest()
        {
            var target = new CityValueGenerator();

            Action action = () => target.Generate(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void HasHigherPriorityThanStringValueGeneratorTest()
        {
            var target = new CityValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Theory]
        [InlineData(typeof(Stream), "city", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "city", true)]
        [InlineData(typeof(string), "City", true)]
        public void IsSupportedTest(Type type, string referenceName, bool expected)
        {
            var target = new CityValueGenerator();

            var actual = target.IsSupported(type, referenceName, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new CityValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
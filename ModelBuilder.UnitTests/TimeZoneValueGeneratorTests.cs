namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using FluentAssertions;
    using Xunit;

    public class TimeZoneValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomValueMatchingCountryTest()
        {
            var source = new Address
            {
                Country = "Australia"
            };
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(source);

            var target = new TimeZoneValueGenerator();

            var actual = (string)target.Generate(typeof(string), "timezone", buildChain);

            actual.Should().StartWith("Australia/");
        }

        [Fact]
        public void GenerateReturnsRandomValueTest()
        {
            var target = new TimeZoneValueGenerator();

            var first = target.Generate(typeof(string), "timezone", null);

            first.Should().BeOfType<string>();
            first.As<string>().Should().NotBeNullOrWhiteSpace();

            var second = target.Generate(typeof(string), "timezone", null);

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsValueForTimeZoneTypeTest()
        {
            var target = new TimeZoneValueGenerator();

            var actual = target.Generate(typeof(string), "TimeZone", null);

            actual.Should().BeOfType<string>();
        }

        [Theory]
        [InlineData(typeof(string), "timezone", true)]
        [InlineData(typeof(string), "TimeZone", true)]
        [InlineData(typeof(string), "timeZone", true)]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName, bool expected)
        {
            var target = new TimeZoneValueGenerator();

            var actual = (string)target.Generate(type, referenceName, null);

            actual.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(typeof(Stream), "timezone")]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(string), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var target = new TimeZoneValueGenerator();

            Action action = () => target.Generate(type, referenceName, null);

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullTypeTest()
        {
            var target = new TimeZoneValueGenerator();

            Action action = () => target.Generate(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(Stream), "timezone", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "timezone", true)]
        [InlineData(typeof(string), "TimeZone", true)]
        [InlineData(typeof(string), "timeZone", true)]
        public void IsSupportedTest(Type type, string referenceName, bool expected)
        {
            var target = new TimeZoneValueGenerator();

            var actual = target.IsSupported(type, referenceName, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new TimeZoneValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
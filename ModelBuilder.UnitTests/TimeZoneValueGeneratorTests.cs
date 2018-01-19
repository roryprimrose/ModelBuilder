namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.Data;
    using NSubstitute;
    using Xunit;

    public class TimeZoneValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomTimeZoneMatchingCaseInsensitiveCountryTest()
        {
            var address = new Address
            {
                Country = "AUSTRALIA"
            };
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(address);

            var target = new TimeZoneValueGenerator();

            var actual = target.Generate(typeof(string), "timezone", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var valueToMatch = address.Country.ToLowerInvariant();

            TestData.TimeZones.Where(x => x.StartsWith(valueToMatch, StringComparison.OrdinalIgnoreCase)).Should()
                .Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomTimeZoneMatchingCountryTest()
        {
            var address = new Address
            {
                Country = "Australia"
            };
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(address);

            var target = new TimeZoneValueGenerator();

            var actual = target.Generate(typeof(string), "timezone", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            TestData.TimeZones.Where(x => x.StartsWith(address.Country)).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomTimeZoneMatchingCountryWhenNoCityMatchTest()
        {
            var address = new Address
            {
                City = Guid.NewGuid().ToString(),
                Country = "Australia"
            };
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(address);

            var target = new TimeZoneValueGenerator();

            var actual = target.Generate(typeof(string), "timezone", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            TestData.TimeZones.Where(x => x.StartsWith(address.Country)).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomTimeZoneWhenNoMatchingCountryTest()
        {
            var address = new Address
            {
                Country = Guid.NewGuid().ToString()
            };
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(address);

            var target = new TimeZoneValueGenerator();

            var actual = target.Generate(typeof(string), "timezone", executeStrategy) as string;

            TestData.TimeZones.Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomValueTest()
        {
            var address = new Address();
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(address);

            var target = new TimeZoneValueGenerator();

            var first = target.Generate(typeof(string), "timezone", executeStrategy);

            first.Should().BeOfType<string>();
            first.As<string>().Should().NotBeNullOrWhiteSpace();

            var second = target.Generate(typeof(string), "timezone", executeStrategy);

            first.Should().NotBe(second);
        }

        [Theory]
        [InlineData("AUSTRALIA", "CANBERRA")]
        [InlineData("Australia", "Canberra")]
        [InlineData("Other", "Canberra")] // Matches on city first then country
        [InlineData("", "Canberra")]
        [InlineData(null, "Canberra")]
        public void GenerateReturnsValueMatchingCityValuesTest(string country, string city)
        {
            var address = new Address
            {
                Country = country,
                City = city
            };
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(address);

            var target = new TimeZoneValueGenerator();

            var actual = target.Generate(typeof(string), "timezone", executeStrategy) as string;

            actual.Should().Be("Australia/Canberra");
        }

        [Theory]
        [InlineData(typeof(string), "timezone", true)]
        [InlineData(typeof(string), "TimeZone", true)]
        [InlineData(typeof(string), "timeZone", true)]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName, bool expected)
        {
            var address = new Address();
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(address);

            var target = new TimeZoneValueGenerator();

            var actual = (string)target.Generate(type, referenceName, executeStrategy);

            actual.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(typeof(Stream), "timezone")]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(string), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var buildChain = new LinkedList<object>();
            var resolver = new DefaultPropertyResolver();

            var configuration = Substitute.For<IBuildConfiguration>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            configuration.PropertyResolver.Returns(resolver);
            executeStrategy.Configuration.Returns(configuration);
            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TimeZoneValueGenerator();

            Action action = () => target.Generate(type, referenceName, executeStrategy);

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void HasHigherPriorityThanStringValueGeneratorTest()
        {
            var target = new TimeZoneValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
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
            var address = new Address();
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(address);

            var target = new TimeZoneValueGenerator();

            var actual = target.IsSupported(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new TimeZoneValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsPositiveValueTest()
        {
            var target = new TimeZoneValueGenerator();

            target.Priority.Should().BeGreaterThan(0);
        }
    }
}
﻿namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.Data;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NodaTime.TimeZones;
    using NSubstitute;
    using Xunit;

    public class TimeZoneValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomTimeZoneMatchingCaseInsensitiveCountryTest()
        {
            var address = new Address
            {
                Country = "australia"
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new TimeZoneValueGenerator();

            var actual = target.Generate(typeof(string), "timezone", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var valueToMatch = address.Country.ToUpperInvariant();

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
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new TimeZoneValueGenerator();

            var actual = target.Generate(typeof(string), "timezone", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            TestData.TimeZones.Where(x => x.StartsWith(address.Country, StringComparison.OrdinalIgnoreCase)).Should()
                .Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomTimeZoneMatchingCountryWhenNoCityMatchTest()
        {
            var address = new Address
            {
                City = Guid.NewGuid().ToString(),
                Country = "Australia"
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new TimeZoneValueGenerator();

            var actual = target.Generate(typeof(string), "timezone", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            TestData.TimeZones.Where(x => x.StartsWith(address.Country, StringComparison.OrdinalIgnoreCase)).Should()
                .Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomTimeZoneWhenNoMatchingCountryTest()
        {
            var address = new Address
            {
                Country = Guid.NewGuid().ToString()
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new TimeZoneValueGenerator();

            var actual = target.Generate(typeof(string), "timezone", executeStrategy) as string;

            TestData.TimeZones.Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomValueTest()
        {
            var address = new Address();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new TimeZoneValueGenerator();

            var first = (string) target.Generate(typeof(string), "timezone", executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (string) target.Generate(typeof(string), "timezone", executeStrategy);

                if (string.Equals(first, second, StringComparison.OrdinalIgnoreCase) == false)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsStringValueTest()
        {
            var address = new Address();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new TimeZoneValueGenerator();

            var actual = target.Generate(typeof(string), "timezone", executeStrategy);

            actual.Should().BeOfType<string>();
            actual.As<string>().Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void GenerateReturnsValueInNodaTimeDatabaseTest()
        {
            var address = new Address();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new TimeZoneValueGenerator();

            var ids = TzdbDateTimeZoneSource.Default.GetIds().ToList();

            for (var index = 0; index < 1000; index++)
            {
                var actual = target.Generate(typeof(string), "timezone", executeStrategy) as string;

                ids.Should().Contain(actual);
            }
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
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new TimeZoneValueGenerator();

            var actual = target.Generate(typeof(string), "timezone", executeStrategy) as string;

            actual.Should().Be("Australia/Canberra");
        }

        [Theory]
        [InlineData(typeof(string), "timezone")]
        [InlineData(typeof(string), "TimeZone")]
        [InlineData(typeof(string), "timeZone")]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName)
        {
            var address = new Address();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new TimeZoneValueGenerator();

            var actual = (string) target.Generate(type, referenceName, executeStrategy);

            actual.Should().NotBeNullOrEmpty();
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
        public void IsMatchTest(Type type, string referenceName, bool expected)
        {
            var address = new Address();
            var buildChain = new BuildHistory();

            buildChain.Push(address);

            var target = new TimeZoneValueGenerator();

            var actual = target.IsMatch(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullTypeTest()
        {
            var target = new TimeZoneValueGenerator();

            Action action = () => target.IsMatch(null, null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsPositiveValueTest()
        {
            var target = new TimeZoneValueGenerator();

            target.Priority.Should().BeGreaterThan(0);
        }
    }
}
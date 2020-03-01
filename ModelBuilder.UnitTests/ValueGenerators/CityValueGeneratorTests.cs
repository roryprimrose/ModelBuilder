namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.Data;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class CityValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomCityMatchingCaseInsensitiveCountryTest()
        {
            var address = new Address
            {
                Country = "CANADA"
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new Wrapper();

            var actual = target.RunGenerate(typeof(string), "city", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var valueToMatch = address.Country.ToUpperInvariant();

            var possibleMatches = TestData.Locations.Where(x => x.Country.ToUpperInvariant() == valueToMatch);

            possibleMatches.Select(x => x.City).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomCityMatchingCountryTest()
        {
            var address = new Address
            {
                Country = "Canada"
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new Wrapper();

            var actual = target.RunGenerate(typeof(string), "city", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var possibleMatches = TestData.Locations.Where(x => x.Country == address.Country);

            possibleMatches.Select(x => x.City).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomCityWhenNoMatchingCountryTest()
        {
            var address = new Address
            {
                Country = Guid.NewGuid().ToString()
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new Wrapper();

            var actual = target.RunGenerate(typeof(string), "city", executeStrategy) as string;

            TestData.Locations.Select(x => x.City).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomStateMatchingCaseInsensitiveStateTest()
        {
            var address = new Address
            {
                State = "ONTARIO"
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new Wrapper();

            var actual = target.RunGenerate(typeof(string), "city", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var valueToMatch = address.State.ToUpperInvariant();

            var possibleMatches = TestData.Locations.Where(x => x.State.ToUpperInvariant() == valueToMatch);

            possibleMatches.Select(x => x.City).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomStateMatchingStateTest()
        {
            var address = new Address
            {
                State = "Ontario"
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new Wrapper();

            var actual = target.RunGenerate(typeof(string), "city", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var possibleMatches = TestData.Locations.Where(x => x.State == address.State);

            possibleMatches.Select(x => x.City).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomStateWhenNoMatchingStateTest()
        {
            var address = new Address
            {
                State = Guid.NewGuid().ToString()
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new Wrapper();

            var actual = target.RunGenerate(typeof(string), "city", executeStrategy) as string;

            TestData.Locations.Select(x => x.City).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomValueTest()
        {
            var address = new Address();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new Wrapper();

            var first = (string) target.RunGenerate(typeof(string), "city", executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (string) target.RunGenerate(typeof(string), "city", executeStrategy);

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

            var target = new Wrapper();

            var actual = target.RunGenerate(typeof(string), "city", executeStrategy);

            actual.Should().BeOfType<string>();
            actual.As<string>().Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData("city")]
        [InlineData("City")]
        [InlineData("CITY")]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(string referenceName)
        {
            var address = new Address();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new Wrapper();

            var actual = (string) target.RunGenerate(typeof(string), referenceName, executeStrategy);

            actual.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void HasHigherPriorityThanStringValueGeneratorTest()
        {
            var target = new Wrapper();
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
        public void IsMatchReturnsExpectedValueTest(Type type, string referenceName, bool expected)
        {
            var address = new Address();
            var buildChain = new BuildHistory();

            buildChain.Push(address);

            var target = new Wrapper();

            var actual = target.RunIsMatch(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullBuildChainTest()
        {
            var type = typeof(string);

            var target = new Wrapper();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => target.RunIsMatch(type, null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullTypeTest()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => target.RunIsMatch(null, null, buildChain);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : CityValueGenerator
        {
            public object RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return Generate(type, referenceName, executeStrategy);
            }

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(type, referenceName, buildChain);
            }
        }
    }
}
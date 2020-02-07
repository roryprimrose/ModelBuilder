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

    public class StateValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomStateMatchingCaseInsensitiveCountryTest()
        {
            var address = new Address
            {
                Country = "UNITED STATES"
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new StateValueGenerator();

            var actual = target.Generate(typeof(string), "state", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var valueToMatch = address.Country.ToUpperInvariant();

            var possibleMatches = TestData.Locations.Where(x => x.Country.ToUpperInvariant() == valueToMatch);

            possibleMatches.Select(x => x.State).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomStateMatchingCountryTest()
        {
            var address = new Address
            {
                Country = "United States"
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new StateValueGenerator();

            var actual = target.Generate(typeof(string), "state", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var possibleMatches = TestData.Locations.Where(x => x.Country == address.Country);

            possibleMatches.Select(x => x.State).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomStateWhenNoMatchingCountryTest()
        {
            var address = new Address
            {
                Country = Guid.NewGuid().ToString()
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new StateValueGenerator();

            var actual = target.Generate(typeof(string), "state", executeStrategy) as string;

            TestData.Locations.Select(x => x.State).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomValueTest()
        {
            var address = new Address();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new StateValueGenerator();

            var first = (string)target.Generate(typeof(string), "state", executeStrategy);

            string second = null;

            for (var index = 0; index < 1000; index++)
            {
                second = (string)target.Generate(typeof(string), "state", executeStrategy);

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

            var target = new StateValueGenerator();

            var actual = target.Generate(typeof(string), "state", executeStrategy) as string;

            actual.Should().BeOfType<string>();
            actual.As<string>().Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(typeof(string), "state")]
        [InlineData(typeof(string), "State")]
        [InlineData(typeof(string), "region")]
        [InlineData(typeof(string), "Region")]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName)
        {
            var address = new Address();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new StateValueGenerator();

            var actual = (string)target.Generate(type, referenceName, executeStrategy);

            actual.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void HasHigherPriorityThanStringValueGeneratorTest()
        {
            var target = new StateValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Theory]
        [InlineData(typeof(Stream), "state", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "state", true)]
        [InlineData(typeof(string), "State", true)]
        [InlineData(typeof(string), "region", true)]
        [InlineData(typeof(string), "Region", true)]
        public void IsMatchTest(Type type, string referenceName, bool expected)
        {
            var address = new Address();
            var buildChain = new BuildHistory();

            buildChain.Push(address);

            var target = new StateValueGenerator();

            var actual = target.IsMatch(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullBuildChainTest()
        {
            var type = typeof(string);

            var target = new StateValueGenerator();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => target.IsMatch(type, null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullTypeTest()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new StateValueGenerator();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => target.IsMatch(null, null, buildChain);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
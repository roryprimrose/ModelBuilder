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

    public class CityValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomCityTest()
        {
            var address = new Address();
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(address);

            var target = new CityValueGenerator();

            var first = target.Generate(typeof(string), "city", executeStrategy);

            first.Should().BeOfType<string>();
            first.As<string>().Should().NotBeNullOrWhiteSpace();

            var otherValueFound = false;

            for (var index = 0; index < 100; index++)
            {
                var second = target.Generate(typeof(string), "city", executeStrategy);

                if (first != second)
                {
                    otherValueFound = true;

                    break;
                }
            }

            otherValueFound.Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsRandomStateMatchingCaseInsensitiveStateTest()
        {
            var address = new Address
            {
                State = "ONTARIO"
            };
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(address);

            var target = new CityValueGenerator();

            var actual = target.Generate(typeof(string), "city", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var valueToMatch = address.State.ToLowerInvariant();

            var possibleMatches = TestData.Locations.Where(x => x.State.ToLowerInvariant() == valueToMatch);

            possibleMatches.Select(x => x.City).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomStateMatchingStateTest()
        {
            var address = new Address
            {
                State = "Ontario"
            };
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(address);

            var target = new CityValueGenerator();

            var actual = target.Generate(typeof(string), "city", executeStrategy) as string;

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
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(address);

            var target = new CityValueGenerator();

            var actual = target.Generate(typeof(string), "city", executeStrategy) as string;

            TestData.Locations.Select(x => x.City).Should().Contain(actual);
        }

        [Theory]
        [InlineData("city")]
        [InlineData("City")]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(string referenceName)
        {
            var address = new Address();
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(address);

            var target = new CityValueGenerator();

            var actual = (string)target.Generate(typeof(string), referenceName, executeStrategy);

            actual.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(typeof(Stream), "city")]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(string), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var address = new Address();
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(address);

            var target = new CityValueGenerator();

            Action action = () => target.Generate(type, referenceName, executeStrategy);

            action.ShouldThrow<NotSupportedException>();
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
            var address = new Address();
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(address);

            var target = new CityValueGenerator();

            var actual = target.IsSupported(type, referenceName, buildChain);

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
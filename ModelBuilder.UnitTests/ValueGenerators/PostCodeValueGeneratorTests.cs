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

    public class PostCodeValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomCityWhenNoMatchingCityTest()
        {
            var address = new Address
            {
                City = Guid.NewGuid().ToString()
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new PostCodeValueGenerator();

            var actual = target.Generate(typeof(string), "PostCode", executeStrategy) as string;

            TestData.Locations.Select(x => x.PostCode).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomCountryWhenNoMatchingCountryTest()
        {
            var address = new Address
            {
                Country = Guid.NewGuid().ToString()
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new PostCodeValueGenerator();

            var actual = target.Generate(typeof(string), "PostCode", executeStrategy) as string;

            TestData.Locations.Select(x => x.PostCode).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomPostCodeMatchingCaseInsensitiveCityTest()
        {
            var address = new Address
            {
                City = "RIBAS"
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new PostCodeValueGenerator();

            var actual = target.Generate(typeof(string), "PostCode", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var valueToMatch = address.City.ToUpperInvariant();

            var possibleMatches = TestData.Locations.Where(x => x.City.ToUpperInvariant() == valueToMatch);

            possibleMatches.Select(x => x.PostCode).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomPostCodeMatchingCaseInsensitiveCountryTest()
        {
            var address = new Address
            {
                Country = "AUSTRALIA"
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new PostCodeValueGenerator();

            var actual = target.Generate(typeof(string), "PostCode", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var valueToMatch = address.Country.ToUpperInvariant();

            var possibleMatches = TestData.Locations.Where(x => x.Country.ToUpperInvariant() == valueToMatch);

            possibleMatches.Select(x => x.PostCode).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomPostCodeMatchingCaseInsensitiveStateTest()
        {
            var address = new Address
            {
                State = "AUSTRALIAN CAPITAL TERRITORY"
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new PostCodeValueGenerator();

            var actual = target.Generate(typeof(string), "PostCode", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var valueToMatch = address.State.ToUpperInvariant();

            var possibleMatches = TestData.Locations.Where(x => x.State.ToUpperInvariant() == valueToMatch);

            possibleMatches.Select(x => x.PostCode).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomPostCodeMatchingCityTest()
        {
            var address = new Address
            {
                City = "Ribas"
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new PostCodeValueGenerator();

            var actual = target.Generate(typeof(string), "PostCode", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var possibleMatches = TestData.Locations.Where(x => x.City == address.City);

            possibleMatches.Select(x => x.PostCode).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomPostCodeMatchingCountryTest()
        {
            var address = new Address
            {
                Country = "Australia"
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new PostCodeValueGenerator();

            var actual = target.Generate(typeof(string), "PostCode", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var possibleMatches = TestData.Locations.Where(x => x.Country == address.Country);

            possibleMatches.Select(x => x.PostCode).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomPostCodeMatchingStateTest()
        {
            var address = new Address
            {
                State = "Australian Capital Territory"
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new PostCodeValueGenerator();

            var actual = target.Generate(typeof(string), "PostCode", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var possibleMatches = TestData.Locations.Where(x => x.State == address.State);

            possibleMatches.Select(x => x.PostCode).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomPostCodeTest()
        {
            var address = new Address();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new PostCodeValueGenerator();

            var first = (string)target.Generate(typeof(string), "Zip", executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (string)target.Generate(typeof(string), "Zip", executeStrategy);

                if (string.Equals(first, second, StringComparison.OrdinalIgnoreCase) == false)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
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

            var target = new PostCodeValueGenerator();

            var actual = target.Generate(typeof(string), "PostCode", executeStrategy) as string;

            TestData.Locations.Select(x => x.PostCode).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsStringValueTest()
        {
            var address = new Address();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new PostCodeValueGenerator();

            var actual = target.Generate(typeof(string), "Zip", executeStrategy);

            actual.Should().BeOfType<string>();
            actual.As<string>().Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData("postcode")]
        [InlineData("PostCode")]
        [InlineData("zip")]
        [InlineData("Zip")]
        [InlineData("zipCode")]
        [InlineData("ZipCode")]
        [InlineData("zipcode")]
        [InlineData("Zipcode")]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(string referenceName)
        {
            var address = new Address();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new PostCodeValueGenerator();

            var actual = (string)target.Generate(typeof(string), referenceName, executeStrategy);

            actual.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullTypeTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new PostCodeValueGenerator();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => target.Generate(null, null, executeStrategy);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void HasHigherPriorityThanStringValueGeneratorTest()
        {
            var target = new PostCodeValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Theory]
        [InlineData(typeof(Stream), "postcode", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "postcode", true)]
        [InlineData(typeof(string), "PostCode", true)]
        [InlineData(typeof(string), "zip", true)]
        [InlineData(typeof(string), "Zip", true)]
        [InlineData(typeof(string), "zipCode", true)]
        [InlineData(typeof(string), "ZipCode", true)]
        [InlineData(typeof(string), "zipcode", true)]
        [InlineData(typeof(string), "Zipcode", true)]
        public void IsSupportedTest(Type type, string referenceName, bool expected)
        {
            var address = new Address();
            var buildChain = new BuildHistory();

            buildChain.Push(address);

            var target = new PostCodeValueGenerator();

            var actual = target.IsSupported(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullBuildChainTest()
        {
            var type = typeof(string);

            var target = new PostCodeValueGenerator();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => target.IsSupported(type, null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new PostCodeValueGenerator();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => target.IsSupported(null, null, buildChain);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsPositiveValueTest()
        {
            var target = new PostCodeValueGenerator();

            target.Priority.Should().BeGreaterThan(0);
        }
    }
}
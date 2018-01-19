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

    public class PostCodeValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomCityWhenNoMatchingCityTest()
        {
            var address = new Address
            {
                City = Guid.NewGuid().ToString()
            };
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(address);

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
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(address);

            var target = new PostCodeValueGenerator();

            var actual = target.Generate(typeof(string), "PostCode", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var valueToMatch = address.City.ToLowerInvariant();

            var possibleMatches = TestData.Locations.Where(x => x.City.ToLowerInvariant() == valueToMatch);

            possibleMatches.Select(x => x.PostCode).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomPostCodeMatchingCityTest()
        {
            var address = new Address
            {
                City = "Ribas"
            };
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(address);

            var target = new PostCodeValueGenerator();

            var actual = target.Generate(typeof(string), "PostCode", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var possibleMatches = TestData.Locations.Where(x => x.City == address.City);

            possibleMatches.Select(x => x.PostCode).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomPostCodeTest()
        {
            var address = new Address();
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(address);

            var target = new PostCodeValueGenerator();

            var first = target.Generate(typeof(string), "Zip", executeStrategy);

            first.Should().BeOfType<string>();
            first.As<string>().Should().NotBeNullOrWhiteSpace();

            var otherValueFound = false;

            for (var index = 0; index < 100; index++)
            {
                var second = target.Generate(typeof(string), "Zip", executeStrategy);

                if (first != second)
                {
                    otherValueFound = true;

                    break;
                }
            }

            otherValueFound.Should().BeTrue();
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
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(address);

            var target = new PostCodeValueGenerator();

            var actual = (string)target.Generate(typeof(string), referenceName, executeStrategy);

            actual.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(typeof(Stream), "postcode")]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(string), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new PostCodeValueGenerator();

            Action action = () => target.Generate(type, referenceName, executeStrategy);

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullTypeTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new PostCodeValueGenerator();

            Action action = () => target.Generate(null, null, executeStrategy);

            action.ShouldThrow<ArgumentNullException>();
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
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(address);

            var target = new PostCodeValueGenerator();

            var actual = target.IsSupported(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new PostCodeValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsPositiveValueTest()
        {
            var target = new PostCodeValueGenerator();

            target.Priority.Should().BeGreaterThan(0);
        }
    }
}
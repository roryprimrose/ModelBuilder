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

    public class PhoneValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomPhoneMatchingCaseInsensitiveCountry()
        {
            var address = new Address
            {
                Country = "UNITED STATES"
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(string), "phone", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var valueToMatch = address.Country.ToUpperInvariant();

            var possibleMatches = TestData.Locations.Where(x => x.Country.ToUpperInvariant() == valueToMatch);

            possibleMatches.Select(x => x.Phone).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomPhoneMatchingCountry()
        {
            var address = new Address
            {
                Country = "United States"
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(string), "phone", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var possibleMatches = TestData.Locations.Where(x => x.Country == address.Country);

            possibleMatches.Select(x => x.Phone).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomPhoneWhenNoMatchingCountry()
        {
            var address = new Address
            {
                Country = Guid.NewGuid().ToString()
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(string), "phone", executeStrategy) as string;

            TestData.Locations.Select(x => x.Phone).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomValue()
        {
            var address = new Address();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var sut = new Wrapper();

            var first = (string)sut.RunGenerate(typeof(string), "cell", executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (string)sut.RunGenerate(typeof(string), "cell", executeStrategy);

                if (string.Equals(first, second, StringComparison.OrdinalIgnoreCase) == false)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsStringValue()
        {
            var address = new Address();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(string), "cell", executeStrategy);

            actual.Should().BeOfType<string>();
            actual.As<string>().Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(typeof(string), "cell")]
        [InlineData(typeof(string), "Cell")]
        [InlineData(typeof(string), "cellNumber")]
        [InlineData(typeof(string), "CellNumber")]
        [InlineData(typeof(string), "cellnumber")]
        [InlineData(typeof(string), "Cellnumber")]
        [InlineData(typeof(string), "mobile")]
        [InlineData(typeof(string), "Mobile")]
        [InlineData(typeof(string), "mobileNumber")]
        [InlineData(typeof(string), "MobileNumber")]
        [InlineData(typeof(string), "mobilenumber")]
        [InlineData(typeof(string), "Mobilenumber")]
        [InlineData(typeof(string), "phone")]
        [InlineData(typeof(string), "Phone")]
        [InlineData(typeof(string), "phoneNumber")]
        [InlineData(typeof(string), "PhoneNumber")]
        [InlineData(typeof(string), "phonenumber")]
        [InlineData(typeof(string), "Phonenumber")]
        [InlineData(typeof(string), "fax")]
        [InlineData(typeof(string), "Fax")]
        [InlineData(typeof(string), "faxNumber")]
        [InlineData(typeof(string), "FaxNumber")]
        [InlineData(typeof(string), "faxnumber")]
        [InlineData(typeof(string), "Faxnumber")]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName)
        {
            var address = new Address();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var sut = new Wrapper();

            var actual = (string)sut.RunGenerate(type, referenceName, executeStrategy);

            actual.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void HasHigherPriorityThanStringValueGenerator()
        {
            var sut = new Wrapper();
            var other = new StringValueGenerator();

            sut.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Theory]
        [InlineData(typeof(Stream), "phonenumber", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "cell", true)]
        [InlineData(typeof(string), "Cell", true)]
        [InlineData(typeof(string), "cellNumber", true)]
        [InlineData(typeof(string), "CellNumber", true)]
        [InlineData(typeof(string), "cellnumber", true)]
        [InlineData(typeof(string), "Cellnumber", true)]
        [InlineData(typeof(string), "mobile", true)]
        [InlineData(typeof(string), "Mobile", true)]
        [InlineData(typeof(string), "mobileNumber", true)]
        [InlineData(typeof(string), "MobileNumber", true)]
        [InlineData(typeof(string), "mobilenumber", true)]
        [InlineData(typeof(string), "Mobilenumber", true)]
        [InlineData(typeof(string), "phone", true)]
        [InlineData(typeof(string), "Phone", true)]
        [InlineData(typeof(string), "phoneNumber", true)]
        [InlineData(typeof(string), "PhoneNumber", true)]
        [InlineData(typeof(string), "phonenumber", true)]
        [InlineData(typeof(string), "Phonenumber", true)]
        [InlineData(typeof(string), "fax", true)]
        [InlineData(typeof(string), "Fax", true)]
        [InlineData(typeof(string), "faxNumber", true)]
        [InlineData(typeof(string), "FaxNumber", true)]
        [InlineData(typeof(string), "faxnumber", true)]
        public void IsMatchReturnsWhetherTypeAndNameAreSupportedTest(Type type, string? referenceName, bool expected)
        {
            var address = new Address();
            var buildChain = new BuildHistory();

            buildChain.Push(address);

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, referenceName!, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void PriorityReturnsPositiveValue()
        {
            var sut = new Wrapper();

            sut.Priority.Should().BeGreaterThan(0);
        }

        private class Wrapper : PhoneValueGenerator
        {
            public object RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return Generate(executeStrategy, type, referenceName)!;
            }

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(buildChain, type, referenceName);
            }
        }
    }
}
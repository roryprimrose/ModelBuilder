namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.Data;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class AddressValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var first = (string) target.RunGenerate(typeof(string), "AddressLine1", executeStrategy);

            string second = null;

            for (var index = 0; index < 1000; index++)
            {
                second = (string) target.RunGenerate(typeof(string), "AddressLine1", executeStrategy);

                if (string.Equals(first, second, StringComparison.OrdinalIgnoreCase) == false)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Theory]
        [InlineData("address")]
        [InlineData("Address")]
        [InlineData("address2")]
        [InlineData("Address2")]
        [InlineData("addressline2")]
        [InlineData("addressLine2")]
        [InlineData("AddressLine2")]
        [InlineData("Addressline2")]
        public void GenerateReturnsStreetAddressTest(string referenceName)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var actual = target.RunGenerate(typeof(string), referenceName, executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var matchingLocations = TestData.Locations.Where(
                x => actual.Contains(x.StreetName, StringComparison.OrdinalIgnoreCase) && actual.Contains(
                    x.StreetSuffix,
                    StringComparison.OrdinalIgnoreCase));

            matchingLocations.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData("address1")]
        [InlineData("Address1")]
        [InlineData("addressline1")]
        [InlineData("addressLine1")]
        [InlineData("AddressLine1")]
        [InlineData("Addressline1")]
        public void GenerateReturnsUnitFloorLocationForSecondLineTest(string referenceName)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var actual = target.RunGenerate(typeof(string), referenceName, executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();
            actual.Should().Contain("Unit ");
            actual.Should().Contain(", Floor ");
        }

        [Theory]
        [InlineData("address")]
        [InlineData("Address")]
        [InlineData("address1")]
        [InlineData("Address1")]
        [InlineData("addressline1")]
        [InlineData("addressLine1")]
        [InlineData("AddressLine1")]
        [InlineData("Addressline1")]
        [InlineData("address2")]
        [InlineData("Address2")]
        [InlineData("addressline2")]
        [InlineData("addressLine2")]
        [InlineData("AddressLine2")]
        [InlineData("Addressline2")]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(string referenceName)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var actual = target.RunGenerate(typeof(string), referenceName, executeStrategy) as string;

            actual.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("address", true)]
        [InlineData("Address", true)]
        [InlineData("address1", true)]
        [InlineData("Address1", true)]
        [InlineData("address2", true)]
        [InlineData("Address2", true)]
        [InlineData("address3", true)]
        [InlineData("Address3", true)]
        [InlineData("addressline1", true)]
        [InlineData("addressLine1", true)]
        [InlineData("AddressLine1", true)]
        [InlineData("Addressline1", true)]
        [InlineData("addressline2", true)]
        [InlineData("addressLine2", true)]
        [InlineData("AddressLine2", true)]
        [InlineData("Addressline2", true)]
        [InlineData("addressline3", true)]
        [InlineData("addressLine3", true)]
        [InlineData("AddressLine3", true)]
        [InlineData("Addressline3", true)]
        [InlineData("addressline4", true)]
        [InlineData("addressLine4", true)]
        [InlineData("AddressLine4", true)]
        [InlineData("Addressline4", true)]
        [InlineData("emailAddress", false)]
        [InlineData("EmailAddress", false)]
        [InlineData("internetAddress", false)]
        [InlineData("InternetAddress", false)]
        [InlineData("WrongName", false)]
        public void IsMatchCorrectlyEvaluatesReferenceName(string referenceName, bool expected)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

            var actual = target.RunIsMatch(typeof(string), referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void PriorityReturnsValueHigherThanStringValueGenerator()
        {
            var target = new AddressValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Fact]
        public void PriorityReturnsValueLowerThanEmailValueGenerator()
        {
            var target = new AddressValueGenerator();
            var other = new EmailValueGenerator();

            target.Priority.Should().BeLessThan(other.Priority);
        }

        private class Wrapper : AddressValueGenerator
        {
            public object RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return base.Generate(executeStrategy, type, referenceName);
            }

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return base.IsMatch(buildChain, type, referenceName);
            }
        }
    }
}
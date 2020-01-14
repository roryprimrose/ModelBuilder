namespace ModelBuilder.UnitTests
{
    using System;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.Data;
    using NSubstitute;
    using Xunit;

    public class AddressValueGeneratorTests
    {
        [Theory]
        [InlineData("address3")]
        [InlineData("addressline3")]
        [InlineData("address4")]
        [InlineData("addressline4")]
        [InlineData("address5")]
        [InlineData("addressline5")]
        public void GenerateReturnsNullForAddressLinesBeyondSecondTest(string referenceName)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AddressValueGenerator();

            var actual = (string) target.Generate(typeof(string), referenceName, executeStrategy);

            actual.Should().BeNullOrEmpty();
        }

        [Fact]
        public void GenerateReturnsRandomValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AddressValueGenerator();

            var first = (string) target.Generate(typeof(string), "address", executeStrategy);

            string second = null;

            for (var index = 0; index < 1000; index++)
            {
                second = (string) target.Generate(typeof(string), "address", executeStrategy);

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
        public void GenerateReturnsStreetAddressTest(string propertyName)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AddressValueGenerator();

            var actual = target.Generate(typeof(string), propertyName, executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var matchingLocations = TestData.Locations.Where(
                x => actual.Contains(x.StreetName, StringComparison.OrdinalIgnoreCase) && actual.Contains(
                         x.StreetSuffix,
                         StringComparison.OrdinalIgnoreCase));

            matchingLocations.Should().NotBeEmpty();
        }

        [Fact]
        public void GenerateReturnsStringValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AddressValueGenerator();

            var actual = target.Generate(typeof(string), "address", executeStrategy) as string;

            actual.Should().BeOfType<string>();
            actual.As<string>().Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData("address1")]
        [InlineData("Address1")]
        [InlineData("addressline1")]
        [InlineData("addressLine1")]
        [InlineData("AddressLine1")]
        [InlineData("Addressline1")]
        public void GenerateReturnsUnitFloorLocationForSecondLineTest(string propertyName)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AddressValueGenerator();

            var actual = target.Generate(typeof(string), propertyName, executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();
            actual.Should().Contain("Unit ");
            actual.Should().Contain(", Floor ");
        }

        [Theory]
        [InlineData(typeof(string), "address", true)]
        [InlineData(typeof(string), "Address", true)]
        [InlineData(typeof(string), "address1", true)]
        [InlineData(typeof(string), "Address1", true)]
        [InlineData(typeof(string), "addressline1", true)]
        [InlineData(typeof(string), "addressLine1", true)]
        [InlineData(typeof(string), "AddressLine1", true)]
        [InlineData(typeof(string), "Addressline1", true)]
        [InlineData(typeof(string), "address2", true)]
        [InlineData(typeof(string), "Address2", true)]
        [InlineData(typeof(string), "addressline2", true)]
        [InlineData(typeof(string), "addressLine2", true)]
        [InlineData(typeof(string), "AddressLine2", true)]
        [InlineData(typeof(string), "Addressline2", true)]
        [InlineData(typeof(string), "address3", false)]
        [InlineData(typeof(string), "Address3", false)]
        [InlineData(typeof(string), "addressline3", false)]
        [InlineData(typeof(string), "addressLine3", false)]
        [InlineData(typeof(string), "AddressLine3", false)]
        [InlineData(typeof(string), "Addressline3", false)]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName, bool expected)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AddressValueGenerator();

            var actual = (string) target.Generate(type, referenceName, executeStrategy);

            if (expected)
            {
                actual.Should().NotBeNullOrEmpty();
            }
            else
            {
                actual.Should().BeNull();
            }
        }

        [Theory]
        [InlineData(typeof(Stream), "address")]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(string), "EmailAddress")]
        [InlineData(typeof(string), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AddressValueGenerator();

            Action action = () => target.Generate(type, referenceName, executeStrategy);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void HasHigherPriorityThanStringValueGeneratorTest()
        {
            var target = new AddressValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Fact]
        public void HasLowerPriorityThanEmailValueGeneratorTest()
        {
            var target = new AddressValueGenerator();
            var other = new EmailValueGenerator();

            target.Priority.Should().BeLessThan(other.Priority);
        }

        [Theory]
        [InlineData(typeof(Stream), "address", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "address", true)]
        [InlineData(typeof(string), "Address", true)]
        [InlineData(typeof(string), "address1", true)]
        [InlineData(typeof(string), "Address1", true)]
        [InlineData(typeof(string), "addressline1", true)]
        [InlineData(typeof(string), "addressLine1", true)]
        [InlineData(typeof(string), "AddressLine1", true)]
        [InlineData(typeof(string), "Addressline1", true)]
        [InlineData(typeof(string), "address2", true)]
        [InlineData(typeof(string), "Address2", true)]
        [InlineData(typeof(string), "addressline2", true)]
        [InlineData(typeof(string), "addressLine2", true)]
        [InlineData(typeof(string), "AddressLine2", true)]
        [InlineData(typeof(string), "Addressline2", true)]
        public void IsSupportedTest(Type type, string referenceName, bool expected)
        {
            var target = new AddressValueGenerator();

            var actual = target.IsSupported(type, referenceName, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new AddressValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
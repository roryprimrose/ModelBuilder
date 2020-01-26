namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.Data;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class AddressValueGeneratorTests
    {
        [Fact]
        public void CanCreateParameters()
        {
            var config = BuildConfigurationFactory.CreateEmpty().AddTypeCreator<DefaultTypeCreator>()
                .AddValueGenerator<AddressValueGenerator>();

            var actual = config.Create<ParameterTest>();

            actual.Propaddress.Should().NotBeNullOrWhiteSpace();
            actual.PropAddress.Should().NotBeNullOrWhiteSpace();
            actual.Propaddress1.Should().NotBeNullOrWhiteSpace();
            actual.PropAddress1.Should().NotBeNullOrWhiteSpace();
            actual.Propaddress2.Should().NotBeNullOrWhiteSpace();
            actual.PropAddress2.Should().NotBeNullOrWhiteSpace();
            actual.Propaddress3.Should().BeNullOrWhiteSpace();
            actual.PropAddress3.Should().BeNullOrWhiteSpace();
            actual.Propaddressline1.Should().NotBeNullOrWhiteSpace();
            actual.PropaddressLine1.Should().NotBeNullOrWhiteSpace();
            actual.PropAddressLine1.Should().NotBeNullOrWhiteSpace();
            actual.PropAddressline1.Should().NotBeNullOrWhiteSpace();
            actual.Propaddressline2.Should().NotBeNullOrWhiteSpace();
            actual.PropaddressLine2.Should().NotBeNullOrWhiteSpace();
            actual.PropAddressLine2.Should().NotBeNullOrWhiteSpace();
            actual.PropAddressline2.Should().NotBeNullOrWhiteSpace();
            actual.Propaddressline3.Should().BeNullOrWhiteSpace();
            actual.PropaddressLine3.Should().BeNullOrWhiteSpace();
            actual.PropAddressLine3.Should().BeNullOrWhiteSpace();
            actual.PropAddressline3.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        public void CanPopulateProperties()
        {
            var config = BuildConfigurationFactory.CreateEmpty().AddTypeCreator<DefaultTypeCreator>()
                .AddValueGenerator<AddressValueGenerator>().Ignoring<PropertyTest>(x => x.Address4)
                .Ignoring<PropertyTest>(x => x.WrongName).Ignoring<PropertyTest>(x => x.EmailAddress)
                .Ignoring<PropertyTest>(x => x.emailAddress).Ignoring<PropertyTest>(x => x.InternetAddress)
                .Ignoring<PropertyTest>(x => x.internetAddress);

            var actual = config.Create<PropertyTest>();

            actual.address.Should().NotBeNullOrWhiteSpace();
            actual.Address.Should().NotBeNullOrWhiteSpace();
            actual.address1.Should().NotBeNullOrWhiteSpace();
            actual.Address1.Should().NotBeNullOrWhiteSpace();
            actual.address2.Should().NotBeNullOrWhiteSpace();
            actual.Address2.Should().NotBeNullOrWhiteSpace();
            actual.address3.Should().BeNullOrWhiteSpace();
            actual.Address3.Should().BeNullOrWhiteSpace();
            actual.addressline1.Should().NotBeNullOrWhiteSpace();
            actual.addressLine1.Should().NotBeNullOrWhiteSpace();
            actual.AddressLine1.Should().NotBeNullOrWhiteSpace();
            actual.Addressline1.Should().NotBeNullOrWhiteSpace();
            actual.addressline2.Should().NotBeNullOrWhiteSpace();
            actual.addressLine2.Should().NotBeNullOrWhiteSpace();
            actual.AddressLine2.Should().NotBeNullOrWhiteSpace();
            actual.Addressline2.Should().NotBeNullOrWhiteSpace();
            actual.addressline3.Should().BeNullOrWhiteSpace();
            actual.addressLine3.Should().BeNullOrWhiteSpace();
            actual.AddressLine3.Should().BeNullOrWhiteSpace();
            actual.Addressline3.Should().BeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData("address", true)]
        [InlineData("Address", true)]
        [InlineData("address1", true)]
        [InlineData("Address1", true)]
        [InlineData("address2", true)]
        [InlineData("Address2", true)]
        [InlineData("address3", false)]
        [InlineData("Address3", false)]
        [InlineData("addressline1", true)]
        [InlineData("addressLine1", true)]
        [InlineData("AddressLine1", true)]
        [InlineData("Addressline1", true)]
        [InlineData("addressline2", true)]
        [InlineData("addressLine2", true)]
        [InlineData("AddressLine2", true)]
        [InlineData("Addressline2", true)]
        [InlineData("addressline3", false)]
        [InlineData("addressLine3", false)]
        [InlineData("AddressLine3", false)]
        [InlineData("Addressline3", false)]
        public void GenerateCorrectlyEvaluatesParameter(string referenceName, bool valueExpected)
        {
            var parameterInfo = typeof(ParameterTest).GetConstructors().Single().GetParameters()
                .Single(x => x.Name == referenceName);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var target = new AddressValueGenerator();

            var actual = target.Generate(parameterInfo, executeStrategy);

            if (valueExpected)
            {
                actual.As<string>().Should().NotBeNullOrWhiteSpace();
            }
            else
            {
                actual.As<string>().Should().BeNullOrWhiteSpace();
            }
        }

        [Theory]
        [InlineData(nameof(PropertyTest.address), true)]
        [InlineData(nameof(PropertyTest.Address), true)]
        [InlineData(nameof(PropertyTest.address1), true)]
        [InlineData(nameof(PropertyTest.Address1), true)]
        [InlineData(nameof(PropertyTest.address2), true)]
        [InlineData(nameof(PropertyTest.Address2), true)]
        [InlineData(nameof(PropertyTest.address3), false)]
        [InlineData(nameof(PropertyTest.Address3), false)]
        [InlineData(nameof(PropertyTest.addressline1), true)]
        [InlineData(nameof(PropertyTest.addressLine1), true)]
        [InlineData(nameof(PropertyTest.AddressLine1), true)]
        [InlineData(nameof(PropertyTest.Addressline1), true)]
        [InlineData(nameof(PropertyTest.addressline2), true)]
        [InlineData(nameof(PropertyTest.addressLine2), true)]
        [InlineData(nameof(PropertyTest.AddressLine2), true)]
        [InlineData(nameof(PropertyTest.Addressline2), true)]
        [InlineData(nameof(PropertyTest.addressline3), false)]
        [InlineData(nameof(PropertyTest.addressLine3), false)]
        [InlineData(nameof(PropertyTest.AddressLine3), false)]
        [InlineData(nameof(PropertyTest.Addressline3), false)]
        public void GenerateCorrectlyEvaluatesProperty(string referenceName, bool valueExpected)
        {
            var propertyInfo = typeof(PropertyTest).GetProperty(referenceName);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var target = new AddressValueGenerator();

            var actual = target.Generate(propertyInfo, executeStrategy);

            if (valueExpected)
            {
                actual.As<string>().Should().NotBeNullOrWhiteSpace();
            }
            else
            {
                actual.As<string>().Should().BeNullOrWhiteSpace();
            }
        }

        [Fact]
        public void GenerateReturnsNullForAddressLinesBeyondSecondTest()
        {
            var propertyInfo = typeof(PropertyTest).GetProperty(nameof(Address.AddressLine3));

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AddressValueGenerator();

            var actual = (string)target.Generate(propertyInfo, executeStrategy);

            actual.Should().BeNullOrEmpty();
        }

        [Fact]
        public void GenerateReturnsRandomValueTest()
        {
            var propertyInfo = typeof(PropertyTest).GetProperty(nameof(PropertyTest.Address));

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AddressValueGenerator();

            var first = (string)target.Generate(propertyInfo, executeStrategy);

            string second = null;

            for (var index = 0; index < 1000; index++)
            {
                second = (string)target.Generate(propertyInfo, executeStrategy);

                if (string.Equals(first, second, StringComparison.OrdinalIgnoreCase) == false)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Theory]
        [InlineData(nameof(PropertyTest.address))]
        [InlineData(nameof(PropertyTest.Address))]
        [InlineData(nameof(PropertyTest.address2))]
        [InlineData(nameof(PropertyTest.Address2))]
        [InlineData(nameof(PropertyTest.addressline2))]
        [InlineData(nameof(PropertyTest.addressLine2))]
        [InlineData(nameof(PropertyTest.AddressLine2))]
        [InlineData(nameof(PropertyTest.Addressline2))]
        public void GenerateReturnsStreetAddressTest(string propertyName)
        {
            var propertyInfo = typeof(PropertyTest).GetProperty(propertyName);

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AddressValueGenerator();

            var actual = target.Generate(propertyInfo, executeStrategy) as string;

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
            var propertyInfo = typeof(PropertyTest).GetProperty(nameof(PropertyTest.address));

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AddressValueGenerator();

            var actual = target.Generate(propertyInfo, executeStrategy) as string;

            actual.Should().BeOfType<string>();
            actual.As<string>().Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(nameof(PropertyTest.address1))]
        [InlineData(nameof(PropertyTest.Address1))]
        [InlineData(nameof(PropertyTest.addressline1))]
        [InlineData(nameof(PropertyTest.addressLine1))]
        [InlineData(nameof(PropertyTest.AddressLine1))]
        [InlineData(nameof(PropertyTest.Addressline1))]
        public void GenerateReturnsUnitFloorLocationForSecondLineTest(string propertyName)
        {
            var propertyInfo = typeof(PropertyTest).GetProperty(propertyName);

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AddressValueGenerator();

            var actual = target.Generate(propertyInfo, executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();
            actual.Should().Contain("Unit ");
            actual.Should().Contain(", Floor ");
        }

        [Theory]
        [InlineData(nameof(PropertyTest.address), true)]
        [InlineData(nameof(PropertyTest.Address), true)]
        [InlineData(nameof(PropertyTest.address1), true)]
        [InlineData(nameof(PropertyTest.Address1), true)]
        [InlineData(nameof(PropertyTest.addressline1), true)]
        [InlineData(nameof(PropertyTest.addressLine1), true)]
        [InlineData(nameof(PropertyTest.AddressLine1), true)]
        [InlineData(nameof(PropertyTest.Addressline1), true)]
        [InlineData(nameof(PropertyTest.address2), true)]
        [InlineData(nameof(PropertyTest.Address2), true)]
        [InlineData(nameof(PropertyTest.addressline2), true)]
        [InlineData(nameof(PropertyTest.addressLine2), true)]
        [InlineData(nameof(PropertyTest.AddressLine2), true)]
        [InlineData(nameof(PropertyTest.Addressline2), true)]
        [InlineData(nameof(PropertyTest.address3), false)]
        [InlineData(nameof(PropertyTest.addressline3), false)]
        [InlineData(nameof(PropertyTest.addressLine3), false)]
        [InlineData(nameof(PropertyTest.AddressLine3), false)]
        [InlineData(nameof(PropertyTest.Addressline3), false)]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(string referenceName, bool valueExpected)
        {
            var propertyInfo = typeof(PropertyTest).GetProperty(referenceName);

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AddressValueGenerator();

            var actual = (string)target.Generate(propertyInfo, executeStrategy);

            if (valueExpected)
            {
                actual.Should().NotBeNullOrEmpty();
            }
            else
            {
                actual.Should().BeNull();
            }
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
        [InlineData("address")]
        [InlineData("Address")]
        [InlineData("address1")]
        [InlineData("Address1")]
        [InlineData("address2")]
        [InlineData("Address2")]
        [InlineData("address3")]
        [InlineData("Address3")]
        [InlineData("addressline1")]
        [InlineData("addressLine1")]
        [InlineData("AddressLine1")]
        [InlineData("Addressline1")]
        [InlineData("addressline2")]
        [InlineData("addressLine2")]
        [InlineData("AddressLine2")]
        [InlineData("Addressline2")]
        [InlineData("addressline3")]
        [InlineData("addressLine3")]
        [InlineData("AddressLine3")]
        [InlineData("Addressline3")]
        public void IsSupportedForParameterTest(string referenceName)
        {
            var parameterInfo = typeof(ParameterTest).GetConstructors().Single().GetParameters()
                .Single(x => x.Name == referenceName);

            var buildChain = Substitute.For<IBuildChain>();

            var target = new AddressValueGenerator();

            var actual = target.IsSupported(parameterInfo, buildChain);

            actual.Should().BeTrue();
        }

        [Theory]
        [InlineData(nameof(PropertyTest.address), true)]
        [InlineData(nameof(PropertyTest.Address), true)]
        [InlineData(nameof(PropertyTest.address1), true)]
        [InlineData(nameof(PropertyTest.Address1), true)]
        [InlineData(nameof(PropertyTest.address2), true)]
        [InlineData(nameof(PropertyTest.Address2), true)]
        [InlineData(nameof(PropertyTest.address3), true)]
        [InlineData(nameof(PropertyTest.Address3), true)]
        [InlineData(nameof(PropertyTest.Address4), false)] // Wrong type
        [InlineData(nameof(PropertyTest.addressline1), true)]
        [InlineData(nameof(PropertyTest.addressLine1), true)]
        [InlineData(nameof(PropertyTest.AddressLine1), true)]
        [InlineData(nameof(PropertyTest.Addressline1), true)]
        [InlineData(nameof(PropertyTest.addressline2), true)]
        [InlineData(nameof(PropertyTest.addressLine2), true)]
        [InlineData(nameof(PropertyTest.AddressLine2), true)]
        [InlineData(nameof(PropertyTest.Addressline2), true)]
        [InlineData(nameof(PropertyTest.addressline3), true)]
        [InlineData(nameof(PropertyTest.addressLine3), true)]
        [InlineData(nameof(PropertyTest.AddressLine3), true)]
        [InlineData(nameof(PropertyTest.Addressline3), true)]
        [InlineData(nameof(PropertyTest.EmailAddress), false)]
        [InlineData(nameof(PropertyTest.emailAddress), false)]
        [InlineData(nameof(PropertyTest.InternetAddress), false)]
        [InlineData(nameof(PropertyTest.internetAddress), false)]
        [InlineData(nameof(PropertyTest.WrongName), false)]
        public void IsSupportedForPropertyTest(string referenceName, bool expected)
        {
            var propertyInfo = typeof(PropertyTest).GetProperty(referenceName);

            var buildChain = Substitute.For<IBuildChain>();

            var target = new AddressValueGenerator();

            var actual = target.IsSupported(propertyInfo, buildChain);

            actual.Should().Be(expected);
        }

        private class ParameterTest
        {
            public ParameterTest(
                string address,
                string Address,
                string address1,
                string Address1,
                string address2,
                string Address2,
                string address3,
                string Address3,
                string addressline1,
                string addressLine1,
                string Addressline1,
                string AddressLine1,
                string addressline2,
                string addressLine2,
                string Addressline2,
                string AddressLine2,
                string addressline3,
                string addressLine3,
                string Addressline3,
                string AddressLine3)
            {
                Propaddress = address;
                PropAddress = Address;
                Propaddress1 = address1;
                PropAddress1 = Address1;
                Propaddress2 = address2;
                PropAddress2 = Address2;
                Propaddress3 = address3;
                PropAddress3 = Address3;
                Propaddressline1 = addressline1;
                PropaddressLine1 = addressLine1;
                PropAddressline1 = Addressline1;
                PropAddressLine1 = AddressLine1;
                Propaddressline2 = addressline2;
                PropaddressLine2 = addressLine2;
                PropAddressline2 = Addressline2;
                PropAddressLine2 = AddressLine2;
                Propaddressline3 = addressline3;
                PropaddressLine3 = addressLine3;
                PropAddressline3 = Addressline3;
                PropAddressLine3 = AddressLine3;
            }

            public string Propaddress { get; }
            public string PropAddress { get; }
            public string Propaddress1 { get; }
            public string PropAddress1 { get; }
            public string Propaddress2 { get; }
            public string PropAddress2 { get; }
            public string Propaddress3 { get; }
            public string PropAddress3 { get; }
            public string Propaddressline1 { get; }
            public string PropaddressLine1 { get; }
            public string PropAddressline1 { get; }
            public string PropAddressLine1 { get; }
            public string Propaddressline2 { get; }
            public string PropaddressLine2 { get; }
            public string PropAddressline2 { get; }
            public string PropAddressLine2 { get; }
            public string Propaddressline3 { get; }
            public string PropaddressLine3 { get; }
            public string PropAddressline3 { get; }
            public string PropAddressLine3 { get; }
        }

        private class PropertyTest
        {
            public string address { get; set; }
            public string Address { get; set; }
            public string address1 { get; set; }
            public string Address1 { get; set; }
            public string address2 { get; set; }
            public string Address2 { get; set; }
            public string address3 { get; set; }
            public string Address3 { get; set; }
            public int Address4 { get; set; } // Wrong type
            public string addressline1 { get; set; }
            public string addressLine1 { get; set; }
            public string Addressline1 { get; set; }
            public string AddressLine1 { get; set; }
            public string addressline2 { get; set; }
            public string addressLine2 { get; set; }
            public string Addressline2 { get; set; }
            public string AddressLine2 { get; set; }
            public string addressline3 { get; set; }
            public string addressLine3 { get; set; }
            public string Addressline3 { get; set; }
            public string AddressLine3 { get; set; }
            public string emailAddress { get; set; }
            public string EmailAddress { get; set; }
            public string internetAddress { get; set; }
            public string InternetAddress { get; set; }
            public string WrongName { get; set; }
        }
    }
}
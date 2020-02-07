namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Linq;
    using System.Reflection;
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
                .AddValueGenerator<AddressValueGenerator>();

            var actual = config.Create<SupportedPropertyTest>();

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
        [InlineData(nameof(SupportedPropertyTest.address), true)]
        [InlineData(nameof(SupportedPropertyTest.Address), true)]
        [InlineData(nameof(SupportedPropertyTest.address1), true)]
        [InlineData(nameof(SupportedPropertyTest.Address1), true)]
        [InlineData(nameof(SupportedPropertyTest.address2), true)]
        [InlineData(nameof(SupportedPropertyTest.Address2), true)]
        [InlineData(nameof(SupportedPropertyTest.address3), false)]
        [InlineData(nameof(SupportedPropertyTest.Address3), false)]
        [InlineData(nameof(SupportedPropertyTest.addressline1), true)]
        [InlineData(nameof(SupportedPropertyTest.addressLine1), true)]
        [InlineData(nameof(SupportedPropertyTest.AddressLine1), true)]
        [InlineData(nameof(SupportedPropertyTest.Addressline1), true)]
        [InlineData(nameof(SupportedPropertyTest.addressline2), true)]
        [InlineData(nameof(SupportedPropertyTest.addressLine2), true)]
        [InlineData(nameof(SupportedPropertyTest.AddressLine2), true)]
        [InlineData(nameof(SupportedPropertyTest.Addressline2), true)]
        [InlineData(nameof(SupportedPropertyTest.addressline3), false)]
        [InlineData(nameof(SupportedPropertyTest.addressLine3), false)]
        [InlineData(nameof(SupportedPropertyTest.AddressLine3), false)]
        [InlineData(nameof(SupportedPropertyTest.Addressline3), false)]
        public void GenerateCorrectlyEvaluatesProperty(string referenceName, bool valueExpected)
        {
            var propertyInfo = typeof(SupportedPropertyTest).GetProperty(referenceName);

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
            var propertyInfo = typeof(SupportedPropertyTest).GetProperty(nameof(Address.AddressLine3));

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AddressValueGenerator();

            var actual = (string) target.Generate(propertyInfo, executeStrategy);

            actual.Should().BeNullOrEmpty();
        }

        [Fact]
        public void GenerateReturnsRandomValueTest()
        {
            var propertyInfo = typeof(SupportedPropertyTest).GetProperty(nameof(SupportedPropertyTest.Address));

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AddressValueGenerator();

            var first = (string) target.Generate(propertyInfo, executeStrategy);

            string second = null;

            for (var index = 0; index < 1000; index++)
            {
                second = (string) target.Generate(propertyInfo, executeStrategy);

                if (string.Equals(first, second, StringComparison.OrdinalIgnoreCase) == false)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Theory]
        [InlineData(nameof(SupportedPropertyTest.address))]
        [InlineData(nameof(SupportedPropertyTest.Address))]
        [InlineData(nameof(SupportedPropertyTest.address2))]
        [InlineData(nameof(SupportedPropertyTest.Address2))]
        [InlineData(nameof(SupportedPropertyTest.addressline2))]
        [InlineData(nameof(SupportedPropertyTest.addressLine2))]
        [InlineData(nameof(SupportedPropertyTest.AddressLine2))]
        [InlineData(nameof(SupportedPropertyTest.Addressline2))]
        public void GenerateReturnsStreetAddressTest(string propertyName)
        {
            var propertyInfo = typeof(SupportedPropertyTest).GetProperty(propertyName);

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
            var propertyInfo = typeof(SupportedPropertyTest).GetProperty(nameof(SupportedPropertyTest.address));

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AddressValueGenerator();

            var actual = target.Generate(propertyInfo, executeStrategy) as string;

            actual.Should().BeOfType<string>();
            actual.As<string>().Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(nameof(SupportedPropertyTest.address1))]
        [InlineData(nameof(SupportedPropertyTest.Address1))]
        [InlineData(nameof(SupportedPropertyTest.addressline1))]
        [InlineData(nameof(SupportedPropertyTest.addressLine1))]
        [InlineData(nameof(SupportedPropertyTest.AddressLine1))]
        [InlineData(nameof(SupportedPropertyTest.Addressline1))]
        public void GenerateReturnsUnitFloorLocationForSecondLineTest(string propertyName)
        {
            var propertyInfo = typeof(SupportedPropertyTest).GetProperty(propertyName);

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
        [InlineData(nameof(SupportedPropertyTest.address), true)]
        [InlineData(nameof(SupportedPropertyTest.Address), true)]
        [InlineData(nameof(SupportedPropertyTest.address1), true)]
        [InlineData(nameof(SupportedPropertyTest.Address1), true)]
        [InlineData(nameof(SupportedPropertyTest.addressline1), true)]
        [InlineData(nameof(SupportedPropertyTest.addressLine1), true)]
        [InlineData(nameof(SupportedPropertyTest.AddressLine1), true)]
        [InlineData(nameof(SupportedPropertyTest.Addressline1), true)]
        [InlineData(nameof(SupportedPropertyTest.address2), true)]
        [InlineData(nameof(SupportedPropertyTest.Address2), true)]
        [InlineData(nameof(SupportedPropertyTest.addressline2), true)]
        [InlineData(nameof(SupportedPropertyTest.addressLine2), true)]
        [InlineData(nameof(SupportedPropertyTest.AddressLine2), true)]
        [InlineData(nameof(SupportedPropertyTest.Addressline2), true)]
        [InlineData(nameof(SupportedPropertyTest.address3), false)]
        [InlineData(nameof(SupportedPropertyTest.addressline3), false)]
        [InlineData(nameof(SupportedPropertyTest.addressLine3), false)]
        [InlineData(nameof(SupportedPropertyTest.AddressLine3), false)]
        [InlineData(nameof(SupportedPropertyTest.Addressline3), false)]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(string referenceName, bool valueExpected)
        {
            var propertyInfo = typeof(SupportedPropertyTest).GetProperty(referenceName);

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AddressValueGenerator();

            var actual = (string) target.Generate(propertyInfo, executeStrategy);

            if (valueExpected)
            {
                actual.Should().NotBeNullOrEmpty();
            }
            else
            {
                actual.Should().BeNull();
            }
        }

        [Theory]
        [MemberData(nameof(DataSet.GetParameters), typeof(ParameterTest), MemberType = typeof(DataSet))]
        public void IsMatchForParameterTest(ParameterInfo parameterInfo)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new AddressValueGenerator();

            var actual = target.IsMatch(parameterInfo, buildChain);

            actual.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(DataSet.GetProperties), typeof(UnspportedPropertyTest), MemberType = typeof(DataSet))]
        public void IsMatchReturnsFalseForUnsupportedPropertiesTest(PropertyInfo propertyInfo)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new AddressValueGenerator();

            var actual = target.IsMatch(propertyInfo, buildChain);

            actual.Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(DataSet.GetProperties), typeof(SupportedPropertyTest), MemberType = typeof(DataSet))]
        public void IsMatchReturnsTrueForSupportedPropertiesTest(PropertyInfo propertyInfo)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new AddressValueGenerator();

            var actual = target.IsMatch(propertyInfo, buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void PriorityReturnsValueHigherThanStringValueGeneratorTest()
        {
            var target = new AddressValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Fact]
        public void PriorityReturnsValueLowerThanEmailValueGeneratorTest()
        {
            var target = new AddressValueGenerator();
            var other = new EmailValueGenerator();

            target.Priority.Should().BeLessThan(other.Priority);
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

        private class SupportedPropertyTest
        {
            public string address { get; set; }
            public string Address { get; set; }
            public string address1 { get; set; }
            public string Address1 { get; set; }
            public string address2 { get; set; }
            public string Address2 { get; set; }
            public string address3 { get; set; }
            public string Address3 { get; set; }
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
        }

        private class UnspportedPropertyTest
        {
            public int Address4 { get; set; } // Wrong type
            public string emailAddress { get; set; }
            public string EmailAddress { get; set; }
            public string internetAddress { get; set; }
            public string InternetAddress { get; set; }
            public string WrongName { get; set; }
        }
    }
}
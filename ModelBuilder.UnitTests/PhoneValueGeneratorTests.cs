﻿namespace ModelBuilder.UnitTests
{
    using System;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.Data;
    using NSubstitute;
    using Xunit;

    public class PhoneValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomPhoneMatchingCaseInsensitiveCountryTest()
        {
            var address = new Address {Country = "UNITED STATES"};
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new PhoneValueGenerator();

            var actual = target.Generate(typeof(string), "phone", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var valueToMatch = address.Country.ToLowerInvariant();

            var possibleMatches = TestData.Locations.Where(x => x.Country.ToLowerInvariant() == valueToMatch);

            possibleMatches.Select(x => x.Phone).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomPhoneMatchingCountryTest()
        {
            var address = new Address {Country = "United States"};
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new PhoneValueGenerator();

            var actual = target.Generate(typeof(string), "phone", executeStrategy) as string;

            actual.Should().NotBeNullOrWhiteSpace();

            var possibleMatches = TestData.Locations.Where(x => x.Country == address.Country);

            possibleMatches.Select(x => x.Phone).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomPhoneWhenNoMatchingCountryTest()
        {
            var address = new Address {Country = Guid.NewGuid().ToString()};
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new PhoneValueGenerator();

            var actual = target.Generate(typeof(string), "phone", executeStrategy) as string;

            TestData.Locations.Select(x => x.Phone).Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomValueTest()
        {
            var address = new Address();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new PhoneValueGenerator();

            var first = target.Generate(typeof(string), "cell", executeStrategy);

            first.Should().BeOfType<string>();
            first.As<string>().Should().NotBeNullOrWhiteSpace();

            var second = target.Generate(typeof(string), "cell", executeStrategy);

            first.Should().NotBe(second);
        }

        [Theory]
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
        [InlineData(typeof(string), "Faxnumber", true)]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName, bool expected)
        {
            var address = new Address();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(address);

            var target = new PhoneValueGenerator();

            var actual = (string) target.Generate(type, referenceName, executeStrategy);

            actual.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(typeof(Stream), "phonenumber")]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(string), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new PhoneValueGenerator();

            Action action = () => target.Generate(type, referenceName, executeStrategy);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void HasHigherPriorityThanStringValueGeneratorTest()
        {
            var target = new PhoneValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
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
        public void IsSupportedTest(Type type, string referenceName, bool expected)
        {
            var address = new Address();
            var buildChain = new BuildHistory();

            buildChain.Push(address);

            var target = new PhoneValueGenerator();

            var actual = target.IsSupported(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new PhoneValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsPositiveValueTest()
        {
            var target = new PhoneValueGenerator();

            target.Priority.Should().BeGreaterThan(0);
        }
    }
}
using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
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
            var target = new AddressValueGenerator();

            var actual = (string) target.Generate(typeof(string), referenceName, null);

            actual.Should().BeNullOrEmpty();
        }

        [Fact]
        public void GenerateReturnsRandomAddressTest()
        {
            var target = new AddressValueGenerator();

            var first = target.Generate(typeof(string), "address", null);

            first.Should().BeOfType<string>();
            first.As<string>().Should().NotBeNullOrWhiteSpace();

            var second = target.Generate(typeof(string), "address", null);

            first.Should().NotBe(second);
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
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName, bool expected)
        {
            var target = new AddressValueGenerator();

            var actual = (string) target.Generate(type, referenceName, null);

            actual.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(typeof(Stream), "address")]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(string), "EmailAddress")]
        [InlineData(typeof(string), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var target = new AddressValueGenerator();

            Action action = () => target.Generate(type, referenceName, null);

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullTypeTest()
        {
            var target = new AddressValueGenerator();

            Action action = () => target.Generate(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
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

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class PhoneValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomValueTest()
        {
            var target = new PhoneValueGenerator();

            var first = target.Generate(typeof(string), "cell", null);

            first.Should().BeOfType<string>();
            first.As<string>().Should().NotBeNullOrWhiteSpace();

            var second = target.Generate(typeof(string), "cell", null);

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsValueForPhoneTypeTest()
        {
            var target = new PhoneValueGenerator();

            var actual = target.Generate(typeof(string), "Cell", null);

            actual.Should().BeOfType<string>();
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
            var target = new PhoneValueGenerator();

            var actual = (string) target.Generate(type, referenceName, null);

            actual.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(typeof(Stream), "phonenumber")]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(string), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var target = new PhoneValueGenerator();

            Action action = () => target.Generate(type, referenceName, null);

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullTypeTest()
        {
            var target = new PhoneValueGenerator();

            Action action = () => target.Generate(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
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
            var target = new PhoneValueGenerator();

            var actual = target.IsSupported(type, referenceName, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new PhoneValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
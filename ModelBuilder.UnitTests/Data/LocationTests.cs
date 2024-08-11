namespace ModelBuilder.UnitTests.Data
{
    using System;
    using FluentAssertions;
    using ModelBuilder.Data;
    using Xunit;

    public class LocationTests
    {
        [Fact]
        public void ParseCorrectlyAssignsValuesToProperties()
        {
            var country = Guid.NewGuid().ToString();
            var state = Guid.NewGuid().ToString();
            var city = Guid.NewGuid().ToString();
            var postcode = Guid.NewGuid().ToString();
            var streetName = Guid.NewGuid().ToString();
            var streetSuffix = Guid.NewGuid().ToString();
            var phone = Guid.NewGuid().ToString();
            var data = $@"{country},{state},{city},{postcode},{streetName},{streetSuffix},{phone}";

            var actual = Location.Parse(data);

            actual.Country.Should().Be(country);
            actual.State.Should().Be(state);
            actual.City.Should().Be(city);
            actual.PostCode.Should().Be(postcode);
            actual.StreetName.Should().Be(streetName);
            actual.StreetSuffix.Should().Be(streetSuffix);
            actual.Phone.Should().Be(phone);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void ParseThrowsExceptionWithInvalidData(string? data)
        {
            Action action = () => Location.Parse(data!);

            action.Should().Throw<ArgumentException>();
        }
    }
}
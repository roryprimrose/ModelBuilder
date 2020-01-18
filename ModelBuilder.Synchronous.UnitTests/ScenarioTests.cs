namespace ModelBuilder.Synchronous.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class ScenarioTests
    {
        [Fact]
        public void CanCreateEnumValue()
        {
            var actual = Model.Create<SimpleEnum>();

            Enum.IsDefined(typeof(SimpleEnum), actual).Should().BeTrue();
        }

        [Fact]
        public void CanCreateGuidValue()
        {
            var actual = Model.Create<Guid>();

            actual.Should().NotBeEmpty();
        }

        [Fact]
        public void CanCreateInstanceTest()
        {
            var actual = Model.Create<Address>();

            actual.AddressLine1.Should().NotBeNullOrWhiteSpace();
            actual.City.Should().NotBeNullOrWhiteSpace();
            actual.Country.Should().NotBeNullOrWhiteSpace();
            actual.Postcode.Should().NotBeNullOrWhiteSpace();
            actual.State.Should().NotBeNullOrWhiteSpace();
            actual.Suburb.Should().NotBeNullOrWhiteSpace();
            actual.TimeZone.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CanCreateInstanceWithEnumerablePropertyTest()
        {
            var actual = Model.Create<Company>();

            actual.Address.Should().NotBeNullOrWhiteSpace();
            actual.Name.Should().NotBeNullOrWhiteSpace();
            actual.Staff.Should().NotBeEmpty();
        }

        [Fact]
        public void CanCreateInstanceWithParametersTest()
        {
            var actual = Model.Create<ReadOnlyModel>();

            actual.Value.Should().NotBeEmpty();
        }

        [Fact]
        public void CanPopulateExistingInstanceTest()
        {
            var expected = new SlimModel();

            var actual = Model.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Value.Should().NotBeEmpty();
        }
    }
}
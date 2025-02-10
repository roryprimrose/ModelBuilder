namespace ModelBuilder.Generator.UnitTests
{
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;

    public class ModelTests
    {
        [Fact]
        public void CanCreateInstance()
        {
            var actual = Model.Create<Address>();

            actual.AddressLine1.Should().NotBeNullOrWhiteSpace();
            actual.City.Should().NotBeNullOrWhiteSpace();
            actual.Country.Should().NotBeNullOrWhiteSpace();
            actual.Postcode.Should().NotBeNullOrWhiteSpace();
            actual.State.Should().NotBeNullOrWhiteSpace();
            actual.Suburb.Should().NotBeNullOrWhiteSpace();
            actual.TimeZone.Should().NotBeNullOrWhiteSpace();
            actual.Code.Should().NotBe('\0');
        }

        [Fact]
        public void CanCreateInstanceAlreadyGenerated()
        {
            var first = Model.Create<Address>();

            first.Should().NotBeNull();

            var second = Model.Create<Office>();

            second.Should().NotBeNull();
        }
    }

    public class NextModelTests
    {
        [Fact]
        public void CanCreateInstanceAlreadyGenerated()
        {
            var first = Model.Create<Address>();

            first.Should().NotBeNull();

            var second = Model.Create<Office>();

            second.Should().NotBeNull();
        }
    }
}
namespace ModelBuilder.Generator.UnitTests.Other
{
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;

    public class ModelTests
    {
        [Fact]
        public void CanCreateInstance()
        {
            var actual = Model.Create<Address>();

            actual.AddressLine1.Should().NotBeNullOrWhiteSpace();
            actual.City.Should().NotBeNullOrWhiteSpace();
            actual.Country.Should().NotBeNullOrWhiteSpace();
            actual.Postcode.Should().NotBeNullOrWhiteSpace();
            actual.State.Should().NotBeNullOrWhiteSpace();
            actual.Suburb.Should().NotBeNullOrWhiteSpace();
            actual.TimeZone.Should().NotBeNullOrWhiteSpace();
            actual.Code.Should().NotBe('\0');
        }

        [Fact]
        public void CanCreateInstanceAlreadyGenerated()
        {
            var first = Model.Create<Address>();

            first.Should().NotBeNull();

            var second = Model.Create<Office>();

            second.Should().NotBeNull();
        }
    }

    public class NextModelTests
    {
        [Fact]
        public void CanCreateInstanceAlreadyGenerated()
        {
            var first = Model.Create<Address>();

            first.Should().NotBeNull();

            var second = Model.Create<Office>();

            second.Should().NotBeNull();
        }
    }
}
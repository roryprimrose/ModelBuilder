namespace ModelBuilder.UnitTests
{
    using FluentAssertions;
    using ModelBuilder.Data;
    using Xunit;

    public class TestDataTests
    {
        [Fact]
        public void CompaniesReturnsTestData()
        {
            var target = TestData.Companies;

            target.Should().NotBeEmpty();
        }

        [Fact]
        public void DomainsReturnsTestData()
        {
            var target = TestData.Domains;

            target.Should().NotBeEmpty();
        }

        [Fact]
        public void FemaleNamesReturnsTestData()
        {
            var target = TestData.FemaleNames;

            target.Should().NotBeEmpty();
        }

        [Fact]
        public void LastNamesReturnsTestData()
        {
            var target = TestData.LastNames;

            target.Should().NotBeEmpty();
        }

        [Fact]
        public void LocationsReturnsTestData()
        {
            var target = TestData.Locations;

            target.Should().NotBeEmpty();
        }

        [Fact]
        public void MaleNamesReturnsTestData()
        {
            var target = TestData.MaleNames;

            target.Should().NotBeEmpty();
        }

        [Fact]
        public void TimeZonesReturnsTestData()
        {
            var target = TestData.TimeZones;

            target.Should().NotBeEmpty();
        }
    }
}
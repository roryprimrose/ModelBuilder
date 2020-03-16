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
            var sut = TestData.Companies;

            sut.Should().NotBeEmpty();
        }

        [Fact]
        public void DomainsReturnsTestData()
        {
            var sut = TestData.Domains;

            sut.Should().NotBeEmpty();
        }

        [Fact]
        public void FemaleNamesReturnsTestData()
        {
            var sut = TestData.FemaleNames;

            sut.Should().NotBeEmpty();
        }

        [Fact]
        public void LastNamesReturnsTestData()
        {
            var sut = TestData.LastNames;

            sut.Should().NotBeEmpty();
        }

        [Fact]
        public void LocationsReturnsTestData()
        {
            var sut = TestData.Locations;

            sut.Should().NotBeEmpty();
        }

        [Fact]
        public void MaleNamesReturnsTestData()
        {
            var sut = TestData.MaleNames;

            sut.Should().NotBeEmpty();
        }

        [Fact]
        public void TimeZonesReturnsTestData()
        {
            var sut = TestData.TimeZones;

            sut.Should().NotBeEmpty();
        }
    }
}
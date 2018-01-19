namespace ModelBuilder.UnitTests
{
    using FluentAssertions;
    using ModelBuilder.Data;
    using Xunit;

    public class TestDataTests
    {
        [Fact]
        public void CompaniesReturnsTestDataTest()
        {
            var target = TestData.Companies;

            target.Should().NotBeEmpty();
        }

        [Fact]
        public void CulturesReturnsTestDataTest()
        {
            var target = TestData.Cultures;

            target.Should().NotBeEmpty();
        }

        [Fact]
        public void DomainsReturnsTestDataTest()
        {
            var target = TestData.Domains;

            target.Should().NotBeEmpty();
        }

        [Fact]
        public void FemaleNamesReturnsTestDataTest()
        {
            var target = TestData.FemaleNames;

            target.Should().NotBeEmpty();
        }

        [Fact]
        public void LastNamesReturnsTestDataTest()
        {
            var target = TestData.LastNames;

            target.Should().NotBeEmpty();
        }

        [Fact]
        public void LocationsReturnsTestDataTest()
        {
            var target = TestData.Locations;

            target.Should().NotBeEmpty();
        }

        [Fact]
        public void MaleNamesReturnsTestDataTest()
        {
            var target = TestData.MaleNames;

            target.Should().NotBeEmpty();
        }

        [Fact]
        public void TimeZonesReturnsTestDataTest()
        {
            var target = TestData.TimeZones;

            target.Should().NotBeEmpty();
        }
    }
}
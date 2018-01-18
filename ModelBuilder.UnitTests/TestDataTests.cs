using System.Linq;
using FluentAssertions;
using ModelBuilder.Data;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class TestDataTests
    {
        [Fact]
        public void FemalesReturnsTestDataTest()
        {
            var target = TestData.Females;

            target.Should().NotBeEmpty();
            target.All(x => x.Gender == "Female").Should().BeTrue();
        }

        [Fact]
        public void MalesReturnsTestDataTest()
        {
            var target = TestData.Males;

            target.Should().NotBeEmpty();
            target.All(x => x.Gender == "Male").Should().BeTrue();
        }

        [Fact]
        public void NextFemaleReturnsTestDataTest()
        {
            var target = TestData.NextFemale();

            target.Should().NotBeNull();
            target.Gender.Should().Be("Female");
        }

        [Fact]
        public void NextMaleReturnsTestDataTest()
        {
            var target = TestData.NextMale();

            target.Should().NotBeNull();
            target.Gender.Should().Be("Male");
        }

        [Fact]
        public void NextPersonReturnsTestDataTest()
        {
            var target = TestData.NextPerson();

            target.Should().NotBeNull();
            target.Email.Should().NotBeNullOrWhiteSpace();
            target.Domain.Should().NotBeNullOrWhiteSpace();
            target.Gender.Should().NotBeNullOrWhiteSpace();
            target.Address.Should().NotBeNullOrWhiteSpace();
            target.Company.Should().NotBeNullOrWhiteSpace();
            target.LastName.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void PeopleReturnsTestDataTest()
        {
            var target = TestData.People;

            target.Should().NotBeEmpty();
        }
    }
}
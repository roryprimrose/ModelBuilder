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
        public void PeopleReturnsTestDataTest()
        {
            var target = TestData.People;

            target.Should().NotBeEmpty();
        }
    }
}
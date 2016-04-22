using System.Linq;
using FluentAssertions;
using ModelBuilder.Data;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class FirstNameValueGeneratorTests
    {
        [Fact]
        public void GeneratorReturnsFemaleNameWhenGenderIsFemaleTest()
        {
            var person = new Person
            {
                Gender = Gender.Female
            };

            var target = new FirstNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "FirstName", person);

            TestData.Females.Any(x => x.FirstName == actual).Should().BeTrue();
        }

        [Fact]
        public void GeneratorReturnsFemaleNameWhenGenderIsUnknownTest()
        {
            var person = new Person
            {
                Gender = Gender.Unknown
            };

            var target = new FirstNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "FirstName", person);

            TestData.Females.Any(x => x.FirstName == actual).Should().BeTrue();
        }

        [Fact]
        public void GeneratorReturnsMaleNameWhenGenderIsMaleTest()
        {
            var person = new Person
            {
                Gender = Gender.Male
            };

            var target = new FirstNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "FirstName", person);

            TestData.Males.Any(x => x.FirstName == actual).Should().BeTrue();
        }

        [Fact]
        public void PriorityReturnsHigherPriorityThanStringValidatorTest()
        {
            var target = new FirstNameValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }
    }
}
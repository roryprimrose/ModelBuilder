using System.Linq;
using FluentAssertions;
using ModelBuilder.Data;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class LastNameValueGeneratorTests
    {
        [Fact]
        public void GeneratorReturnsFemaleNameWhenGenderIsFemaleTest()
        {
            var person = new Person
            {
                Gender = Gender.Female
            };

            var target = new LastNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "LastName", person);

            TestData.Females.Any(x => x.LastName == actual).Should().BeTrue();
        }

        [Fact]
        public void GeneratorReturnsFemaleNameWhenGenderIsUnknownTest()
        {
            var person = new Person
            {
                Gender = Gender.Unknown
            };

            var target = new LastNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "LastName", person);

            TestData.Females.Any(x => x.LastName == actual).Should().BeTrue();
        }

        [Fact]
        public void GeneratorReturnsMaleNameWhenGenderIsMaleTest()
        {
            var person = new Person
            {
                Gender = Gender.Male
            };

            var target = new LastNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "LastName", person);

            TestData.Males.Any(x => x.LastName == actual).Should().BeTrue();
        }

        [Fact]
        public void PriorityReturnsHigherPriorityThanStringValidatorTest()
        {
            var target = new LastNameValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }
    }
}
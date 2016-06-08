namespace ModelBuilder.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.Data;
    using Xunit;

    public class FirstNameValueGeneratorTests
    {
        [Fact]
        public void GeneratorReturnsFemaleNameWhenGenderIsFemaleTest()
        {
            var person = new Person
            {
                Gender = Gender.Female
            };
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(person);

            var target = new FirstNameValueGenerator();

            var actual = (string)target.Generate(typeof(string), "FirstName", buildChain);

            TestData.Females.Any(x => x.FirstName == actual).Should().BeTrue();
        }

        [Fact]
        public void GeneratorReturnsFemaleNameWhenGenderIsUnknownTest()
        {
            var person = new Person
            {
                Gender = Gender.Unknown
            };
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(person);

            var target = new FirstNameValueGenerator();

            var actual = (string)target.Generate(typeof(string), "FirstName", buildChain);

            TestData.Females.Any(x => x.FirstName == actual).Should().BeTrue();
        }

        [Fact]
        public void GeneratorReturnsMaleNameWhenGenderIsMaleTest()
        {
            var person = new Person
            {
                Gender = Gender.Male
            };
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(person);

            var target = new FirstNameValueGenerator();

            var actual = (string)target.Generate(typeof(string), "FirstName", buildChain);

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
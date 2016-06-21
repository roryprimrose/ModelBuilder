namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using FluentAssertions;
    using Xunit;

    public class LastNameValueGeneratorTests
    {
        [Fact]
        public void GenerateThrowsExceptionWithNullBuildChainTest()
        {
            var target = new LastNameValueGeneratorWrapper();

            Action action = () => target.RunNullTest();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void GeneratorReturnsFemaleNameWhenGenderIsFemaleTest()
        {
            var person = new Person
            {
                Gender = Gender.Female
            };
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(person);

            var target = new LastNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "LastName", buildChain);

            TestData.Females.Any(x => x.LastName == actual).Should().BeTrue();
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

            var target = new LastNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "LastName", buildChain);

            TestData.Females.Any(x => x.LastName == actual).Should().BeTrue();
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

            var target = new LastNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "LastName", buildChain);

            TestData.Males.Any(x => x.LastName == actual).Should().BeTrue();
        }

        [Fact]
        public void GeneratorReturnsNameWhenTypeLacksGenderTest()
        {
            var person = new PersonWithoutGender();
            var buildChain = new LinkedList<object>();

            buildChain.AddLast(person);

            var target = new LastNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "LastName", buildChain);

            TestData.People.Any(x => x.LastName == actual).Should().BeTrue();
        }

        [Fact]
        public void PriorityReturnsHigherPriorityThanStringValidatorTest()
        {
            var target = new LastNameValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        private class LastNameValueGeneratorWrapper : LastNameValueGenerator
        {
            public void RunNullTest()
            {
                GenerateValue(typeof(string), "LastName", null);
            }
        }
    }
}
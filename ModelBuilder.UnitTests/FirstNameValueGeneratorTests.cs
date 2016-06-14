namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.Data;
    using Xunit;

    public class FirstNameValueGeneratorTests
    {
        [Fact]
        public void GenerateThrowsExceptionWithNullBuildChainTest()
        {
            var target = new FirstNameValueGeneratorWrapper();

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

        private class FirstNameValueGeneratorWrapper : FirstNameValueGenerator
        {
            public void RunNullTest()
            {
                GenerateValue(typeof(string), "FirstName", null);
            }
        }
    }
}
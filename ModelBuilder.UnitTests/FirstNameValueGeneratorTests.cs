namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using FluentAssertions;
    using NSubstitute;
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
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(person);

            var target = new FirstNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "FirstName", executeStrategy);

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
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(person);

            var target = new FirstNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "FirstName", executeStrategy);

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
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(person);

            var target = new FirstNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "FirstName", executeStrategy);

            TestData.Males.Any(x => x.FirstName == actual).Should().BeTrue();
        }

        [Fact]
        public void GeneratorReturnsNameWhenTypeLacksGenderTest()
        {
            var person = new PersonWithoutGender();
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(person);

            var target = new FirstNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "FirstName", executeStrategy);

            TestData.People.Any(x => x.FirstName == actual).Should().BeTrue();
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
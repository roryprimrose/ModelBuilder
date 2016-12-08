namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class LastNameValueGeneratorTests
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

            var target = new LastNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "LastName", executeStrategy);

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
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(person);

            var target = new LastNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "LastName", executeStrategy);

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
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(person);

            var target = new LastNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "LastName", executeStrategy);

            TestData.Males.Any(x => x.LastName == actual).Should().BeTrue();
        }

        [Fact]
        public void GeneratorReturnsNameWhenTypeLacksGenderTest()
        {
            var person = new PersonWithoutGender();
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddLast(person);

            var target = new LastNameValueGenerator();

            var actual = (string) target.Generate(typeof(string), "LastName", executeStrategy);

            TestData.People.Any(x => x.LastName == actual).Should().BeTrue();
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
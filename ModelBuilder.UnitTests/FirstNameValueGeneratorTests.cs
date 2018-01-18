namespace ModelBuilder.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.Data;
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

            var actual = (string)target.Generate(typeof(string), "FirstName", executeStrategy);

            TestData.FemaleNames.Any(x => x == actual).Should().BeTrue();
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

            var actual = (string)target.Generate(typeof(string), "FirstName", executeStrategy);

            TestData.FemaleNames.Any(x => x == actual).Should().BeTrue();
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

            var actual = (string)target.Generate(typeof(string), "FirstName", executeStrategy);

            TestData.MaleNames.Any(x => x == actual).Should().BeTrue();
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

            var actual = (string)target.Generate(typeof(string), "FirstName", executeStrategy);

            if (TestData.MaleNames.Any(x => x == actual))
            {
                // This is a match on a male name so all good
            }
            else
            {
                // Not a male name so it must be a female name
                TestData.FemaleNames.Any(x => x == actual).Should().BeTrue(); 
            }
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
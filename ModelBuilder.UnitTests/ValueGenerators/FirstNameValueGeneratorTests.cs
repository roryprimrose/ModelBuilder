namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.Data;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class FirstNameValueGeneratorTests
    {
        [Fact]
        public void GeneratorReturnsFemaleNameWhenGenderIsFemale()
        {
            var person = new Person
            {
                Gender = Gender.Female
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "FirstName", executeStrategy);

            TestData.FemaleNames.Any(x => x == actual).Should().BeTrue();
        }

        [Fact]
        public void GeneratorReturnsFemaleNameWhenGenderIsUnknown()
        {
            var person = new Person
            {
                Gender = Gender.Unknown
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "FirstName", executeStrategy);

            TestData.FemaleNames.Any(x => x == actual).Should().BeTrue();
        }

        [Fact]
        public void GeneratorReturnsMaleNameWhenGenderIsMale()
        {
            var person = new Person
            {
                Gender = Gender.Male
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "FirstName", executeStrategy);

            TestData.MaleNames.Any(x => x == actual).Should().BeTrue();
        }

        [Fact]
        public void GeneratorReturnsNameWhenTypeLacksGender()
        {
            var person = new PersonWithoutGender();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "FirstName", executeStrategy);

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

        [Theory]
        [InlineData(typeof(Stream), "firstname", false)]
        [InlineData(typeof(string), null!, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "FirstName", true)]
        [InlineData(typeof(string), "firstname", true)]
        [InlineData(typeof(string), "FIRSTNAME", true)]
        [InlineData(typeof(string), "First_Name", true)]
        [InlineData(typeof(string), "first_name", true)]
        [InlineData(typeof(string), "FIRST_NAME", true)]
        [InlineData(typeof(string), "GivenName", true)]
        [InlineData(typeof(string), "givenname", true)]
        [InlineData(typeof(string), "GIVENNAME", true)]
        [InlineData(typeof(string), "Given_Name", true)]
        [InlineData(typeof(string), "given_name", true)]
        [InlineData(typeof(string), "GIVEN_NAME", true)]
        public void IsMatchReturnsWhetherTypeAndNameAreSupportedTest(Type type, string referenceName, bool expected)
        {
            var person = new Person();
            var buildChain = new BuildHistory();

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void PriorityReturnsHigherPriorityThanStringValidator()
        {
            var sut = new Wrapper();
            var other = new StringValueGenerator();

            sut.Priority.Should().BeGreaterThan(other.Priority);
        }

        private class Wrapper : FirstNameValueGenerator
        {
            public object RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return Generate(executeStrategy, type, referenceName)!;
            }

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(buildChain, type, referenceName);
            }
        }
    }
}
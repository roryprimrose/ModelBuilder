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

    public class MiddleNameValueGeneratorTests
    {
        [Fact]
        public void GeneratorReturnsFemaleNameWhenGenderIsFemale()
        {
            var person = new Names
            {
                Gender = Gender.Female
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "MiddleName", executeStrategy);

            TestData.FemaleNames.Any(x => x == actual).Should().BeTrue();
        }

        [Fact]
        public void GeneratorReturnsFemaleNameWhenGenderIsUnknown()
        {
            var person = new Names
            {
                Gender = Gender.Unknown
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "MiddleName", executeStrategy);

            TestData.FemaleNames.Any(x => x == actual).Should().BeTrue();
        }

        [Fact]
        public void GeneratorReturnsMaleNameWhenGenderIsMale()
        {
            var person = new Names
            {
                Gender = Gender.Male
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "MiddleName", executeStrategy);

            TestData.MaleNames.Any(x => x == actual).Should().BeTrue();
        }

        [Fact]
        public void GeneratorReturnsNameWhenTypeLacksGender()
        {
            var person = new NamesWithoutGender();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "MiddleName", executeStrategy);

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
        [InlineData(typeof(Stream), "middlename", false)]
        [InlineData(typeof(string), null!, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "MiddleName", true)]
        [InlineData(typeof(string), "middlename", true)]
        [InlineData(typeof(string), "SECONDNAME", true)]
        [InlineData(typeof(string), "Middle_Name", true)]
        [InlineData(typeof(string), "middle_name", true)]
        [InlineData(typeof(string), "SECOND_NAME", true)]
        [InlineData(typeof(string), "SecondName", true)]
        [InlineData(typeof(string), "secondname", true)]
        [InlineData(typeof(string), "MIDDLENAME", true)]
        [InlineData(typeof(string), "Second_Name", true)]
        [InlineData(typeof(string), "second_name", true)]
        [InlineData(typeof(string), "MIDDLE_NAME", true)]
        public void IsMatchReturnsWhetherTypeAndNameAreSupportedTest(Type type, string referenceName, bool expected)
        {
            var person = new Names();
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

        private class Wrapper : MiddleNameValueGenerator
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
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

    public class LastNameValueGeneratorTests
    {
        [Fact]
        public void GeneratorReturnsName()
        {
            var person = new PersonWithoutGender();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(typeof(string), "LastName", executeStrategy);

            TestData.LastNames.Any(x => x == actual).Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(Stream), "lastname", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "LastName", true)]
        [InlineData(typeof(string), "lastname", true)]
        [InlineData(typeof(string), "LASTNAME", true)]
        [InlineData(typeof(string), "Last_Name", true)]
        [InlineData(typeof(string), "last_name", true)]
        [InlineData(typeof(string), "LAST_NAME", true)]
        [InlineData(typeof(string), "Surname", true)]
        [InlineData(typeof(string), "surname", true)]
        [InlineData(typeof(string), "SURNAME", true)]
        public void IsMatchReturnsWhetherTypeAndNameAreSupportedTest(Type type, string? referenceName, bool expected)
        {
            var person = new Person();
            var buildChain = new BuildHistory();

            buildChain.Push(person);

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, referenceName!, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void PriorityReturnsHigherPriorityThanStringValidator()
        {
            var sut = new LastNameValueGenerator();
            var other = new StringValueGenerator();

            sut.Priority.Should().BeGreaterThan(other.Priority);
        }

        private class Wrapper : LastNameValueGenerator
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
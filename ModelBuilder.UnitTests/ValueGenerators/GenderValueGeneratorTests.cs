namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.IO;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class GenderValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomValues()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var maleFound = false;
            var femaleFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = (string)sut.RunGenerate(typeof(string), "Gender", executeStrategy);

                if (actual == "Male")
                {
                    maleFound = true;
                }
                else
                {
                    femaleFound = true;
                }

                if (maleFound && femaleFound)
                {
                    break;
                }
            }

            maleFound.Should().BeTrue();
            femaleFound.Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(string), "Sex", executeStrategy);

            actual.Should().BeOfType<string>();
        }

        [Theory]
        [InlineData(typeof(string), "gender")]
        [InlineData(typeof(string), "Gender")]
        [InlineData(typeof(string), "sex")]
        [InlineData(typeof(string), "Sex")]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = (string)sut.RunGenerate(type, referenceName, executeStrategy);

            actual.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(typeof(Stream), "gender", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "gender", true)]
        [InlineData(typeof(string), "Gender", true)]
        [InlineData(typeof(string), "sex", true)]
        [InlineData(typeof(string), "Sex", true)]
        public void IsMatchReturnsWhetherTypeAndNameAreSupportedTest(Type type, string? referenceName, bool expected)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, referenceName!, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void PriorityReturnsPositiveValue()
        {
            var sut = new Wrapper();

            sut.Priority.Should().BeGreaterThan(0);
        }

        private class Wrapper : GenderValueGenerator
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
namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Globalization;
    using System.IO;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class CultureValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsCultureInfoValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(CultureInfo), "culture", executeStrategy);

            actual.Should().BeOfType<CultureInfo>();
            actual.As<CultureInfo>().Should().NotBeNull();
        }

        [Fact]
        public void GenerateReturnsRandomCultureInfo()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var first = (CultureInfo)sut.RunGenerate(typeof(CultureInfo), "culture", executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (CultureInfo)sut.RunGenerate(typeof(CultureInfo), "culture", executeStrategy);

                if (string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) == false)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomStringValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var first = (string)sut.RunGenerate(typeof(string), "culture", executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (string)sut.RunGenerate(typeof(string), "culture", executeStrategy);

                if (string.Equals(first, second, StringComparison.OrdinalIgnoreCase) == false)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsStringValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(string), "culture", executeStrategy);

            actual.Should().BeOfType<string>();
            actual.As<string>().Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(typeof(string), "culture")]
        [InlineData(typeof(string), "Culture")]
        [InlineData(typeof(string), "culturename")]
        [InlineData(typeof(string), "Culturename")]
        [InlineData(typeof(string), "cultureName")]
        [InlineData(typeof(string), "CultureName")]
        [InlineData(typeof(CultureInfo), "culture")]
        [InlineData(typeof(CultureInfo), "Culture")]
        [InlineData(typeof(CultureInfo), "culturename")]
        [InlineData(typeof(CultureInfo), "Culturename")]
        [InlineData(typeof(CultureInfo), "cultureName")]
        [InlineData(typeof(CultureInfo), "CultureName")]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(type, referenceName, executeStrategy);

            if (type == typeof(string))
            {
                actual.As<string>().Should().NotBeNullOrEmpty();
            }
            else
            {
                actual.Should().NotBeNull();
            }
        }

        [Fact]
        public void HasHigherPriorityThanStringValueGenerator()
        {
            var sut = new Wrapper();
            var other = new StringValueGenerator();

            sut.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Theory]
        [InlineData(typeof(Stream), "culture", false)]
        [InlineData(typeof(string), null!, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "culture", true)]
        [InlineData(typeof(string), "Culture", true)]
        [InlineData(typeof(string), "culturename", true)]
        [InlineData(typeof(string), "Culturename", true)]
        [InlineData(typeof(string), "cultureName", true)]
        [InlineData(typeof(string), "CultureName", true)]
        [InlineData(typeof(CultureInfo), "culture", true)]
        [InlineData(typeof(CultureInfo), "Culture", true)]
        [InlineData(typeof(CultureInfo), "culturename", true)]
        [InlineData(typeof(CultureInfo), "Culturename", true)]
        [InlineData(typeof(CultureInfo), "cultureName", true)]
        [InlineData(typeof(CultureInfo), "CultureName", true)]
        public void IsMatchReturnsWhetherTypeAndNameAreSupportedTest(Type type, string referenceName, bool expected)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullType()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper();

            Action action = () => sut.RunIsMatch(null!, null!, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : CultureValueGenerator
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
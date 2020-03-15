namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.IO;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class CountryValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var first = (string) target.RunGenerate(typeof(string), "country", executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (string) target.RunGenerate(typeof(string), "country", executeStrategy);

                if (string.Equals(first, second, StringComparison.OrdinalIgnoreCase) == false)
                {
                    // We have proved that a different value can be returned
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsStringValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var actual = (string) target.RunGenerate(typeof(string), "country", executeStrategy);

            actual.Should().BeOfType<string>();
            actual.As<string>().Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(typeof(string), "country")]
        [InlineData(typeof(string), "Country")]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var actual = (string) target.RunGenerate(type, referenceName, executeStrategy);

            actual.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void HasHigherPriorityThanStringValueGeneratorTest()
        {
            var target = new Wrapper();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Theory]
        [InlineData(typeof(Stream), "country", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "country", true)]
        [InlineData(typeof(string), "Country", true)]
        public void IsMatchReturnsExpectedValueTest(Type type, string referenceName, bool expected)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

            var actual = target.RunIsMatch(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullTypeTest()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

            Action action = () => target.RunIsMatch(null, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : CountryValueGenerator
        {
            public object RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return Generate(executeStrategy, type, referenceName);
            }

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(buildChain, type, referenceName);
            }
        }
    }
}
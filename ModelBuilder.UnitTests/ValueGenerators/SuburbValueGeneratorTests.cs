namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.IO;
    using FluentAssertions;
    using ModelBuilder.Data;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class SuburbValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomSuburbTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var first = (string) target.RunGenerate(typeof(string), "suburb", executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (string) target.RunGenerate(typeof(string), "suburb", executeStrategy);

                if (string.Equals(first, second, StringComparison.OrdinalIgnoreCase) == false)
                {
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

            var actual = target.RunGenerate(typeof(string), "suburb", executeStrategy);

            actual.Should().BeOfType<string>();
            actual.As<string>().Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(typeof(string), "suburb")]
        [InlineData(typeof(string), "Suburb")]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var actual = (string) target.RunGenerate(type, referenceName, executeStrategy);

            TestData.LastNames.Should().Contain(actual);
        }

        [Fact]
        public void HasHigherPriorityThanStringValueGeneratorTest()
        {
            var target = new Wrapper();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Theory]
        [InlineData(typeof(Stream), "suburb", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "suburb", true)]
        [InlineData(typeof(string), "Suburb", true)]
        public void IsMatchReturnsWhetherTypeAndNameAreSupportedTest(Type type, string referenceName, bool expected)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

            var actual = target.RunIsMatch(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        private class Wrapper : SuburbValueGenerator
        {
            public object RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return Generate(type, referenceName, executeStrategy);
            }

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(type, referenceName, buildChain);
            }
        }
    }
}
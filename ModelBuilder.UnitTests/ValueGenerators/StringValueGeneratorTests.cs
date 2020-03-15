namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class StringValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var first = (string) target.RunGenerate(typeof(string), null, executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (string) target.RunGenerate(typeof(string), null, executeStrategy);

                if (string.Equals(first, second, StringComparison.OrdinalIgnoreCase) == false)
                {
                    break;
                }
            }

            first.Should().NotBeNull();
            second.Should().NotBeNull();
            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsStringValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var actual = target.RunGenerate(typeof(string), null, executeStrategy);

            actual.Should().BeOfType<string>();
            actual.As<string>().Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(typeof(bool), false)]
        [InlineData(typeof(string), true)]
        public void IsMatchReturnsWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

            var actual = target.RunIsMatch(type, null, buildChain);

            actual.Should().Be(supported);
        }

        private class Wrapper : StringValueGenerator
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
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

            var target = new StringValueGenerator();

            var first = (string) target.Generate(typeof(string), null, executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (string) target.Generate(typeof(string), null, executeStrategy);

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

            var target = new StringValueGenerator();

            var actual = target.Generate(typeof(string), null, executeStrategy);

            actual.Should().BeOfType<string>();
            actual.As<string>().Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(typeof(bool), false)]
        [InlineData(typeof(string), true)]
        public void IsMatchReturnsWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new StringValueGenerator();

            var actual = target.IsMatch(type, null, buildChain);

            actual.Should().Be(supported);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullTypeTest()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new StringValueGenerator();

            Action action = () => target.IsMatch(null, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
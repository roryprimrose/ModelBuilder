namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Globalization;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class CharValueGeneratorTests
    {
        private readonly ITestOutputHelper _output;

        public CharValueGeneratorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void GenerateReturnsCharValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(char), null!, executeStrategy);

            actual.Should().BeOfType<char>();
        }

        [Fact]
        public void GenerateReturnsCharValuesInSpecifiedRange()
        {
            var minValue = Convert.ToChar(33, CultureInfo.CurrentCulture);
            var maxValue = Convert.ToChar(126, CultureInfo.CurrentCulture);
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            for (var index = 0; index < 1000; index++)
            {
                var actual = sut.RunGenerate(typeof(char), null!, executeStrategy).As<char>();

                _output.WriteLine(actual.ToString());

                actual.Should().BeGreaterOrEqualTo(minValue);
                actual.Should().BeLessOrEqualTo(maxValue);
            }
        }

        [Fact]
        public void GenerateReturnsRandomValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var first = (char)sut.RunGenerate(typeof(char), null!, executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (char)sut.RunGenerate(typeof(char), null!, executeStrategy);

                if (Equals(first, second) == false)
                {
                    break;
                }
            }

            first.Should().NotBeNull();
            second.Should().NotBeNull();
            first.Should().NotBe(second);
        }

        [Theory]
        [InlineData(typeof(bool), false)]
        [InlineData(typeof(char), true)]
        public void IsMatchReturnsWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, null!, buildChain);

            actual.Should().Be(supported);
        }

        private class Wrapper : CharValueGenerator
        {
            public object RunGenerate(Type type, string? referenceName, IExecuteStrategy executeStrategy)
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
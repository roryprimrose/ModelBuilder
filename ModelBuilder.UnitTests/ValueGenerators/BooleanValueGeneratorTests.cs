namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class BooleanValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomValuesForBooleanType()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new BooleanValueGenerator();

            var trueFound = false;
            var falseFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = (bool) sut.Generate(executeStrategy, typeof(bool))!;

                if (actual)
                {
                    trueFound = true;
                }
                else
                {
                    falseFound = true;
                }

                if (trueFound && falseFound)
                {
                    break;
                }
            }

            trueFound.Should().BeTrue();
            falseFound.Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsRandomValuesForNullableBooleanType()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new BooleanValueGenerator();

            var nullFound = false;
            var trueFound = false;
            var falseFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = (bool?) sut.Generate(executeStrategy, typeof(bool?));

                if (actual == null!)
                {
                    nullFound = true;
                }
                else if (actual.Value)
                {
                    trueFound = true;
                }
                else
                {
                    falseFound = true;
                }

                if (nullFound
                    && trueFound
                    && falseFound)
                {
                    break;
                }
            }

            nullFound.Should().BeTrue();
            trueFound.Should().BeTrue();
            falseFound.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(bool), true)]
        [InlineData(typeof(bool?), true)]
        [InlineData(typeof(string), false)]
        public void IsMatchValidatesSupportedTypesTest(Type type, bool expected)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new BooleanValueGenerator();

            var actual = sut.IsMatch(buildChain, type);

            actual.Should().Be(expected);
        }
    }
}
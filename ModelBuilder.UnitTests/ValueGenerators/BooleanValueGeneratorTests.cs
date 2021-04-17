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
        public void AllowNullReturnsFalseByDefault()
        {
            var sut = new BooleanValueGenerator();

            sut.AllowNull.Should().BeFalse();
        }

        [Theory]
        [InlineData(false, 100, false)]
        [InlineData(true, 100, true)]
        [InlineData(true, 50, true)]
        [InlineData(true, 10, true)]
        [InlineData(true, 0, false)]
        public void GenerateReturnsNullBasedOnAllowNullAndPercentageChance(bool allowNull, int percentageChance,
            bool expected)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new BooleanValueGenerator
            {
                AllowNull = allowNull,
                NullPercentageChance = percentageChance
            };

            var nullFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = (bool?) sut.Generate(executeStrategy, typeof(bool?));

                if (actual == null!)
                {
                    nullFound = true;
                    break;
                }
            }

            nullFound.Should().Be(expected);
        }

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

            var sut = new BooleanValueGenerator
            {
                AllowNull = true
            };

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

        [Fact]
        public void NullPercentageChanceReturns10ByDefault()
        {
            var sut = new BooleanValueGenerator();

            sut.NullPercentageChance.Should().Be(10);
        }
    }
}
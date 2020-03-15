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
        public void GenerateReturnsRandomValuesForBooleanTypeTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new BooleanValueGenerator();

            var trueFound = false;
            var falseFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = (bool) target.Generate(executeStrategy, typeof(bool));

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
        public void GenerateReturnsRandomValuesForNullableBooleanTypeTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new BooleanValueGenerator();

            var nullFound = false;
            var trueFound = false;
            var falseFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = (bool?) target.Generate(executeStrategy, typeof(bool?));

                if (actual == null)
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

            var target = new BooleanValueGenerator();

            var actual = target.IsMatch(buildChain, type);

            actual.Should().Be(expected);
        }
    }
}
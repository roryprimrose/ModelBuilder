namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
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
                var actual = (bool) target.Generate(typeof(bool), null, executeStrategy);

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
                var actual = (bool?) target.Generate(typeof(bool?), null, executeStrategy);

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

        [Fact]
        public void GenerateReturnsValueForBooleanTypeTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new BooleanValueGenerator();

            var actual = target.Generate(typeof(bool), null, executeStrategy);

            actual.Should().BeOfType<bool>();
        }

        [Fact]
        public void GenerateReturnsValueForNullableBooleanTypeTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new BooleanValueGenerator();

            var actual = target.Generate(typeof(bool?), null, executeStrategy);

            if (actual != null)
            {
                var converted = actual as bool?;

                converted.Should().HaveValue();
            }
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new BooleanValueGenerator();

            Action action = () => target.IsSupported(null, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(bool), true)]
        [InlineData(typeof(bool?), true)]
        [InlineData(typeof(string), false)]
        public void IsSupportedValidatesSupportedTypesTest(Type type, bool expected)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new BooleanValueGenerator();

            var actual = target.IsSupported(type, null, buildChain);

            actual.Should().Be(expected);
        }
    }
}
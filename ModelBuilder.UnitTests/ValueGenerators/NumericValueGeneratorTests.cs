namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Globalization;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class NumericValueGeneratorTests
    {
        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void GenerateForTypeCanEvaluateManyTimesTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new NumericValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.Generate(type, null, executeStrategy);

                if (type.IsNullable()
                    && value == null)
                {
                    // Nullable values could be returned so nothing more to assert
                    return;
                }

                var evaluateType = type;

                if (type.IsNullable())
                {
                    evaluateType = type.GenericTypeArguments[0];
                }

                value.Should().BeOfType(evaluateType);

                var convertedValue = Convert.ToDouble(value, CultureInfo.InvariantCulture);

                convertedValue.Should().BeGreaterOrEqualTo(min);
                convertedValue.Should().BeLessOrEqualTo(max);
            }
        }

        [Fact]
        public void GenerateForTypeCanReturnNonMaxValuesTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var valueFound = false;

            var target = new NumericValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.Generate(typeof(double), null, executeStrategy);

                var actual = Convert.ToDouble(value, CultureInfo.InvariantCulture);

                if (actual.Equals(double.MaxValue) == false)
                {
                    valueFound = true;

                    break;
                }
            }

            valueFound.Should().BeTrue();
        }

        [Fact]
        public void GenerateForTypeCanReturnNonMinValuesTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var valueFound = false;

            var target = new NumericValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.Generate(typeof(double), null, executeStrategy);

                var actual = Convert.ToDouble(value, CultureInfo.InvariantCulture);

                if (actual.Equals(double.MinValue) == false)
                {
                    valueFound = true;

                    break;
                }
            }

            valueFound.Should().BeTrue();
        }

        [Fact]
        public void GenerateForTypeCanReturnNullAndNonNullValuesTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var nullFound = false;
            var valueFound = false;

            var target = new NumericValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = (int?) target.Generate(typeof(int?), null, executeStrategy);

                if (value == null)
                {
                    nullFound = true;
                }
                else
                {
                    valueFound = true;
                }

                if (nullFound && valueFound)
                {
                    break;
                }
            }

            nullFound.Should().BeTrue();
            valueFound.Should().BeTrue();
        }

        [Fact]
        public void GenerateForTypeDoesNotReturnInfinityForDoubleTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new NumericValueGenerator();

            var value = target.Generate(typeof(double), null, executeStrategy);

            var actual = Convert.ToDouble(value, CultureInfo.InvariantCulture);

            double.IsInfinity(actual).Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        public void GenerateForTypeReturnsDecimalValuesTest(Type type)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var decimalFound = false;

            var target = new NumericValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.Generate(type, null, executeStrategy);

                var actual = Convert.ToDouble(value, CultureInfo.InvariantCulture);

                if (unchecked(actual != (int) actual))
                {
                    decimalFound = true;

                    break;
                }
            }

            decimalFound.Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void GenerateForTypeReturnsNewValueTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new NumericValueGenerator();

            var value = target.Generate(type, null, executeStrategy);

            if (type.IsNullable()
                && value == null)
            {
                // Nullable values could be returned so nothing more to assert
                return;
            }

            var evaluateType = type;

            if (type.IsNullable())
            {
                evaluateType = type.GenericTypeArguments[0];
            }

            value.Should().BeOfType(evaluateType);

            var convertedValue = Convert.ToDouble(value, CultureInfo.InvariantCulture);

            convertedValue.Should().BeGreaterOrEqualTo(min);
            convertedValue.Should().BeLessOrEqualTo(max);
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void GenerateForTypeValidatesRequestedTypeTest(Type type, bool typeSupported)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new NumericValueGenerator();

            Action action = () => target.Generate(type, null, executeStrategy);

            if (typeSupported)
            {
                action.Should().NotThrow();
            }
            else
            {
                action.Should().Throw<NotSupportedException>();
            }
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsSupportedForTypeEvaluatesRequestedTypeTest(Type type, bool typeSupported)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new NumericValueGenerator();

            var actual = target.IsSupported(type, null, buildChain);

            actual.Should().Be(typeSupported);
        }

        [Fact]
        public void IsSupportedForTypeThrowsExceptionWithNullTypeTest()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new NumericValueGenerator();

            Action action = () => target.IsSupported(null, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
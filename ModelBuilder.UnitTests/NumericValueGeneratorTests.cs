namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class NumericValueGeneratorTests
    {
        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void GenerateCanEvalutateManyTimesTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new NumericValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.Generate(type, null, executeStrategy);

                if (type.IsNullable() &&
                    (value == null))
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

                var convertedValue = Convert.ToDouble(value);

                convertedValue.Should().BeGreaterOrEqualTo(min);
                convertedValue.Should().BeLessOrEqualTo(max);
            }
        }

        [Fact]
        public void GenerateCanReturnNonMaxValuesTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var valueFound = false;

            var target = new NumericValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.Generate(typeof(double), null, executeStrategy);

                var actual = Convert.ToDouble(value);

                if (actual.Equals(double.MaxValue) == false)
                {
                    valueFound = true;

                    break;
                }
            }

            valueFound.Should().BeTrue();
        }

        [Fact]
        public void GenerateCanReturnNonMinValuesTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var valueFound = false;

            var target = new NumericValueGenerator();

            for (var index = 0; index < 10000; index++)
            {
                var value = target.Generate(typeof(double), null, executeStrategy);

                var actual = Convert.ToDouble(value);

                if (actual.Equals(double.MinValue) == false)
                {
                    valueFound = true;

                    break;
                }
            }

            valueFound.Should().BeTrue();
        }

        [Fact]
        public void GenerateCanReturnNullAndNonNullValuesTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var nullFound = false;
            var valueFound = false;

            var target = new NumericValueGenerator();

            for (var index = 0; index < 100000; index++)
            {
                var value = (int?)target.Generate(typeof(int?), null, executeStrategy);

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
        public void GenerateDoesNotReturnInfinityForDoubleTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new NumericValueGenerator();

            var value = target.Generate(typeof(double), null, executeStrategy);

            var actual = Convert.ToDouble(value);

            double.IsInfinity(actual).Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        public void GenerateReturnsDecimalValuesTest(Type type)
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var decimalFound = false;

            var target = new NumericValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.Generate(type, null, executeStrategy);

                var actual = Convert.ToDouble(value);

                if (unchecked(actual != (int)actual))
                {
                    decimalFound = true;

                    break;
                }
            }

            decimalFound.Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void GenerateReturnsNewValueTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new NumericValueGenerator();

            var value = target.Generate(type, null, executeStrategy);

            if (type.IsNullable() &&
                (value == null))
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

            var convertedValue = Convert.ToDouble(value);

            convertedValue.Should().BeGreaterOrEqualTo(min);
            convertedValue.Should().BeLessOrEqualTo(max);
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void GenerateValidatesRequestedTypeTest(Type type, bool typeSupported, double min, double max)
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new NumericValueGenerator();

            Action action = () => target.Generate(type, null, executeStrategy);

            if (typeSupported)
            {
                action.ShouldNotThrow();
            }
            else
            {
                action.ShouldThrow<NotSupportedException>();
            }
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsSupportedEvaluatesRequestedTypeTest(Type type, bool typeSupported, double min, double max)
        {
            var target = new NumericValueGenerator();

            var actual = target.IsSupported(type, null, null);

            actual.Should().Be(typeSupported);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new NumericValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
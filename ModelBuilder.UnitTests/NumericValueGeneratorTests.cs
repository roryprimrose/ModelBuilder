using System;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class NumericValueGeneratorTests
    {
        [Theory]
        [ClassData(typeof (NumericTypeDataSource))]
        public void GenerateCanEvalutateManyTimesTest(Type type, bool typeSupported)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var target = new NumericValueGenerator();

            for (var index = 0; index < 100000; index++)
            {
                var value = target.Generate(type, null, null);

                value.Should().NotBeNull();
                value.Should().BeOfType(type);
            }
        }

        [Fact]
        public void GenerateCanReturnNonMaxValuesTest()
        {
            var valueFound = false;

            var target = new NumericValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.Generate(typeof (double), null, null);

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
            var valueFound = false;

            var target = new NumericValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.Generate(typeof (double), null, null);

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
        public void GenerateDoesNotReturnInfinityForDoubleTest()
        {
            var target = new NumericValueGenerator();

            var value = target.Generate(typeof (double), null, null);

            var actual = Convert.ToDouble(value);

            double.IsInfinity(actual).Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof (float))]
        [InlineData(typeof (double))]
        public void GenerateReturnsDecimalValuesTest(Type type)
        {
            var decimalFound = false;

            var target = new NumericValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.Generate(type, null, null);

                var actual = Convert.ToDouble(value);

                if (unchecked(actual != (int) actual))
                {
                    decimalFound = true;

                    break;
                }
            }

            decimalFound.Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof (NumericTypeDataSource))]
        public void GenerateReturnsNewValueTest(Type type, bool typeSupported)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var target = new NumericValueGenerator();

            var value = target.Generate(type, null, null);

            value.Should().NotBeNull();
            value.Should().BeOfType(type);
        }

        [Theory]
        [ClassData(typeof (NumericTypeDataSource))]
        public void GenerateValidatesRequestedTypeTest(Type type, bool typeSupported)
        {
            var target = new NumericValueGenerator();

            Action action = () => target.Generate(type, null, null);

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
        [ClassData(typeof (NumericTypeDataSource))]
        public void IsSupportedEvaluatesRequestedTypeTest(Type type, bool typeSupported)
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
namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using FluentAssertions;
    using Xunit;
    using Xunit.Abstractions;

    public class RandomGeneratorTests
    {
        private readonly ITestOutputHelper _output;

        public RandomGeneratorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void GetMaxDecimal()
        {
            var sut = new RandomGenerator();

            var actual = sut.GetMax(typeof(decimal));

            var converted = Convert.ToDecimal(actual, CultureInfo.InvariantCulture);

            converted.Should().Be(decimal.MaxValue);
            converted.Should().NotBe(decimal.MinValue);
        }

        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void GetMaxEvaluatesRequestedTypeTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                return;
            }

            var sut = new RandomGenerator();

            var actual = sut.GetMax(type);

            var converted = Convert.ToDouble(actual, CultureInfo.InvariantCulture);

            converted.Should().Be(max);
            converted.Should().NotBe(min);
        }

        [Fact]
        public void GetMaxThrowsExceptionWithNullType()
        {
            var sut = new RandomGenerator();

            Action action = () => sut.GetMax(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetMinDecimal()
        {
            var sut = new RandomGenerator();

            var actual = sut.GetMin(typeof(decimal));

            var converted = Convert.ToDecimal(actual, CultureInfo.InvariantCulture);

            converted.Should().Be(decimal.MinValue);
            converted.Should().NotBe(decimal.MaxValue);
        }

        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void GetMinEvaluatesRequestedTypeTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                return;
            }

            var sut = new RandomGenerator();

            var actual = sut.GetMin(type);

            var converted = Convert.ToDouble(actual, CultureInfo.InvariantCulture);

            converted.Should().Be(min);
            converted.Should().NotBe(max);
        }

        [Fact]
        public void GetMinThrowsExceptionWithNullType()
        {
            var sut = new RandomGenerator();

            Action action = () => sut.GetMin(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsSupportedEvaluatesRequestedTypeTest(Type type, bool typeSupported)
        {
            if (typeSupported == false)
            {
                return;
            }

            var sut = new RandomGenerator();

            var actual = sut.IsSupported(type);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsSupportedReturnsTrueForDecimal()
        {
            var sut = new RandomGenerator();

            var actual = sut.IsSupported(typeof(decimal));

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullType()
        {
            var sut = new RandomGenerator();

            Action action = () => sut.IsSupported(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueByteArrayPopulatesBuffer()
        {
            var buffer = new byte[10240];

            var sut = new RandomGenerator();

            sut.NextValue(buffer);

            buffer.Any(x => x != default(byte)).Should().BeTrue();
        }

        [Fact]
        public void NextValueForByteArrayThrowsExceptionWithNullBuffer()
        {
            var sut = new RandomGenerator();

            Action action = () => sut.NextValue(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueGeneratesHighCoverageOfValues()
        {
            var tracker = new Dictionary<int, int>();

            var generator = new RandomGenerator();

            for (var index = 0; index < 100000; index++)
            {
                var actual = generator.NextValue(0, 100000);

                if (tracker.ContainsKey(actual))
                {
                    tracker[actual]++;
                }
                else
                {
                    tracker[actual] = 1;
                }
            }

            _output.WriteLine("Generator created " + tracker.Count + " unique values");

            foreach (var pair in tracker.OrderBy(x => x.Key))
            {
                _output.WriteLine(pair.Key + ": " + pair.Value);
            }

            tracker.Count.Should().BeGreaterThan(9000);
        }

        [Fact]
        public void NextValueWithDecimalCanEvaluateManyTimes()
        {
            var sut = new RandomGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = sut.NextValue(typeof(decimal), decimal.MinValue, decimal.MaxValue);

                value.Should().NotBeNull();
                value.Should().BeOfType<decimal>();
            }
        }

        [Fact]
        public void NextValueWithDecimalReturnsDecimalMaxWhenGeneratedValueIsGreater()
        {
            var sut = new RandomGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = (decimal) sut.NextValue(typeof(decimal), double.MaxValue, double.MaxValue);

                value.Should().Be(decimal.MaxValue);
            }
        }

        [Fact]
        public void NextValueWithDecimalReturnsDecimalMinWhenGeneratedValueIsLower()
        {
            var sut = new RandomGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = (decimal) sut.NextValue(typeof(decimal), double.MinValue, double.MinValue);

                value.Should().Be(decimal.MinValue);
            }
        }

        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void NextValueWithTypeCanEvaluateManyTimesTest(Type type, bool typeSupported, object min, object max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var sut = new RandomGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = sut.NextValue(type, min, max);

                value.Should().NotBeNull();

                if (type.IsNullable())
                {
                    value.Should().BeOfType(type.GetGenericArguments()[0]);
                }
                else
                {
                    value.Should().BeOfType(type);
                }
            }
        }

        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void NextValueWithTypeCanReturnMaxValueTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            if (type.IsNullable())
            {
                // Ignore this test
                return;
            }

            var sut = new RandomGenerator();

            var value = sut.NextValue(type, max, max);

            // There are some special scenarios where the conversion of max values to and from double cause an exception
            if (type == typeof(decimal))
            {
                value.Should().Be(decimal.MaxValue);
            }
            else if (type == typeof(long))
            {
                value.Should().Be(long.MaxValue);
            }
            else if (type == typeof(ulong))
            {
                value.Should().Be(ulong.MaxValue);
            }
            else
            {
                var expectedValue = Convert.ChangeType(max, type, CultureInfo.InvariantCulture);

                value.Should().Be(expectedValue);
                value.Should().NotBe(min);
            }
        }

        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void NextValueWithTypeCanReturnMinValueTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            if (type.IsNullable())
            {
                // Ignore this test
                return;
            }

            var sut = new RandomGenerator();

            var value = sut.NextValue(type, min, min);

            if (type == typeof(decimal))
            {
                // Special scenario here because new Decimal(new Double(decimal.MinValue)) throws an exception
                value.Should().Be(decimal.MinValue);
            }
            else
            {
                var expectedValue = Convert.ChangeType(min, type, CultureInfo.InvariantCulture);

                value.Should().Be(expectedValue);
                value.Should().NotBe(max);
            }
        }

        [Fact]
        public void NextValueWithTypeCanReturnNonMaxValues()
        {
            var valueFound = false;

            var sut = new RandomGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = sut.NextValue(typeof(double), double.MinValue, double.MaxValue);

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
        public void NextValueWithTypeCanReturnNonMinValues()
        {
            var valueFound = false;

            var sut = new RandomGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = sut.NextValue(typeof(double), double.MinValue, double.MaxValue);

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
        public void NextValueWithTypeDoesNotReturnInfinityForDouble()
        {
            var sut = new RandomGenerator();

            var value = sut.NextValue(typeof(double), double.MinValue, double.MaxValue);

            var actual = Convert.ToDouble(value, CultureInfo.InvariantCulture);

            double.IsInfinity(actual).Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        public void NextValueWithTypeReturnsDecimalValuesTest(Type type)
        {
            var decimalFound = false;

            var sut = new RandomGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var min = sut.GetMin(type);
                var max = sut.GetMax(type);

                var value = sut.NextValue(type, min, max);

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
        public void NextValueWithTypeReturnsNewValueTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var sut = new RandomGenerator();

            var value = sut.NextValue(type, min, max);

            value.Should().NotBeNull();

            if (type.IsNullable())
            {
                value.Should().BeOfType(type.GetGenericArguments()[0]);
            }
            else
            {
                value.Should().BeOfType(type);
            }
        }

        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void NextValueWithTypeReturnsRandomValueTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                return;
            }

            var sut = new RandomGenerator();

            var actual = sut.NextValue(type, min, max);

            var converted = Convert.ToDouble(actual, CultureInfo.InvariantCulture);

            converted.Should().BeGreaterOrEqualTo(min);
            converted.Should().BeLessOrEqualTo(max);
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWhenMinimumGreaterThanMaximum()
        {
            var sut = new RandomGenerator();

            Action action = () => sut.NextValue(typeof(double), 1, 0);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWithNonNumericMaximum()
        {
            var sut = new RandomGenerator();

            Action action = () => sut.NextValue(typeof(int), 0, Guid.NewGuid().ToString());

            action.Should().Throw<FormatException>();
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWithNonNumericMinimum()
        {
            var sut = new RandomGenerator();

            Action action = () => sut.NextValue(typeof(int), Guid.NewGuid().ToString(), 0);

            action.Should().Throw<FormatException>();
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWithNullMaximum()
        {
            var sut = new RandomGenerator();

            Action action = () => sut.NextValue(typeof(int), 0, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWithNullMinimum()
        {
            var sut = new RandomGenerator();

            Action action = () => sut.NextValue(typeof(int), null, 0);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWithNullType()
        {
            var sut = new RandomGenerator();

            Action action = () => sut.NextValue(null, 0, 0);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void NextValueWithTypeValidatesRequestedTypeTest(Type type, bool typeSupported, double min, double max)
        {
            var sut = new RandomGenerator();

            Action action = () => sut.NextValue(type, min, max);

            if (typeSupported)
            {
                action.Should().NotThrow();
            }
            else
            {
                action.Should().Throw<NotSupportedException>();
            }
        }
    }
}
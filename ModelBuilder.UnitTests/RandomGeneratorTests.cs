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

        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void GetMaxEvaluatesRequestedTypeTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                return;
            }

            var target = new RandomGenerator();

            var actual = target.GetMax(type);

            var converted = Convert.ToDouble(actual, CultureInfo.InvariantCulture);

            converted.Should().Be(max);
            converted.Should().NotBe(min);
        }

        [Fact]
        public void GetMaxThrowsExceptionWithNullTypeTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.GetMax(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void GetMinEvaluatesRequestedTypeTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                return;
            }

            var target = new RandomGenerator();

            var actual = target.GetMin(type);

            var converted = Convert.ToDouble(actual, CultureInfo.InvariantCulture);

            converted.Should().Be(min);
            converted.Should().NotBe(max);
        }

        [Fact]
        public void GetMinThrowsExceptionWithNullTypeTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.GetMin(null);

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

            var target = new RandomGenerator();

            var actual = target.IsSupported(type);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.IsSupported(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueByteArrayPopulatesBufferTest()
        {
            var buffer = new byte[10240];

            var target = new RandomGenerator();

            target.NextValue(buffer);

            buffer.Any(x => x != default(byte)).Should().BeTrue();
        }

        [Fact]
        public void NextValueForByteArrayThrowsExceptionWithNullBufferTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueGeneratesHighCoverageOfValues()
        {
            var tracker = new Dictionary<int, int>();

            var generator = new RandomGenerator();

            for (var index = 0; index < 10000; index++)
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

        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void NextValueWithTypeCanEvaluateManyTimesTest(Type type, bool typeSupported, object min, object max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var target = new RandomGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.NextValue(type, min, max);

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

            var target = new RandomGenerator();

            var value = target.NextValue(type, max, max);

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

            var target = new RandomGenerator();

            var value = target.NextValue(type, min, min);

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
        public void NextValueWithTypeCanReturnNonMaxValuesTest()
        {
            var valueFound = false;

            var target = new RandomGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.NextValue(typeof(double), double.MinValue, double.MaxValue);

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
        public void NextValueWithTypeCanReturnNonMinValuesTest()
        {
            var valueFound = false;

            var target = new RandomGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.NextValue(typeof(double), double.MinValue, double.MaxValue);

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
        public void NextValueWithTypeDoesNotReturnInfinityForDoubleTest()
        {
            var target = new RandomGenerator();

            var value = target.NextValue(typeof(double), double.MinValue, double.MaxValue);

            var actual = Convert.ToDouble(value, CultureInfo.InvariantCulture);

            double.IsInfinity(actual).Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(float))]
        [InlineData(typeof(double))]
        public void NextValueWithTypeReturnsDecimalValuesTest(Type type)
        {
            var decimalFound = false;

            var target = new RandomGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var min = target.GetMin(type);
                var max = target.GetMax(type);

                var value = target.NextValue(type, min, max);

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

            var target = new RandomGenerator();

            var value = target.NextValue(type, min, max);

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

            var target = new RandomGenerator();

            var actual = target.NextValue(type, min, max);

            var converted = Convert.ToDouble(actual, CultureInfo.InvariantCulture);

            converted.Should().BeGreaterOrEqualTo(min);
            converted.Should().BeLessOrEqualTo(max);
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWhenMinimumGreaterThanMaximumTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue(typeof(double), 1, 0);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWithNonNumericMaximumTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue(typeof(int), 0, Guid.NewGuid().ToString());

            action.Should().Throw<FormatException>();
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWithNonNumericMinimumTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue(typeof(int), Guid.NewGuid().ToString(), 0);

            action.Should().Throw<FormatException>();
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWithNullMaximumTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue(typeof(int), 0, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWithNullMinimumTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue(typeof(int), null, 0);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWithNullTypeTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue(null, 0, 0);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void NextValueWithTypeValidatesRequestedTypeTest(Type type, bool typeSupported, double min, double max)
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue(type, min, max);

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
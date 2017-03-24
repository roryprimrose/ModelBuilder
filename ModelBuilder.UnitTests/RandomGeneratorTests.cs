namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Xunit;

    public class RandomGeneratorTests
    {
        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void GetMaxEvaluatesRequestedTypeTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                return;
            }

            var target = new RandomGenerator();

            var actual = target.GetMax(type);

            var converted = Convert.ToDouble(actual);

            converted.Should().Be(max);
        }

        [Fact]
        public void GetMaxThrowsExceptionWithNullTypeTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.GetMax(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void GetMinEvaluatesRequestedTypeTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                return;
            }

            var target = new RandomGenerator();

            var actual = target.GetMin(type);

            var converted = Convert.ToDouble(actual);

            converted.Should().Be(min);
        }

        [Fact]
        public void GetMinThrowsExceptionWithNullTypeTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.GetMin(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsSupportedEvaluatesRequestedTypeTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                return;
            }

            var target = new RandomGenerator();

            var actual = target.IsSupported(type);

            actual.Should().Be(typeSupported);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.IsSupported(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void NextValueByteArrayPopulatesBufferTest()
        {
            var buffer = new byte[1024];

            var target = new RandomGenerator();

            target.NextValue(buffer);

            buffer.Any(x => x != default(byte)).Should().BeTrue();
        }

        [Fact]
        public void NextValueForByteArrayThrowsExceptionWithNullBufferTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void NextValueWithTypeCanEvalutateManyTimesTest(Type type, bool typeSupported, object min, object max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var target = new RandomGenerator();

            for (var index = 0; index < 10000; index++)
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

        [Fact]
        public void NextValueWithTypeCanReturnNonMaxValuesTest()
        {
            var valueFound = false;

            var target = new RandomGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.NextValue(typeof(double), double.MinValue, double.MaxValue);

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
        public void NextValueWithTypeCanReturnNonMinValuesTest()
        {
            var valueFound = false;

            var target = new RandomGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.NextValue(typeof(double), double.MinValue, double.MaxValue);

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
        public void NextValueWithTypeDoesNotReturnInfinityForDoubleTest()
        {
            var target = new RandomGenerator();

            var value = target.NextValue(typeof(double), double.MinValue, double.MaxValue);

            var actual = Convert.ToDouble(value);

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
        [ClassData(typeof(NumericTypeDataSource))]
        public void NextValueWithTypeReturnsRandomValueTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                return;
            }

            var target = new RandomGenerator();

            var actual = target.NextValue(type, min, max);

            var converted = Convert.ToDouble(actual);

            converted.Should().BeGreaterOrEqualTo(min);
            converted.Should().BeLessOrEqualTo(max);
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWhenMinimumGreaterThanMaximumTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue(typeof(double), 1, 0);

            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWithNonNumericMaximumTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue(typeof(int), 0, Guid.NewGuid().ToString());

            action.ShouldThrow<FormatException>();
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWithNonNumericMinimumTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue(typeof(int), Guid.NewGuid().ToString(), 0);

            action.ShouldThrow<FormatException>();
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWithNullMaximumTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue(typeof(int), 0, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWithNullMinimumTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue(typeof(int), null, 0);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWithNullTypeTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue(null, 0, 0);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void NextValueWithTypeValidatesRequestedTypeTest(Type type, bool typeSupported, double min, double max)
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue(type, min, max);

            if (typeSupported)
            {
                action.ShouldNotThrow();
            }
            else
            {
                action.ShouldThrow<NotSupportedException>();
            }
        }
    }
}
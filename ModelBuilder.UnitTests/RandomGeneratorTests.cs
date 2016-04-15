using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class RandomGeneratorTests
    {
        [Theory]
        [ClassData(typeof (NumericTypeDataSource))]
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
        [ClassData(typeof (NumericTypeDataSource))]
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
        [ClassData(typeof (NumericTypeDataSource))]
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
        public void NextByteArrayPopulatesBufferTest()
        {
            var buffer = new byte[1024];

            var target = new RandomGenerator();

            target.Next(buffer);

            buffer.Any(x => x != default(byte)).Should().BeTrue();
        }

        [Fact]
        public void NextByteArrayThrowsExceptionWithNullBufferTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.Next(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [InlineData(double.MinValue, double.MaxValue)]
        [InlineData(-1, 1)]
        [InlineData(0, 0)]
        public void NextReturnsRandomValueUsingFullRangeTest(double min, double max)
        {
            var target = new RandomGenerator();

            var actual = target.Next(min, max);

            var converted = Convert.ToDouble(actual);

            converted.Should().BeGreaterOrEqualTo(min);
            converted.Should().BeLessOrEqualTo(max);
        }

        [Fact]
        public void NextThrowsExceptionWhenMinimumGreaterThanMaximumTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.Next(1, 0);

            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void NextThrowsExceptionWithUnsupportedTypeTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.Next(0M, 1M);

            action.ShouldThrow<NotSupportedException>();
        }


        [Theory]
        [ClassData(typeof (NumericTypeDataSource))]
        public void NextWithTypeCanEvalutateManyTimesTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var target = new RandomGenerator();

            for (var index = 0; index < 100000; index++)
            {
                var value = target.Next(type, min, max);

                value.Should().NotBeNull();
                value.Should().BeOfType(type);
            }
        }

        [Fact]
        public void NextWithTypeCanReturnNonMaxValuesTest()
        {
            var valueFound = false;

            var target = new RandomGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.Next(typeof (double), double.MinValue, double.MaxValue);

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
        public void NextWithTypeCanReturnNonMinValuesTest()
        {
            var valueFound = false;

            var target = new RandomGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.Next(typeof (double), double.MinValue, double.MaxValue);

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
        public void NextWithTypeDoesNotReturnInfinityForDoubleTest()
        {
            var target = new RandomGenerator();

            var value = target.Next(typeof (double), double.MinValue, double.MaxValue);

            var actual = Convert.ToDouble(value);

            double.IsInfinity(actual).Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof (float))]
        [InlineData(typeof (double))]
        public void NextWithTypeReturnsDecimalValuesTest(Type type)
        {
            var decimalFound = false;

            var target = new RandomGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var min = target.GetMin(type);
                var max = target.GetMax(type);

                var value = target.Next(type, min, max);

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
        public void NextWithTypeReturnsNewValueTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var target = new RandomGenerator();

            var value = target.Next(type, min, max);

            value.Should().NotBeNull();
            value.Should().BeOfType(type);
        }


        [Theory]
        [ClassData(typeof (NumericTypeDataSource))]
        public void NextWithTypeReturnsRandomValueTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                return;
            }

            var target = new RandomGenerator();

            var actual = target.Next(type, min, max);

            var converted = Convert.ToDouble(actual);

            converted.Should().BeGreaterOrEqualTo(min);
            converted.Should().BeLessOrEqualTo(max);
        }

        [Fact]
        public void NextWithTypeThrowsExceptionWhenMinimumGreaterThanMaximumTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.Next(typeof (double), 1, 0);

            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void NextWithTypeThrowsExceptionWithNonNumericMaximumTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.Next(typeof (int), 0, Guid.NewGuid().ToString());

            action.ShouldThrow<FormatException>();
        }

        [Fact]
        public void NextWithTypeThrowsExceptionWithNonNumericMinimumTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.Next(typeof (int), Guid.NewGuid().ToString(), 0);

            action.ShouldThrow<FormatException>();
        }

        [Fact]
        public void NextWithTypeThrowsExceptionWithNullMaximumTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.Next(typeof (int), 0, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void NextWithTypeThrowsExceptionWithNullMinimumTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.Next(typeof (int), null, 0);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void NextWithTypeThrowsExceptionWithNullTypeTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.Next(null, 0, 0);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [ClassData(typeof (NumericTypeDataSource))]
        public void NextWithTypeValidatesRequestedTypeTest(Type type, bool typeSupported, double min, double max)
        {
            var target = new RandomGenerator();

            Action action = () => target.Next(type, min, max);

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
using System;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class RandomGeneratorExtensionTests
    {
        [Fact]
        public void NextValueReturnsRandomValueTest()
        {
            var type = typeof(int);
            var expected = Environment.TickCount;

            var target = Substitute.For<IRandomGenerator>();

            target.GetMax(type).Returns(int.MaxValue);
            target.GetMin(type).Returns(int.MinValue);
            target.NextValue(type, Arg.Is<object>(int.MinValue), Arg.Is<object>(int.MaxValue)).Returns(expected);

            var actual = target.NextValue<int>();

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(double.MinValue, double.MaxValue)]
        [InlineData(-1, 1)]
        [InlineData(0, 0)]
        public void NextValueReturnsRandomValueUsingSpecifiedRangeTest(double min, double max)
        {
            var target = new RandomGenerator();

            var actual = target.NextValue(min, max);

            var converted = Convert.ToDouble(actual);

            converted.Should().BeGreaterOrEqualTo(min);
            converted.Should().BeLessOrEqualTo(max);
        }

        [Fact]
        public void NextValueThrowsExceptionWhenMinimumGreaterThanMaximumTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue(1, 0);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void NextValueThrowsExceptionWithMinimumAndMaximumWhenGeneratorIsNullTest()
        {
            RandomGenerator target = null;

            Action action = () => target.NextValue(1, 10);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueThrowsExceptionWithNullGeneratorTest()
        {
            IRandomGenerator target = null;

            Action action = () => target.NextValue<int>();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueThrowsExceptionWithUnsupportedTypeTest()
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue('C', 'C');

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void NextValueWithMaxReturnsRandomValueTest()
        {
            var type = typeof(int);
            var max = 100;
            var expected = Environment.TickCount;

            var target = Substitute.For<IRandomGenerator>();

            target.GetMin(type).Returns(int.MinValue);
            target.NextValue(type, Arg.Is<object>(int.MinValue), max).Returns(expected);

            var actual = target.NextValue(max);

            actual.Should().Be(expected);
        }

        [Fact]
        public void NextValueWithMaxThrowsExceptionWithNullGeneratorTest()
        {
            IRandomGenerator target = null;

            Action action = () => target.NextValue(100);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueWithTypeAndMaxReturnsRandomValueTest()
        {
            var max = Environment.TickCount;
            var type = typeof(int);
            var expected = Environment.TickCount;

            var target = Substitute.For<IRandomGenerator>();

            target.GetMin(type).Returns(int.MinValue);
            target.NextValue(type, Arg.Is<object>(int.MinValue), max).Returns(expected);

            var actual = target.NextValue(typeof(int), max);

            actual.Should().Be(expected);
        }

        [Fact]
        public void NextValueWithTypeAndMaxThrowsExceptionWithNullGeneratorTest()
        {
            var max = Environment.TickCount;

            IRandomGenerator target = null;

            Action action = () => target.NextValue(typeof(int), max);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueWithTypeAndMaxThrowsExceptionWithNullTypeTest()
        {
            var max = Environment.TickCount;

            var target = Substitute.For<IRandomGenerator>();

            Action action = () => target.NextValue(null, max);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueWithTypeReturnsRandomValueTest()
        {
            var type = typeof(int);
            var expected = Environment.TickCount;

            var target = Substitute.For<IRandomGenerator>();

            target.GetMax(type).Returns(int.MaxValue);
            target.GetMin(type).Returns(int.MinValue);
            target.NextValue(type, Arg.Is<object>(int.MinValue), Arg.Is<object>(int.MaxValue)).Returns(expected);

            var actual = target.NextValue(typeof(int));

            actual.Should().Be(expected);
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWithNullGeneratorTest()
        {
            IRandomGenerator target = null;

            Action action = () => target.NextValue(typeof(int));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWithNullTypeTest()
        {
            var target = Substitute.For<IRandomGenerator>();

            Action action = () => target.NextValue((Type) null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class RandomGeneratorExtensionTests
    {
        [Fact]
        public void NextValueReturnsRandomValue()
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
        public void NextValueThrowsExceptionWhenMinimumGreaterThanMaximum()
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue(1, 0);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void NextValueThrowsExceptionWithMinimumAndMaximumWhenGeneratorIsNull()
        {
            Action action = () => ((RandomGenerator) null).NextValue(1, 10);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueThrowsExceptionWithNullGenerator()
        {
            Action action = () => ((IRandomGenerator) null).NextValue<int>();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueThrowsExceptionWithUnsupportedType()
        {
            var target = new RandomGenerator();

            Action action = () => target.NextValue('C', 'C');

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void NextValueWithMaxReturnsRandomValue()
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
        public void NextValueWithMaxThrowsExceptionWithNullGenerator()
        {
            Action action = () => ((IRandomGenerator) null).NextValue(100);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueWithTypeAndMaxReturnsRandomValue()
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
        public void NextValueWithTypeAndMaxThrowsExceptionWithNullGenerator()
        {
            var max = Environment.TickCount;

            Action action = () => ((IRandomGenerator) null).NextValue(typeof(int), max);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueWithTypeAndMaxThrowsExceptionWithNullType()
        {
            var max = Environment.TickCount;

            var target = Substitute.For<IRandomGenerator>();

            Action action = () => target.NextValue(null, max);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueWithTypeReturnsRandomValue()
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
        public void NextValueWithTypeThrowsExceptionWithNullGenerator()
        {
            Action action = () => ((IRandomGenerator) null).NextValue(typeof(int));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextValueWithTypeThrowsExceptionWithNullType()
        {
            var target = Substitute.For<IRandomGenerator>();

            Action action = () => target.NextValue((Type) null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
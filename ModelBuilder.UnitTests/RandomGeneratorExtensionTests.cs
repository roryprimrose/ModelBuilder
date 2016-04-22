using System;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class RandomGeneratorExtensionTests
    {
        [Fact]
        public void NextReturnsRandomValueTest()
        {
            var type = typeof (int);
            var expected = Environment.TickCount;

            var target = Substitute.For<IRandomGenerator>();

            target.GetMax(type).Returns(int.MaxValue);
            target.GetMin(type).Returns(int.MinValue);
            target.NextValue(type, Arg.Is<double>(int.MinValue), Arg.Is<double>(int.MaxValue)).Returns(expected);

            var actual = target.Next<int>();

            actual.Should().Be(expected);
        }

        [Fact]
        public void NextThrowsExceptionWithNullGeneratorTest()
        {
            IRandomGenerator target = null;

            Action action = () => target.Next<int>();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void NextWithMaxReturnsRandomValueTest()
        {
            var type = typeof (int);
            var max = 100;
            var expected = Environment.TickCount;

            var target = Substitute.For<IRandomGenerator>();

            target.GetMin(type).Returns(int.MinValue);
            target.NextValue(type, Arg.Is<double>(int.MinValue), max).Returns(expected);

            var actual = target.Next(max);

            actual.Should().Be(expected);
        }

        [Fact]
        public void NextWithMaxThrowsExceptionWithNullGeneratorTest()
        {
            IRandomGenerator target = null;

            Action action = () => target.Next(100);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void NextWithTypeAndMaxReturnsRandomValueTest()
        {
            var max = Environment.TickCount;
            var type = typeof (int);
            var expected = Environment.TickCount;

            var target = Substitute.For<IRandomGenerator>();

            target.GetMin(type).Returns(int.MinValue);
            target.NextValue(type, Arg.Is<double>(int.MinValue), max).Returns(expected);

            var actual = target.Next(typeof (int), max);

            actual.Should().Be(expected);
        }

        [Fact]
        public void NextWithTypeAndMaxThrowsExceptionWithNullGeneratorTest()
        {
            var max = Environment.TickCount;

            IRandomGenerator target = null;

            Action action = () => target.Next(typeof (int), max);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void NextWithTypeAndMaxThrowsExceptionWithNullTypeTest()
        {
            var max = Environment.TickCount;

            var target = Substitute.For<IRandomGenerator>();

            Action action = () => target.Next(null, max);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void NextWithTypeReturnsRandomValueTest()
        {
            var type = typeof (int);
            var expected = Environment.TickCount;

            var target = Substitute.For<IRandomGenerator>();

            target.GetMax(type).Returns(int.MaxValue);
            target.GetMin(type).Returns(int.MinValue);
            target.NextValue(type, Arg.Is<double>(int.MinValue), Arg.Is<double>(int.MaxValue)).Returns(expected);

            var actual = target.Next(typeof (int));

            actual.Should().Be(expected);
        }

        [Fact]
        public void NextWithTypeThrowsExceptionWithNullGeneratorTest()
        {
            IRandomGenerator target = null;

            Action action = () => target.Next(typeof (int));

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void NextWithTypeThrowsExceptionWithNullTypeTest()
        {
            var target = Substitute.For<IRandomGenerator>();

            Action action = () => target.Next((Type) null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
using System;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class RandomExtensionsTests
    {
        [Fact]
        public void NextFloatReturnsDifferentValuesTest()
        {
            var target = new Random(Environment.TickCount);

            var first = target.NextFloat();
            var second = target.NextFloat();

            first.Should().NotBe(second);
        }

        [Fact]
        public void NextFloatThrowsExceptionWithNullRandomTest()
        {
            Random target = null;

            Action action = () => target.NextFloat();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void NextInt64ReturnsDifferentValuesTest()
        {
            var target = new Random(Environment.TickCount);

            var first = target.NextInt64();
            var second = target.NextInt64();

            first.Should().NotBe(second);
        }

        [Fact]
        public void NextInt64ThrowsExceptionWithNullRandomTest()
        {
            Random target = null;

            Action action = () => target.NextInt64();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void NextUInt32ReturnsDifferentValuesTest()
        {
            var target = new Random(Environment.TickCount);

            var first = target.NextUInt32();
            var second = target.NextUInt32();

            first.Should().NotBe(second);
        }

        [Fact]
        public void NextUInt32ThrowsExceptionWithNullRandomTest()
        {
            Random target = null;

            Action action = () => target.NextUInt32();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void NextUInt64ReturnsDifferentValuesTest()
        {
            var target = new Random(Environment.TickCount);

            var first = target.NextUInt64();
            var second = target.NextUInt64();

            first.Should().NotBe(second);
        }

        [Fact]
        public void NextUInt64ThrowsExceptionWithNullRandomTest()
        {
            Random target = null;

            Action action = () => target.NextUInt64();

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
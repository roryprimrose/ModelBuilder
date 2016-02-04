using System;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class EnumValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsValueWhenTypeIsEnumTest()
        {
            var target = new EnumValueGenerator();

            var actual = target.Generate(typeof (SimpleEnum));

            Enum.IsDefined(typeof (SimpleEnum), actual).Should().BeTrue();
        }

        [Fact]
        public void GenerateThrowsExceptionWhenTypeIsNotEnumTest()
        {
            var target = new EnumValueGenerator();

            Action action = () => target.Generate(typeof (string));

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullTypeTest()
        {
            var target = new EnumValueGenerator();

            Action action = () => target.Generate(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void IsSupportedReturnsFalseWhenTypeIsNotEnumTest()
        {
            var target = new EnumValueGenerator();

            var actual = target.IsSupported(typeof (string));

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsSupportedReturnsTrueWhenTypeIsEnumTest()
        {
            var target = new EnumValueGenerator();

            var actual = target.IsSupported(typeof (SimpleEnum));

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new EnumValueGenerator();

            Action action = () => target.IsSupported(null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
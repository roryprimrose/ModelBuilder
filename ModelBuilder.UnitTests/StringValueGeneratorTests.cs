using System;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class StringValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomValueTest()
        {
            var target = new StringValueGenerator();

            var first = target.Generate(typeof(string), null, null);
            var second = target.Generate(typeof(string), null, null);

            first.Should().NotBeNull();
            second.Should().NotBeNull();
            first.Should().NotBe(second);
        }

        [Theory]
        [InlineData(typeof(bool), false)]
        [InlineData(typeof(string), true)]
        public void GenerateReturnsValidatesTypeSupportTest(Type type, bool supported)
        {
            var target = new StringValueGenerator();

            Action action = () => target.Generate(type, null, null);

            if (supported)
            {
                action.ShouldNotThrow();
            }
            else
            {
                action.ShouldThrow<NotSupportedException>();
            }
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullTypeTest()
        {
            var target = new StringValueGenerator();

            Action action = () => target.Generate(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(bool), false)]
        [InlineData(typeof(string), true)]
        public void IsSupportedReturnsWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var target = new StringValueGenerator();

            var actual = target.IsSupported(type, null, null);

            actual.Should().Be(supported);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new StringValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
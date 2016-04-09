using System;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class NumericValueGeneratorTests
    {
        [Theory]
        [ClassData(typeof (NumericTypeDataSource))]
        public void GenerateReturnsNewValueTest(Type type, bool typeSupported)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var target = new NumericValueGenerator();

            var value = target.Generate(type, null, null);

            value.Should().NotBeNull();
            value.Should().BeOfType(type);
        }

        [Theory]
        [ClassData(typeof (NumericTypeDataSource))]
        public void GenerateValidatesRequestedTypeTest(Type type, bool typeSupported)
        {
            var target = new NumericValueGenerator();

            Action action = () => target.Generate(type, null, null);

            if (typeSupported)
            {
                action.ShouldNotThrow();
            }
            else
            {
                action.ShouldThrow<NotSupportedException>();
            }
        }

        [Theory]
        [ClassData(typeof (NumericTypeDataSource))]
        public void IsSupportedEvaluatesRequestedTypeTest(Type type, bool typeSupported)
        {
            var target = new NumericValueGenerator();

            var actual = target.IsSupported(type, null, null);

            actual.Should().Be(typeSupported);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new NumericValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
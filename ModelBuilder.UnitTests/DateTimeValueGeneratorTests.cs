using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class DateTimeValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsDateTimeOffsetValueTest()
        {
            var target = new DateTimeValueGenerator();

            var actual = target.Generate(typeof (DateTimeOffset), null, null);

            actual.Should().BeOfType<DateTimeOffset>();
            actual.As<DateTimeOffset>().Offset.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void GenerateReturnsDateTimeValueTest()
        {
            var target = new DateTimeValueGenerator();

            var actual = target.Generate(typeof (DateTime), null, null);

            actual.Should().BeOfType<DateTime>();
            actual.As<DateTime>().Kind.Should().Be(DateTimeKind.Utc);
        }

        [Theory]
        [InlineData(typeof (Stream))]
        [InlineData(typeof (string))]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type)
        {
            var target = new DateTimeValueGenerator();

            Action action = () => target.Generate(type, null, null);

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullTypeTest()
        {
            var target = new DateTimeValueGenerator();

            Action action = () => target.Generate(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof (string), false)]
        [InlineData(typeof (Stream), false)]
        [InlineData(typeof (TimeSpan), true)]
        [InlineData(typeof (TimeZoneInfo), true)]
        [InlineData(typeof (DateTimeOffset), true)]
        [InlineData(typeof (DateTime), true)]
        public void IsSupportedTest(Type type, bool expected)
        {
            var target = new DateTimeValueGenerator();

            var actual = target.IsSupported(type, null, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new DateTimeValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
namespace ModelBuilder.UnitTests
{
    using System;
    using System.IO;
    using FluentAssertions;
    using Xunit;

    public class DateTimeValueGeneratorTests
    {
        [Fact]
        public void GenerateAlwaysReturnsFutureValuesWithin10YearsTest()
        {
            var target = new DateTimeValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = (DateTime?)target.Generate(typeof(DateTime?), null, null);

                if (value == null)
                {
                    continue;
                }

                value.Should().BeAfter(DateTime.UtcNow);
                value.Should().BeBefore(DateTime.UtcNow.AddYears(10));
            }
        }

        [Fact]
        public void GenerateCanReturnNullAndRandomValuesTest()
        {
            var nullFound = false;
            var valueFound = false;

            var target = new DateTimeValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = (DateTime?)target.Generate(typeof(DateTime?), null, null);

                if (value == null)
                {
                    nullFound = true;
                }
                else
                {
                    valueFound = true;
                }

                if (nullFound && valueFound)
                {
                    break;
                }
            }

            nullFound.Should().BeTrue();
            valueFound.Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsRandomDateTimeOffsetValueTest()
        {
            var target = new DateTimeValueGenerator();

            var first = target.Generate(typeof(DateTimeOffset), null, null);

            first.Should().BeOfType<DateTimeOffset>();
            first.As<DateTimeOffset>().Offset.Should().Be(TimeSpan.Zero);

            var second = target.Generate(typeof(DateTimeOffset), null, null);

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomDateTimeValueTest()
        {
            var target = new DateTimeValueGenerator();

            var first = target.Generate(typeof(DateTime), null, null);

            first.Should().BeOfType<DateTime>();
            first.As<DateTime>().Kind.Should().Be(DateTimeKind.Utc);

            var second = target.Generate(typeof(DateTime), null, null);

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomTimeSpanValueTest()
        {
            var target = new DateTimeValueGenerator();

            var first = (TimeSpan)target.Generate(typeof(TimeSpan), null, null);
            var second = (TimeSpan)target.Generate(typeof(TimeSpan), null, null);

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomTimeZoneInfoValueTest()
        {
            var target = new DateTimeValueGenerator();

            var first = (TimeZoneInfo)target.Generate(typeof(TimeZoneInfo), null, null);
            var second = (TimeZoneInfo)target.Generate(typeof(TimeZoneInfo), null, null);

            first.Should().NotBe(second);
        }

        [Theory]
        [InlineData(typeof(Stream))]
        [InlineData(typeof(string))]
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
        [InlineData(typeof(string), false)]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(TimeSpan), true)]
        [InlineData(typeof(TimeSpan?), true)]
        [InlineData(typeof(TimeZoneInfo), true)]
        [InlineData(typeof(DateTimeOffset), true)]
        [InlineData(typeof(DateTimeOffset?), true)]
        [InlineData(typeof(DateTime), true)]
        [InlineData(typeof(DateTime?), true)]
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
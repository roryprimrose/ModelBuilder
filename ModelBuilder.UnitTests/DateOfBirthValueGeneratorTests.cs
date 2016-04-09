using System;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class DateOfBirthValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsDateTimeOffsetValueWithinLast100YearsTest()
        {
            var target = new DateOfBirthValueGenerator();

            var actual = target.Generate(typeof (DateTimeOffset), "dob", null);

            actual.Should().BeOfType<DateTimeOffset>();
            actual.As<DateTimeOffset>().Should().BeBefore(DateTimeOffset.UtcNow);
            actual.As<DateTimeOffset>().Should().BeAfter(DateTimeOffset.UtcNow.AddYears(-100));
            actual.As<DateTimeOffset>().Offset.Should().Be(TimeSpan.Zero);
        }

        [Fact]
        public void GenerateReturnsDateTimeValueWithinLast100YearsTest()
        {
            var target = new DateOfBirthValueGenerator();

            var actual = target.Generate(typeof (DateTime), "dob", null);

            actual.Should().BeOfType<DateTime>();
            actual.As<DateTime>().Should().BeBefore(DateTime.UtcNow);
            actual.As<DateTime>().Should().BeAfter(DateTime.UtcNow.AddYears(-100));
            actual.As<DateTime>().Kind.Should().Be(DateTimeKind.Utc);
        }

        [Theory]
        [InlineData(typeof (TimeSpan), "dob")]
        [InlineData(typeof (TimeZoneInfo), "dob")]
        [InlineData(typeof (string), "dob")]
        [InlineData(typeof (DateTime), null)]
        [InlineData(typeof (DateTime), "")]
        [InlineData(typeof (DateTime), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var target = new DateOfBirthValueGenerator();

            Action action = () => target.Generate(type, referenceName, null);

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullTypeTest()
        {
            var target = new DateOfBirthValueGenerator();

            Action action = () => target.Generate(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void HasHigherPriorityThanDateTimeValueGeneratorTest()
        {
            var target = new DateOfBirthValueGenerator();
            var other = new DateTimeValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Theory]
        [InlineData(typeof (TimeSpan), "dob", false)]
        [InlineData(typeof (TimeZoneInfo), "dob", false)]
        [InlineData(typeof (string), "dob", false)]
        [InlineData(typeof (DateTime), null, false)]
        [InlineData(typeof (DateTime), "", false)]
        [InlineData(typeof (DateTime), "Stuff", false)]
        [InlineData(typeof (DateTime), "dob", true)]
        [InlineData(typeof (DateTimeOffset), "dob", true)]
        [InlineData(typeof (DateTime), "DOB", true)]
        [InlineData(typeof (DateTime), "Dob", true)]
        [InlineData(typeof (DateTime), "Born", true)]
        [InlineData(typeof (DateTime), "born", true)]
        [InlineData(typeof (DateTime), "DateOfBirth", true)]
        [InlineData(typeof (DateTime), "dateofbirth", true)]
        public void IsSupportedTest(Type type, string referenceName, bool expected)
        {
            var target = new DateOfBirthValueGenerator();

            var actual = target.IsSupported(type, referenceName, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new DateOfBirthValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
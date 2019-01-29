namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class DateOfBirthValueGeneratorTests
    {
        [Fact]
        public void GenerateCanReturnNullAndRandomValuesTest()
        {
            var nullFound = false;
            var valueFound = false;

            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DateOfBirthValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = (DateTime?)target.Generate(typeof(DateTime?), "dob", executeStrategy);

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
        public void GenerateReturnsRandomDateTimeOffsetValueWithinLast100YearsTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DateOfBirthValueGenerator();

            var first = target.Generate(typeof(DateTimeOffset), "dob", executeStrategy);

            first.Should().BeOfType<DateTimeOffset>();
            first.As<DateTimeOffset>().Should().BeBefore(DateTimeOffset.UtcNow);
            first.As<DateTimeOffset>().Should().BeAfter(DateTimeOffset.UtcNow.AddYears(-100));
            first.As<DateTimeOffset>().Offset.Should().Be(TimeSpan.Zero);

            var second = target.Generate(typeof(DateTimeOffset), "dob", executeStrategy);

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomDateTimeValueWithinLast100YearsTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DateOfBirthValueGenerator();

            var first = target.Generate(typeof(DateTime), "dob", executeStrategy);

            first.Should().BeOfType<DateTime>();
            first.As<DateTime>().Should().BeBefore(DateTime.UtcNow);
            first.As<DateTime>().Should().BeAfter(DateTime.UtcNow.AddYears(-100));
            first.As<DateTime>().Kind.Should().Be(DateTimeKind.Utc);

            var second = target.Generate(typeof(DateTime), "dob", executeStrategy);

            first.Should().NotBe(second);
        }

        [Theory]
        [InlineData(typeof(TimeSpan), "dob")]
        [InlineData(typeof(TimeZoneInfo), "dob")]
        [InlineData(typeof(string), "dob")]
        [InlineData(typeof(DateTime), null)]
        [InlineData(typeof(DateTime), "")]
        [InlineData(typeof(DateTime), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DateOfBirthValueGenerator();

            Action action = () => target.Generate(type, referenceName, executeStrategy);

            action.Should().Throw<NotSupportedException>();
        }
        
        [Fact]
        public void HasHigherPriorityThanDateTimeValueGeneratorTest()
        {
            var target = new DateOfBirthValueGenerator();
            var other = new DateTimeValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Theory]
        [InlineData(typeof(TimeSpan), "dob", false)]
        [InlineData(typeof(TimeZoneInfo), "dob", false)]
        [InlineData(typeof(string), "dob", false)]
        [InlineData(typeof(DateTime), null, false)]
        [InlineData(typeof(DateTime), "", false)]
        [InlineData(typeof(DateTime), "Stuff", false)]
        [InlineData(typeof(DateTime), "dob", true)]
        [InlineData(typeof(DateTime?), "dob", true)]
        [InlineData(typeof(DateTimeOffset), "dob", true)]
        [InlineData(typeof(DateTimeOffset?), "dob", true)]
        [InlineData(typeof(DateTime), "DOB", true)]
        [InlineData(typeof(DateTime), "Dob", true)]
        [InlineData(typeof(DateTime), "Born", true)]
        [InlineData(typeof(DateTime), "born", true)]
        [InlineData(typeof(DateTime), "DateOfBirth", true)]
        [InlineData(typeof(DateTime), "dateofbirth", true)]
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

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
namespace ModelBuilder.UnitTests
{
    using System;
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

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DateOfBirthValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = (DateTime?) target.Generate(typeof(DateTime?), "dob", executeStrategy);

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
        public void GenerateReturnsDateTimeOffsetValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DateOfBirthValueGenerator();

            var actual = target.Generate(typeof(DateTimeOffset), "dob", executeStrategy);

            actual.Should().BeOfType<DateTimeOffset>();
        }

        [Fact]
        public void GenerateReturnsDateTimeValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DateOfBirthValueGenerator();

            var actual = target.Generate(typeof(DateTime), "dob", executeStrategy);

            actual.Should().BeOfType<DateTime>();
        }

        [Fact]
        public void GenerateReturnsRandomDateTimeOffsetValueWithinLast100YearsTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DateOfBirthValueGenerator();

            var first = (DateTimeOffset) target.Generate(typeof(DateTimeOffset), "dob", executeStrategy);

            first.As<DateTimeOffset>().Should().BeBefore(DateTimeOffset.UtcNow);
            first.As<DateTimeOffset>().Should().BeAfter(DateTimeOffset.UtcNow.AddYears(-100));
            first.As<DateTimeOffset>().Offset.Should().Be(TimeSpan.Zero);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (DateTimeOffset) target.Generate(typeof(DateTimeOffset), "dob", executeStrategy);

                if (first != second)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomDateTimeValueWithinLast100YearsTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DateOfBirthValueGenerator();

            var first = (DateTime) target.Generate(typeof(DateTime), "dob", executeStrategy);

            first.As<DateTime>().Should().BeBefore(DateTime.UtcNow);
            first.As<DateTime>().Should().BeAfter(DateTime.UtcNow.AddYears(-100));
            first.As<DateTime>().Kind.Should().Be(DateTimeKind.Utc);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (DateTime) target.Generate(typeof(DateTime), "dob", executeStrategy);

                if (first != second)
                {
                    break;
                }
            }

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
            var buildChain = new BuildHistory();
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
            var buildChain = Substitute.For<IBuildChain>();

            var target = new DateOfBirthValueGenerator();

            var actual = target.IsSupported(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new DateOfBirthValueGenerator();

            Action action = () => target.IsSupported(null, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
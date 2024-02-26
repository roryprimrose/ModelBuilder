namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class DateOfBirthValueGeneratorTests
    {
        [Fact]
        public void AllowNullReturnsFalseByDefault()
        {
            var sut = new DateOfBirthValueGenerator();

            sut.AllowNull.Should().BeFalse();
        }

        [Fact]
        public void GenerateReturnsDateTimeOffsetValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(DateTimeOffset), "dob", executeStrategy);

            actual.Should().BeOfType<DateTimeOffset>();
        }

        [Fact]
        public void GenerateReturnsDateTimeValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(DateTime), "dob", executeStrategy);

            actual.Should().BeOfType<DateTime>();
        }

        [Theory]
        [InlineData(false, 100, false)]
        [InlineData(true, 100, true)]
        [InlineData(true, 50, true)]
        [InlineData(true, 10, true)]
        [InlineData(true, 0, false)]
        public void GenerateReturnsNullBasedOnAllowNullAndPercentageChance(bool allowNull, int percentageChance,
            bool expected)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new DateOfBirthValueGenerator
            {
                AllowNull = allowNull,
                NullPercentageChance = percentageChance
            };

            var nullFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = (DateTime?)sut.Generate(executeStrategy, typeof(DateTime?));

                if (actual == null!)
                {
                    nullFound = true;
                    break;
                }
            }

            nullFound.Should().Be(expected);
        }

        [Fact]
        public void GenerateReturnsRandomDateTimeOffsetValueWithinLast100Years()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var first = (DateTimeOffset)sut.RunGenerate(typeof(DateTimeOffset), "dob", executeStrategy);

            first.As<DateTimeOffset>().Should().BeBefore(DateTimeOffset.UtcNow);
            first.As<DateTimeOffset>().Should().BeAfter(DateTimeOffset.UtcNow.AddYears(-100));
            first.As<DateTimeOffset>().Offset.Should().Be(TimeSpan.Zero);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (DateTimeOffset)sut.RunGenerate(typeof(DateTimeOffset), "dob", executeStrategy);

                if (first != second)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomDateTimeValueWithinLast100Years()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var first = (DateTime)sut.RunGenerate(typeof(DateTime), "dob", executeStrategy);

            first.As<DateTime>().Should().BeBefore(DateTime.UtcNow);
            first.As<DateTime>().Should().BeAfter(DateTime.UtcNow.AddYears(-100));
            first.As<DateTime>().Kind.Should().Be(DateTimeKind.Utc);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (DateTime)sut.RunGenerate(typeof(DateTime), "dob", executeStrategy);

                if (first != second)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void HasHigherPriorityThanDateTimeValueGenerator()
        {
            var sut = new Wrapper();
            var other = new DateTimeValueGenerator();

            sut.Priority.Should().BeGreaterThan(other.Priority);
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
        public void IsMatchReturnsWhetherTypeAndNameAreSupportedTest(Type type, string? referenceName, bool expected)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, referenceName!, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullType()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper();

            Action action = () => sut.RunIsMatch(null!, null!, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NullPercentageChanceReturns10ByDefault()
        {
            var sut = new DateOfBirthValueGenerator();

            sut.NullPercentageChance.Should().Be(10);
        }

        private class Wrapper : DateOfBirthValueGenerator
        {
            public object RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return Generate(executeStrategy, type, referenceName)!;
            }

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(buildChain, type, referenceName);
            }
        }
    }
}
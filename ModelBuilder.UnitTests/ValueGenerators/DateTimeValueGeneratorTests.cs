namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.IO;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class DateTimeValueGeneratorTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AllowNullDeterminesWhetherNullCanBeReturned(bool allowNull)
        {
            var nullFound = false;

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var buildChain = new BuildHistory();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new DateTimeValueGenerator
            {
                AllowNull = allowNull
            };

            for (var index = 0; index < 10000; index++)
            {
                var value = sut.Generate(executeStrategy, typeof(DateTime?));

                if (value == null)
                {
                    nullFound = true;

                    break;
                }
            }

            nullFound.Should().Be(allowNull);
        }

        [Fact]
        public void AllowNullReturnsFalseByDefault()
        {
            var sut = new DateTimeValueGenerator();

            sut.AllowNull.Should().BeFalse();
        }

        [Fact]
        public void GenerateAlwaysReturnsFutureValuesWithin10Years()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            for (var index = 0; index < 1000; index++)
            {
                var value = (DateTime?) sut.RunGenerate(typeof(DateTime?), null!, executeStrategy);

                value.Should().BeAfter(DateTime.UtcNow);
                value.Should().BeBefore(DateTime.UtcNow.AddYears(10));
            }
        }

        [Fact]
        public void GenerateReturnsDateTimeOffsetValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(DateTimeOffset), null!, executeStrategy);

            actual.Should().BeOfType<DateTimeOffset>();
        }

        [Fact]
        public void GenerateReturnsDateTimeValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(DateTime), null!, executeStrategy);

            actual.Should().BeOfType<DateTime>();
        }

        [Fact]
        public void GenerateReturnsRandomDateTimeOffsetValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var first = (DateTimeOffset) sut.RunGenerate(typeof(DateTimeOffset), null!, executeStrategy);

            first.As<DateTimeOffset>().Offset.Should().Be(TimeSpan.Zero);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (DateTimeOffset) sut.RunGenerate(typeof(DateTimeOffset), null!, executeStrategy);

                if (first != second)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomDateTimeValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var first = (DateTime) sut.RunGenerate(typeof(DateTime), null!, executeStrategy);

            first.As<DateTime>().Kind.Should().Be(DateTimeKind.Utc);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (DateTime) sut.RunGenerate(typeof(DateTime), null!, executeStrategy);

                if (first != second)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomTimeSpanValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var first = (TimeSpan) sut.RunGenerate(typeof(TimeSpan), null!, executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (TimeSpan) sut.RunGenerate(typeof(TimeSpan), null!, executeStrategy);

                if (first != second)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(TimeZoneInfo), false)]
        [InlineData(typeof(TimeSpan), true)]
        [InlineData(typeof(TimeSpan?), true)]
        [InlineData(typeof(DateTimeOffset), true)]
        [InlineData(typeof(DateTimeOffset?), true)]
        [InlineData(typeof(DateTime), true)]
        [InlineData(typeof(DateTime?), true)]
        public void IsMatchTest(Type type, bool expected)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, null!, buildChain);

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

        private class Wrapper : DateTimeValueGenerator
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
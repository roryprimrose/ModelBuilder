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
        [Fact]
        public void GenerateAlwaysReturnsFutureValuesWithin10YearsTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            for (var index = 0; index < 1000; index++)
            {
                var value = (DateTime?) target.RunGenerate(typeof(DateTime?), null, executeStrategy);

                if (value == null)
                {
                    continue;
                }

                value.Should().BeAfter(DateTime.UtcNow);
                value.Should().BeBefore(DateTime.UtcNow.AddYears(10));
            }
        }

        [Theory]
        [InlineData(typeof(DateTime?))]
        [InlineData(typeof(DateTimeOffset?))]
        [InlineData(typeof(TimeSpan?))]
        public void GenerateCanReturnNullAndRandomValuesTest(Type targetType)
        {
            var nullFound = false;
            var valueFound = false;

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.RunGenerate(targetType, null, executeStrategy);

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

            var target = new Wrapper();

            var actual = target.RunGenerate(typeof(DateTimeOffset), null, executeStrategy);

            actual.Should().BeOfType<DateTimeOffset>();
        }

        [Fact]
        public void GenerateReturnsDateTimeValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var actual = target.RunGenerate(typeof(DateTime), null, executeStrategy);

            actual.Should().BeOfType<DateTime>();
        }

        [Fact]
        public void GenerateReturnsRandomDateTimeOffsetValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var first = (DateTimeOffset) target.RunGenerate(typeof(DateTimeOffset), null, executeStrategy);

            first.As<DateTimeOffset>().Offset.Should().Be(TimeSpan.Zero);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (DateTimeOffset) target.RunGenerate(typeof(DateTimeOffset), null, executeStrategy);

                if (first != second)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomDateTimeValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var first = (DateTime) target.RunGenerate(typeof(DateTime), null, executeStrategy);

            first.As<DateTime>().Kind.Should().Be(DateTimeKind.Utc);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (DateTime) target.RunGenerate(typeof(DateTime), null, executeStrategy);

                if (first != second)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomTimeSpanValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var first = (TimeSpan) target.RunGenerate(typeof(TimeSpan), null, executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (TimeSpan) target.RunGenerate(typeof(TimeSpan), null, executeStrategy);

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

            var target = new Wrapper();

            var actual = target.RunIsMatch(type, null, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullTypeTest()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

            Action action = () => target.RunIsMatch(null, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : DateTimeValueGenerator
        {
            public object RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return Generate(executeStrategy, type, referenceName);
            }

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(buildChain, type, referenceName);
            }
        }
    }
}
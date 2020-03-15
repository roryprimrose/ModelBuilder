﻿namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class DateOfBirthValueGeneratorTests
    {
        [Fact]
        public void GenerateCanReturnNullAndRandomValues()
        {
            var nullFound = false;
            var valueFound = false;

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            for (var index = 0; index < 1000; index++)
            {
                var value = (DateTime?) target.RunGenerate(typeof(DateTime?), "dob", executeStrategy);

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
        public void GenerateReturnsDateTimeOffsetValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var actual = target.RunGenerate(typeof(DateTimeOffset), "dob", executeStrategy);

            actual.Should().BeOfType<DateTimeOffset>();
        }

        [Fact]
        public void GenerateReturnsDateTimeValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var actual = target.RunGenerate(typeof(DateTime), "dob", executeStrategy);

            actual.Should().BeOfType<DateTime>();
        }

        [Fact]
        public void GenerateReturnsRandomDateTimeOffsetValueWithinLast100Years()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var first = (DateTimeOffset) target.RunGenerate(typeof(DateTimeOffset), "dob", executeStrategy);

            first.As<DateTimeOffset>().Should().BeBefore(DateTimeOffset.UtcNow);
            first.As<DateTimeOffset>().Should().BeAfter(DateTimeOffset.UtcNow.AddYears(-100));
            first.As<DateTimeOffset>().Offset.Should().Be(TimeSpan.Zero);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (DateTimeOffset) target.RunGenerate(typeof(DateTimeOffset), "dob", executeStrategy);

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

            var target = new Wrapper();

            var first = (DateTime) target.RunGenerate(typeof(DateTime), "dob", executeStrategy);

            first.As<DateTime>().Should().BeBefore(DateTime.UtcNow);
            first.As<DateTime>().Should().BeAfter(DateTime.UtcNow.AddYears(-100));
            first.As<DateTime>().Kind.Should().Be(DateTimeKind.Utc);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (DateTime) target.RunGenerate(typeof(DateTime), "dob", executeStrategy);

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
            var target = new Wrapper();
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
        public void IsMatchReturnsWhetherTypeAndNameAreSupportedTest(Type type, string referenceName, bool expected)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

            var actual = target.RunIsMatch(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullType()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

            Action action = () => target.RunIsMatch(null, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : DateOfBirthValueGenerator
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
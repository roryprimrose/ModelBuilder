﻿namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class DateTimeValueGeneratorTests
    {
        [Fact]
        public void GenerateAlwaysReturnsFutureValuesWithin10YearsTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DateTimeValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = (DateTime?)target.Generate(typeof(DateTime?), null, executeStrategy);

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

            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DateTimeValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.Generate(targetType, null, executeStrategy);

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
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DateTimeValueGenerator();

            var first = target.Generate(typeof(DateTimeOffset), null, executeStrategy);

            first.Should().BeOfType<DateTimeOffset>();
            first.As<DateTimeOffset>().Offset.Should().Be(TimeSpan.Zero);

            var second = target.Generate(typeof(DateTimeOffset), null, executeStrategy);

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomDateTimeValueTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DateTimeValueGenerator();

            var first = target.Generate(typeof(DateTime), null, executeStrategy);

            first.Should().BeOfType<DateTime>();
            first.As<DateTime>().Kind.Should().Be(DateTimeKind.Utc);

            var second = target.Generate(typeof(DateTime), null, executeStrategy);

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomTimeSpanValueTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DateTimeValueGenerator();

            var first = (TimeSpan)target.Generate(typeof(TimeSpan), null, executeStrategy);
            var second = (TimeSpan)target.Generate(typeof(TimeSpan), null, executeStrategy);

            first.Should().NotBe(second);
        }
        
        [Theory]
        [InlineData(typeof(Stream))]
        [InlineData(typeof(string))]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type)
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new DateTimeValueGenerator();

            Action action = () => target.Generate(type, null, executeStrategy);

            action.ShouldThrow<NotSupportedException>();
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
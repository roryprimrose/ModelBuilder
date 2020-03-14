﻿namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.IO;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class TimeZoneInfoValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomTimeZoneInfoValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            var first = (TimeZoneInfo) target.RunGenerate(typeof(TimeZoneInfo), null, executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (TimeZoneInfo) target.RunGenerate(typeof(TimeZoneInfo), null, executeStrategy);

                if (first.Equals(second) == false)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(TimeSpan), false)]
        [InlineData(typeof(TimeSpan?), false)]
        [InlineData(typeof(DateTimeOffset), false)]
        [InlineData(typeof(DateTimeOffset?), false)]
        [InlineData(typeof(DateTime), false)]
        [InlineData(typeof(DateTime?), false)]
        [InlineData(typeof(TimeZoneInfo), true)]
        public void IsMatchTest(Type type, bool expected)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

            var actual = target.RunIsMatch(type, null, buildChain);

            actual.Should().Be(expected);
        }

        private class Wrapper : TimeZoneInfoValueGenerator
        {
            public object RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return Generate(type, referenceName, executeStrategy);
            }

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(type, referenceName, buildChain);
            }
        }
    }
}
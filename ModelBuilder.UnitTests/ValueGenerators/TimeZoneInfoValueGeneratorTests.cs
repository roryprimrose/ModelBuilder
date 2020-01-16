namespace ModelBuilder.UnitTests.ValueGenerators
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

            var target = new TimeZoneInfoValueGenerator();

            var first = (TimeZoneInfo) target.Generate(typeof(TimeZoneInfo), null, executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (TimeZoneInfo) target.Generate(typeof(TimeZoneInfo), null, executeStrategy);

                if (first.Equals(second) == false)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Theory]
        [InlineData(typeof(Stream))]
        [InlineData(typeof(string))]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TimeZoneInfoValueGenerator();

            Action action = () => target.Generate(type, null, executeStrategy);

            action.Should().Throw<NotSupportedException>();
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
        public void IsSupportedTest(Type type, bool expected)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new TimeZoneInfoValueGenerator();

            var actual = target.IsSupported(type, null, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new TimeZoneInfoValueGenerator();

            Action action = () => target.IsSupported(null, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
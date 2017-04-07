namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class TimeZoneValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomValueMatchingCountryTest()
        {
            var source = new Address
            {
                Country = "Australia"
            };
            var buildChain = new LinkedList<object>();
            var resolver = new DefaultPropertyResolver();

            var configuration = Substitute.For<IBuildConfiguration>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            configuration.PropertyResolver.Returns(resolver);
            executeStrategy.Configuration.Returns(configuration);
            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(source);

            var target = new TimeZoneValueGenerator();

            var actual = (string)target.Generate(typeof(string), "timezone", executeStrategy);

            actual.Should().StartWith("Australia/");
        }

        [Fact]
        public void GenerateReturnsRandomValueWhenNoMatchingCountryFoundTest()
        {
            var source = new Address
            {
                Country = Guid.NewGuid().ToString()
            };
            var buildChain = new LinkedList<object>();
            var resolver = new DefaultPropertyResolver();

            var configuration = Substitute.For<IBuildConfiguration>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            configuration.PropertyResolver.Returns(resolver);
            executeStrategy.Configuration.Returns(configuration);
            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.AddFirst(source);

            var target = new TimeZoneValueGenerator();

            var first = (string)target.Generate(typeof(string), "timezone", executeStrategy);
            var second = (string)target.Generate(typeof(string), "timezone", executeStrategy);

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomValueTest()
        {
            var buildChain = new LinkedList<object>();
            var resolver = new DefaultPropertyResolver();

            var configuration = Substitute.For<IBuildConfiguration>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            configuration.PropertyResolver.Returns(resolver);
            executeStrategy.Configuration.Returns(configuration);
            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TimeZoneValueGenerator();

            var first = target.Generate(typeof(string), "timezone", executeStrategy);

            first.Should().BeOfType<string>();
            first.As<string>().Should().NotBeNullOrWhiteSpace();

            var second = target.Generate(typeof(string), "timezone", executeStrategy);

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsValueForTimeZoneTypeTest()
        {
            var buildChain = new LinkedList<object>();
            var resolver = new DefaultPropertyResolver();

            var configuration = Substitute.For<IBuildConfiguration>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            configuration.PropertyResolver.Returns(resolver);
            executeStrategy.Configuration.Returns(configuration);
            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TimeZoneValueGenerator();

            var actual = target.Generate(typeof(string), "TimeZone", executeStrategy);

            actual.Should().BeOfType<string>();
        }

        [Theory]
        [InlineData(typeof(string), "timezone", true)]
        [InlineData(typeof(string), "TimeZone", true)]
        [InlineData(typeof(string), "timeZone", true)]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName, bool expected)
        {
            var buildChain = new LinkedList<object>();
            var resolver = new DefaultPropertyResolver();

            var configuration = Substitute.For<IBuildConfiguration>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            configuration.PropertyResolver.Returns(resolver);
            executeStrategy.Configuration.Returns(configuration);
            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TimeZoneValueGenerator();

            var actual = (string)target.Generate(type, referenceName, executeStrategy);

            actual.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(typeof(Stream), "timezone")]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(string), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var buildChain = new LinkedList<object>();
            var resolver = new DefaultPropertyResolver();

            var configuration = Substitute.For<IBuildConfiguration>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            configuration.PropertyResolver.Returns(resolver);
            executeStrategy.Configuration.Returns(configuration);
            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TimeZoneValueGenerator();

            Action action = () => target.Generate(type, referenceName, executeStrategy);

            action.ShouldThrow<NotSupportedException>();
        }
        
        [Theory]
        [InlineData(typeof(Stream), "timezone", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "timezone", true)]
        [InlineData(typeof(string), "TimeZone", true)]
        [InlineData(typeof(string), "timeZone", true)]
        public void IsSupportedTest(Type type, string referenceName, bool expected)
        {
            var target = new TimeZoneValueGenerator();

            var actual = target.IsSupported(type, referenceName, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new TimeZoneValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsPositiveValueTest()
        {
            var target = new TimeZoneValueGenerator();

            target.Priority.Should().BeGreaterThan(0);
        }
    }
}
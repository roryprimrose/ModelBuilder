namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class CultureValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomCultureTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new CultureValueGenerator();

            var first = target.Generate(typeof(string), "culture", executeStrategy);

            first.Should().BeOfType<string>();
            first.As<string>().Should().NotBeNullOrWhiteSpace();

            var otherValueFound = false;

            for (var index = 0; index < 100; index++)
            {
                var second = target.Generate(typeof(string), "culture", executeStrategy);

                if (first != second)
                {
                    otherValueFound = true;

                    break;
                }
            }

            otherValueFound.Should().BeTrue();
        }

#if NET452
        [Fact]
        public void GenerateReturnsRandomCultureInfoTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new CultureValueGenerator();

            var first = target.Generate(typeof(CultureInfo), "culture", executeStrategy);

            first.Should().BeOfType<CultureInfo>();
            first.As<CultureInfo>().Should().NotBeNull();

            var otherValueFound = false;

            for (var index = 0; index < 100; index++)
            {
                var second = target.Generate(typeof(CultureInfo), "culture", executeStrategy);

                if (first.As<CultureInfo>().Name != second.As<CultureInfo>().Name)
                {
                    otherValueFound = true;

                    break;
                }
            }

            otherValueFound.Should().BeTrue();
        }
#else
        [Fact]
        public void GenerateReturnsCurrentUICultureTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new CultureValueGenerator();

            var first = target.Generate(typeof(CultureInfo), "culture", executeStrategy);

            first.Should().BeOfType<CultureInfo>();
            first.As<CultureInfo>().Should().NotBeNull();

            var otherValueFound = false;

            for (var index = 0; index < 100; index++)
            {
                var second = target.Generate(typeof(CultureInfo), "culture", executeStrategy);

                if (first.As<CultureInfo>().Name != second.As<CultureInfo>().Name)
                {
                    otherValueFound = true;

                    break;
                }
            }

            // netstandard 1.5 will always return the CurrentUI CultureInfo
            otherValueFound.Should().BeFalse();
        }
#endif

        [Theory]
        [InlineData(typeof(string), "culture")]
        [InlineData(typeof(string), "Culture")]
        [InlineData(typeof(string), "culturename")]
        [InlineData(typeof(string), "Culturename")]
        [InlineData(typeof(string), "cultureName")]
        [InlineData(typeof(string), "CultureName")]
        [InlineData(typeof(CultureInfo), "culture")]
        [InlineData(typeof(CultureInfo), "Culture")]
        [InlineData(typeof(CultureInfo), "culturename")]
        [InlineData(typeof(CultureInfo), "Culturename")]
        [InlineData(typeof(CultureInfo), "cultureName")]
        [InlineData(typeof(CultureInfo), "CultureName")]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName)
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new CultureValueGenerator();

            var actual = target.Generate(type, referenceName, executeStrategy);

            if (type == typeof(string))
            {
                actual.As<string>().Should().NotBeNullOrEmpty();
            }
            else
            {
                actual.Should().NotBeNull();
            }
        }

        [Theory]
        [InlineData(typeof(Stream), "culture")]
        [InlineData(typeof(Stream), "Culture")]
        [InlineData(typeof(Stream), "culturename")]
        [InlineData(typeof(Stream), "Culturename")]
        [InlineData(typeof(Stream), "cultureName")]
        [InlineData(typeof(Stream), "CultureName")]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(string), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new CultureValueGenerator();

            Action action = () => target.Generate(type, referenceName, executeStrategy);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void HasHigherPriorityThanStringValueGeneratorTest()
        {
            var target = new CultureValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Theory]
        [InlineData(typeof(Stream), "culture", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "culture", true)]
        [InlineData(typeof(string), "Culture", true)]
        [InlineData(typeof(string), "culturename", true)]
        [InlineData(typeof(string), "Culturename", true)]
        [InlineData(typeof(string), "cultureName", true)]
        [InlineData(typeof(string), "CultureName", true)]
        [InlineData(typeof(CultureInfo), "culture", true)]
        [InlineData(typeof(CultureInfo), "Culture", true)]
        [InlineData(typeof(CultureInfo), "culturename", true)]
        [InlineData(typeof(CultureInfo), "Culturename", true)]
        [InlineData(typeof(CultureInfo), "cultureName", true)]
        [InlineData(typeof(CultureInfo), "CultureName", true)]
        public void IsSupportedTest(Type type, string referenceName, bool expected)
        {
            var target = new CultureValueGenerator();

            var actual = target.IsSupported(type, referenceName, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new CultureValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
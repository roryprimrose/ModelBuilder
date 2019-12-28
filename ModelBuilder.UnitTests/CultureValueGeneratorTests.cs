namespace ModelBuilder.UnitTests
{
    using System;
    using System.Globalization;
    using System.IO;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class CultureValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomStringValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new CultureValueGenerator();

            var first = (string) target.Generate(typeof(string), "culture", executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (string) target.Generate(typeof(string), "culture", executeStrategy);

                if (string.Equals(first, second, StringComparison.OrdinalIgnoreCase) == false)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsStringValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new CultureValueGenerator();

            var actual = target.Generate(typeof(string), "culture", executeStrategy);

            actual.Should().BeOfType<string>();
            actual.As<string>().Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void GenerateReturnsCultureInfoValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new CultureValueGenerator();

            var actual = target.Generate(typeof(CultureInfo), "culture", executeStrategy);

            actual.Should().BeOfType<CultureInfo>();
            actual.As<CultureInfo>().Should().NotBeNull();
        }

        [Fact]
        public void GenerateReturnsRandomCultureInfoTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new CultureValueGenerator();

            var first = (CultureInfo)target.Generate(typeof(CultureInfo), "culture", executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (CultureInfo)target.Generate(typeof(CultureInfo), "culture", executeStrategy);

                if (string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) == false)
                {
                    break;
                }
            }
        
            first.Should().NotBe(second);
        }

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
            var buildChain = new BuildHistory();
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
            var buildChain = new BuildHistory();
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
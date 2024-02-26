namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.IO;
    using FluentAssertions;
    using ModelBuilder.Data;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class DomainNameValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomDomainFromDefinedList()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(string), "domain", executeStrategy) as string;

            TestData.Domains.Should().Contain(actual);
        }

        [Fact]
        public void GenerateReturnsRandomValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var first = (string) sut.RunGenerate(typeof(string), "domain", executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (string) sut.RunGenerate(typeof(string), "domain", executeStrategy);

                if (string.Equals(first, second, StringComparison.OrdinalIgnoreCase) == false)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsString()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(string), "domain", executeStrategy);

            actual.Should().BeOfType<string>();
            actual.As<string>().Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(typeof(string), "domain")]
        [InlineData(typeof(string), "Domain")]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = (string) sut.RunGenerate(type, referenceName, executeStrategy);

            actual.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void HasHigherPriorityThanStringValueGenerator()
        {
            var sut = new Wrapper();
            var other = new StringValueGenerator();

            sut.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Theory]
        [InlineData(typeof(Stream), "domain", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "domain", true)]
        [InlineData(typeof(string), "Domain", true)]
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

        private class Wrapper : DomainNameValueGenerator
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
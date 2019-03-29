namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class StringValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new StringValueGenerator();

            var first = (string) target.Generate(typeof(string), null, executeStrategy);

            var second = first;

            for (var index = 0; index < 100000; index++)
            {
                second = (string) target.Generate(typeof(string), null, executeStrategy);

                if (string.Equals(first, second, StringComparison.OrdinalIgnoreCase) == false)
                {
                    break;
                }
            }

            first.Should().NotBeNull();
            second.Should().NotBeNull();
            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsStringValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new StringValueGenerator();

            var actual = target.Generate(typeof(string), null, executeStrategy);

            actual.Should().BeOfType<string>();
            actual.As<string>().Should().NotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData(typeof(bool), false)]
        [InlineData(typeof(string), true)]
        public void GenerateReturnsValidatesTypeSupportTest(Type type, bool supported)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new StringValueGenerator();

            Action action = () => target.Generate(type, null, executeStrategy);

            if (supported)
            {
                action.Should().NotThrow();
            }
            else
            {
                action.Should().Throw<NotSupportedException>();
            }
        }

        [Theory]
        [InlineData(typeof(bool), false)]
        [InlineData(typeof(string), true)]
        public void IsSupportedReturnsWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var target = new StringValueGenerator();

            var actual = target.IsSupported(type, null, null);

            actual.Should().Be(supported);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new StringValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
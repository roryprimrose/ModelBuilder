namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class GuidValueGeneratorTests
    {
        [Fact]
        public void GenerateCanReturnNullAndRandomValuesTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var nullFound = false;
            var valueFound = false;

            var target = new GuidValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = (Guid?)target.Generate(typeof(Guid?), null, executeStrategy);

                if (value == null)
                {
                    nullFound = true;
                }
                else if (value.Value != Guid.Empty)
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

        [Theory]
        [InlineData(typeof(Guid), true)]
        [InlineData(typeof(Guid?), true)]
        [InlineData(typeof(string), false)]
        public void GenerateEvaluatesWhetherTypeIsSupportedTest(Type type, bool supportedType)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new GuidValueGenerator();

            Action action = () => target.Generate(type, null, executeStrategy);

            if (supportedType)
            {
                action.Should().NotThrow();
            }
            else
            {
                action.Should().Throw<NotSupportedException>();
            }
        }

        [Fact]
        public void GenerateReturnsGuidValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new GuidValueGenerator();

            var actual = target.Generate(typeof(Guid), null, executeStrategy);

            actual.Should().BeOfType<Guid>();
            actual.As<Guid>().Should().NotBeEmpty();
        }

        [Fact]
        public void GenerateReturnsRandomValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new GuidValueGenerator();

            var first = (Guid)target.Generate(typeof(Guid), null, executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (Guid)target.Generate(typeof(Guid), null, executeStrategy);

                if (first != second)
                {
                    break;
                }
            }

            first.Should().NotBeEmpty();
            second.Should().NotBeEmpty();
            first.Should().NotBe(second);
        }

        [Theory]
        [InlineData(typeof(Guid), true)]
        [InlineData(typeof(Guid?), true)]
        [InlineData(typeof(string), false)]
        public void IsSupportedReturnsWhetherTypeIsSupportedTest(Type type, bool supportedType)
        {
            var target = new GuidValueGenerator();

            var actual = target.IsSupported(type, null, null);

            actual.Should().Be(supportedType);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var buildChain = new BuildHistory();

            buildChain.Push(Guid.NewGuid().ToString());

            var target = new GuidValueGenerator();

            Action action = () => target.IsSupported(null, Guid.NewGuid().ToString(), buildChain);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
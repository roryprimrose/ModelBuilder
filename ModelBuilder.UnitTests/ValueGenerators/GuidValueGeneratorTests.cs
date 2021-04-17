namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class GuidValueGeneratorTests
    {
        [Fact]
        public void AllowNullReturnsFalseByDefault()
        {
            var sut = new GuidValueGenerator();

            sut.AllowNull.Should().BeFalse();
        }

        [Fact]
        public void GenerateCanReturnNullAndRandomValues()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var nullFound = false;
            var valueFound = false;

            var sut = new GuidValueGenerator
            {
                AllowNull = true
            };

            for (var index = 0; index < 1000; index++)
            {
                var value = (Guid?) sut.Generate(executeStrategy, typeof(Guid?));

                if (value == null!)
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

        [Fact]
        public void GenerateReturnsGuidValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(Guid), null!, executeStrategy);

            actual.Should().BeOfType<Guid>();
            actual.As<Guid>().Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(false, 100, false)]
        [InlineData(true, 100, true)]
        [InlineData(true, 50, true)]
        [InlineData(true, 10, true)]
        [InlineData(true, 0, false)]
        public void GenerateReturnsNullBasedOnAllowNullAndPercentageChance(bool allowNull, int percentageChance,
            bool expected)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new GuidValueGenerator
            {
                AllowNull = allowNull,
                NullPercentageChance = percentageChance
            };

            var nullFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = (Guid?) sut.Generate(executeStrategy, typeof(Guid?));

                if (actual == null!)
                {
                    nullFound = true;
                    break;
                }
            }

            nullFound.Should().Be(expected);
        }

        [Fact]
        public void GenerateReturnsRandomValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var first = (Guid) sut.RunGenerate(typeof(Guid), null!, executeStrategy);

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = (Guid) sut.RunGenerate(typeof(Guid), null!, executeStrategy);

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
        public void IsMatchReturnsWhetherTypeIsSupportedTest(Type type, bool supportedType)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, null!, buildChain);

            actual.Should().Be(supportedType);
        }

        [Fact]
        public void NullPercentageChanceReturns10ByDefault()
        {
            var sut = new GuidValueGenerator();

            sut.NullPercentageChance.Should().Be(10);
        }

        private class Wrapper : GuidValueGenerator
        {
            public object RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return Generate(executeStrategy, type, referenceName)!;
            }

            public object RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(buildChain, type, referenceName);
            }
        }
    }
}
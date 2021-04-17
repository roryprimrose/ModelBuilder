namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.IO;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class EnumValueGeneratorTests
    {
        [Fact]
        public void AllowNullReturnsFalseByDefault()
        {
            var sut = new EnumValueGenerator();

            sut.AllowNull.Should().BeFalse();
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

            var sut = new EnumValueGenerator
            {
                AllowNull = allowNull,
                NullPercentageChance = percentageChance
            };

            var nullFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = (FileAttributes?) sut.Generate(executeStrategy, typeof(FileAttributes?));

                if (actual == null!)
                {
                    nullFound = true;
                    break;
                }
            }

            nullFound.Should().Be(expected);
        }

        [Fact]
        public void GenerateReturnsOnlyAvailableEnumValueWhenSingleValueDefined()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(SingleEnum), null!, executeStrategy);

            actual.Should().BeOfType<SingleEnum>();
            actual.Should().Be(SingleEnum.First);
        }

        [Fact]
        public void GenerateReturnsRandomFileAttributesValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var first = sut.RunGenerate(typeof(FileAttributes), null!, executeStrategy);

            first.Should().BeOfType<FileAttributes>();

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = sut.RunGenerate(typeof(FileAttributes), null!, executeStrategy);

                if (first != second)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomFlagsEnumValue()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var first = sut.RunGenerate(typeof(BigValues), null!, executeStrategy);

            first.Should().BeOfType<BigValues>();

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = sut.RunGenerate(typeof(BigValues), null!, executeStrategy);

                if (first != second)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomValueWhenTypeIsEnum()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var first = sut.RunGenerate(typeof(BigEnum), null!, executeStrategy);

            first.Should().BeOfType<BigEnum>();
            Enum.IsDefined(typeof(BigEnum), first).Should().BeTrue();

            var otherValueFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var second = sut.RunGenerate(typeof(BigEnum), null!, executeStrategy);

                if (first != second)
                {
                    otherValueFound = true;

                    break;
                }
            }

            otherValueFound.Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsSmallFlagsEnum()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();
            var first = false;
            var second = false;
            var third = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = (SmallFlags) sut.RunGenerate(typeof(SmallFlags), null!, executeStrategy);

                if (actual == SmallFlags.First)
                {
                    first = true;
                }
                else if (actual == SmallFlags.Second)
                {
                    second = true;
                }
                else if (actual == (SmallFlags.First | SmallFlags.Second))
                {
                    third = true;
                }

                if (first
                    && second
                    && third)
                {
                    break;
                }
            }

            first.Should().BeTrue();
            second.Should().BeTrue();
            third.Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsZeroForEmptyEnum()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            var actual = sut.RunGenerate(typeof(NoValues), null!, executeStrategy);

            actual.Should().BeOfType<NoValues>();
            actual.Should().Be((NoValues) 0);
        }

        [Theory]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(SimpleEnum), true)]
        [InlineData(typeof(SimpleEnum?), true)]
        [InlineData(typeof(BigEnum), true)]
        [InlineData(typeof(BigEnum?), true)]
        [InlineData(typeof(BigValues), true)]
        [InlineData(typeof(BigValues?), true)]
        public void IsMatchTest(Type type, bool expected)
        {
            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, null!, null!);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullType()
        {
            var sut = new Wrapper();

            Action action = () => sut.RunIsMatch(null!, null!, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NullPercentageChanceReturns10ByDefault()
        {
            var sut = new EnumValueGenerator();

            sut.NullPercentageChance.Should().Be(10);
        }

        private class Wrapper : EnumValueGenerator
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
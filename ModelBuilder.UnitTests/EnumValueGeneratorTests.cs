namespace ModelBuilder.UnitTests
{
    using System;
    using System.IO;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class EnumValueGeneratorTests
    {
        [Fact]
        public void GenerateCanReturnNullAndRandomValuesTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var nullFound = false;
            var valueFound = false;

            var target = new EnumValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = (SingleEnum?) target.Generate(typeof(SingleEnum?), null, executeStrategy);

                if (value == null)
                {
                    nullFound = true;
                }
                else
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
        public void GenerateReturnsOnlyAvailableEnumValueWhenSingleValueDefinedTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new EnumValueGenerator();

            var actual = target.Generate(typeof(SingleEnum), null, executeStrategy);

            actual.Should().BeOfType<SingleEnum>();
            actual.Should().Be(SingleEnum.First);
        }

        [Fact]
        public void GenerateReturnsRandomFileAttributesValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new EnumValueGenerator();

            var first = target.Generate(typeof(FileAttributes), null, executeStrategy);

            first.Should().BeOfType<FileAttributes>();

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = target.Generate(typeof(FileAttributes), null, executeStrategy);

                if (first != second)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomFlagsEnumValueTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new EnumValueGenerator();

            var first = target.Generate(typeof(BigValues), null, executeStrategy);

            first.Should().BeOfType<BigValues>();

            // Validate that the flags enum value has multiple values
            first.ToString().Should().Contain(", ");

            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = target.Generate(typeof(BigValues), null, executeStrategy);

                if (first != second)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomValueWhenTypeIsEnumTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new EnumValueGenerator();

            var first = target.Generate(typeof(BigEnum), null, executeStrategy);

            first.Should().BeOfType<BigEnum>();
            Enum.IsDefined(typeof(BigEnum), first).Should().BeTrue();

            var otherValueFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var second = target.Generate(typeof(BigEnum), null, executeStrategy);

                if (first != second)
                {
                    otherValueFound = true;

                    break;
                }
            }

            otherValueFound.Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsSmallFlagsEnumTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new EnumValueGenerator();
            var first = false;
            var second = false;
            var third = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = (SmallFlags) target.Generate(typeof(SmallFlags), null, executeStrategy);

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
        public void GenerateReturnsZeroForEmptyEnumTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new EnumValueGenerator();

            var actual = target.Generate(typeof(NoValues), null, executeStrategy);

            actual.Should().BeOfType<NoValues>();
            actual.Should().Be((NoValues) 0);
        }

        [Theory]
        [InlineData(typeof(Stream))]
        [InlineData(typeof(string))]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new EnumValueGenerator();

            Action action = () => target.Generate(type, null, executeStrategy);

            action.Should().Throw<NotSupportedException>();
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
        public void IsSupportedTest(Type type, bool expected)
        {
            var target = new EnumValueGenerator();

            var actual = target.IsSupported(type, null, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new EnumValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
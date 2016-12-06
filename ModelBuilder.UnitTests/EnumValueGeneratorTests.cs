namespace ModelBuilder.UnitTests
{
    using System;
    using System.IO;
    using FluentAssertions;
    using Xunit;

    public class EnumValueGeneratorTests
    {
        [Fact]
        public void GenerateCanReturnNullAndRandomValuesTest()
        {
            var nullFound = false;
            var valueFound = false;

            var target = new EnumValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = (SingleEnum?) target.Generate(typeof(SingleEnum?), null, null);

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
            var target = new EnumValueGenerator();

            var first = target.Generate(typeof(SingleEnum), null, null);

            first.Should().BeOfType<SingleEnum>();
            first.Should().Be(SingleEnum.First);
        }

        [Fact]
        public void GenerateReturnsRandomFileAttributesValueTest()
        {
            var target = new EnumValueGenerator();

            var first = target.Generate(typeof(FileAttributes), null, null);

            first.Should().BeOfType<FileAttributes>();

            var second = target.Generate(typeof(FileAttributes), null, null);

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomFlagsEnumValueTest()
        {
            var target = new EnumValueGenerator();

            var first = target.Generate(typeof(BigFlagsEnum), null, null);

            first.Should().BeOfType<BigFlagsEnum>();

            var values = Enum.GetValues(typeof(BigFlagsEnum));

            values.Should().NotContain(first);

            var second = target.Generate(typeof(BigFlagsEnum), null, null);

            first.Should().NotBe(second);
        }

        [Fact]
        public void GenerateReturnsRandomValueWhenTypeIsEnumTest()
        {
            var target = new EnumValueGenerator();

            var first = target.Generate(typeof(BigEnum), null, null);

            first.Should().BeOfType<BigEnum>();
            Enum.IsDefined(typeof(BigEnum), first).Should().BeTrue();

            var otherValueFound = false;

            for (var index = 0; index < 50; index++)
            {
                var second = target.Generate(typeof(BigEnum), null, null);

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
            var target = new EnumValueGenerator();
            var first = false;
            var second = false;
            var third = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = (SmallFlags) target.Generate(typeof(SmallFlags), null, null);

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

                if (first &&
                    second &&
                    third)
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
            var target = new EnumValueGenerator();

            var first = target.Generate(typeof(EmptyEnum), null, null);

            first.Should().BeOfType<EmptyEnum>();
            first.Should().Be((EmptyEnum) 0);
        }

        [Theory]
        [InlineData(typeof(Stream))]
        [InlineData(typeof(string))]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type)
        {
            var target = new EnumValueGenerator();

            Action action = () => target.Generate(type, null, null);

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullTypeTest()
        {
            var target = new EnumValueGenerator();

            Action action = () => target.Generate(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(SimpleEnum), true)]
        [InlineData(typeof(SimpleEnum?), true)]
        [InlineData(typeof(BigEnum), true)]
        [InlineData(typeof(BigEnum?), true)]
        [InlineData(typeof(BigFlagsEnum), true)]
        [InlineData(typeof(BigFlagsEnum?), true)]
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

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
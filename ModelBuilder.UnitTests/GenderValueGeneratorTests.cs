using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class GenderValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomValuesForGenderTypeTest()
        {
            var target = new GenderValueGenerator();

            var maleFound = false;
            var femaleFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = (string) target.Generate(typeof(string), "Gender", null);

                if (actual == "Male")
                {
                    maleFound = true;
                }
                else
                {
                    femaleFound = true;
                }

                if (maleFound && femaleFound)
                {
                    break;
                }
            }

            maleFound.Should().BeTrue();
            femaleFound.Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsValueForGenderTypeTest()
        {
            var target = new GenderValueGenerator();

            var actual = target.Generate(typeof(string), "Sex", null);

            actual.Should().BeOfType<string>();
        }

        [Theory]
        [InlineData(typeof(string), "gender", true)]
        [InlineData(typeof(string), "Gender", true)]
        [InlineData(typeof(string), "sex", true)]
        [InlineData(typeof(string), "Sex", true)]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName, bool expected)
        {
            var target = new GenderValueGenerator();

            var actual = (string) target.Generate(type, referenceName, null);

            actual.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(typeof(Stream), "gender")]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(string), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var target = new GenderValueGenerator();

            Action action = () => target.Generate(type, referenceName, null);

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullTypeTest()
        {
            var target = new GenderValueGenerator();

            Action action = () => target.Generate(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(Stream), "gender", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "gender", true)]
        [InlineData(typeof(string), "Gender", true)]
        [InlineData(typeof(string), "sex", true)]
        [InlineData(typeof(string), "Sex", true)]
        public void IsSupportedTest(Type type, string referenceName, bool expected)
        {
            var target = new GenderValueGenerator();

            var actual = target.IsSupported(type, referenceName, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new GenderValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.IO;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class GenderValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomValuesForGenderTypeTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new GenderValueGenerator();

            var maleFound = false;
            var femaleFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = (string) target.Generate(typeof(string), "Gender", executeStrategy);

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
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new GenderValueGenerator();

            var actual = target.Generate(typeof(string), "Sex", executeStrategy);

            actual.Should().BeOfType<string>();
        }

        [Theory]
        [InlineData(typeof(string), "gender")]
        [InlineData(typeof(string), "Gender")]
        [InlineData(typeof(string), "sex")]
        [InlineData(typeof(string), "Sex")]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new GenderValueGenerator();

            var actual = (string) target.Generate(type, referenceName, executeStrategy);

            actual.Should().NotBeNullOrEmpty();
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
            var buildChain = Substitute.For<IBuildChain>();

            var target = new GenderValueGenerator();

            var actual = target.IsSupported(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new GenderValueGenerator();

            Action action = () => target.IsSupported(null, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsPositiveValueTest()
        {
            var target = new GenderValueGenerator();

            target.Priority.Should().BeGreaterThan(0);
        }
    }
}
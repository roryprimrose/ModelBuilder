namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class GenderValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomValuesForGenderTypeTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new GenderValueGenerator();

            var maleFound = false;
            var femaleFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = (string)target.Generate(typeof(string), "Gender", executeStrategy);

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
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new GenderValueGenerator();

            var actual = target.Generate(typeof(string), "Sex", executeStrategy);

            actual.Should().BeOfType<string>();
        }

        [Theory]
        [InlineData(typeof(string), "gender", true)]
        [InlineData(typeof(string), "Gender", true)]
        [InlineData(typeof(string), "sex", true)]
        [InlineData(typeof(string), "Sex", true)]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName, bool expected)
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new GenderValueGenerator();

            var actual = (string)target.Generate(type, referenceName, executeStrategy);

            actual.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(typeof(Stream), "gender")]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(string), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new GenderValueGenerator();

            Action action = () => target.Generate(type, referenceName, executeStrategy);

            action.Should().Throw<NotSupportedException>();
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
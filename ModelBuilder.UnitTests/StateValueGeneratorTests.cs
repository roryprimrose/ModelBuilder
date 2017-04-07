namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class StateValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomStateTest()
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new StateValueGenerator();

            var first = target.Generate(typeof(string), "state", executeStrategy) as string;

            first.Should().NotBeNullOrWhiteSpace();

            string second = null;

            for (var index = 0; index < 1000; index++)
            {
                second = target.Generate(typeof(string), "state", executeStrategy) as string;

                if (first != second)
                {
                    break;
                }
            }
            
            first.Should().NotBe(second);
        }

        [Theory]
        [InlineData(typeof(string), "state", true)]
        [InlineData(typeof(string), "State", true)]
        [InlineData(typeof(string), "region", true)]
        [InlineData(typeof(string), "Region", true)]
        public void GenerateReturnsValuesForSeveralNameFormatsTest(Type type, string referenceName, bool expected)
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new StateValueGenerator();

            var actual = (string)target.Generate(type, referenceName, executeStrategy);

            actual.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(typeof(Stream), "state")]
        [InlineData(typeof(string), null)]
        [InlineData(typeof(string), "Stuff")]
        public void GenerateThrowsExceptionWithInvalidParametersTest(Type type, string referenceName)
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new StateValueGenerator();

            Action action = () => target.Generate(type, referenceName, executeStrategy);

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void HasHigherPriorityThanStringValueGeneratorTest()
        {
            var target = new StateValueGenerator();
            var other = new StringValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Theory]
        [InlineData(typeof(Stream), "state", false)]
        [InlineData(typeof(string), null, false)]
        [InlineData(typeof(string), "", false)]
        [InlineData(typeof(string), "Stuff", false)]
        [InlineData(typeof(string), "state", true)]
        [InlineData(typeof(string), "State", true)]
        [InlineData(typeof(string), "region", true)]
        [InlineData(typeof(string), "Region", true)]
        public void IsSupportedTest(Type type, string referenceName, bool expected)
        {
            var target = new StateValueGenerator();

            var actual = target.IsSupported(type, referenceName, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new StateValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
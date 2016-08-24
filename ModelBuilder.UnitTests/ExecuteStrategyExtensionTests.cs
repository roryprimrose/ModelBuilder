namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class ExecuteStrategyExtensionTests
    {
        [Fact]
        public void CreateReturnsValueFromCreateWithWhileNotUsingParametersTest()
        {
            var expected = Guid.NewGuid().ToString();

            var target = Substitute.For<IExecuteStrategy>();

            target.CreateWith(typeof(string)).Returns(expected);

            var actual = target.Create(typeof(string));

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateThrowsExceptionWithNullInstanceTypeTest()
        {
            var target = Substitute.For<IExecuteStrategy>();

            Action action = () => target.Create(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullStrategyTest()
        {
            IExecuteStrategy target = null;

            Action action = () => target.Create(typeof(string));

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreateTReturnsValueFromCreateWithWhileNotUsingParametersTest()
        {
            var expected = Guid.NewGuid().ToString();

            var target = Substitute.For<IExecuteStrategy<string>>();

            target.CreateWith().Returns(expected);

            var actual = target.Create();

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateTThrowsExceptionWithNullStrategyTest()
        {
            IExecuteStrategy<string> target = null;

            Action action = () => target.Create();

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
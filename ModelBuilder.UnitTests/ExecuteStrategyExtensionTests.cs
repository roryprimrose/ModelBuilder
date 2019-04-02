namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class ExecuteStrategyExtensionTests
    {
        [Fact]
        public void CreateReturnsValueFromCreateWhileNotUsingParametersTest()
        {
            var expected = Guid.NewGuid().ToString();

            var target = Substitute.For<IExecuteStrategy>();

            target.Create(typeof(string)).Returns(expected);

            var actual = ExecuteStrategyExtensions.Create(target, typeof(string));

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateThrowsExceptionWithNullInstanceTypeTest()
        {
            var target = Substitute.For<IExecuteStrategy>();

            Action action = () => ExecuteStrategyExtensions.Create(target, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullStrategyTest()
        {
            IExecuteStrategy target = null;

            Action action = () => ExecuteStrategyExtensions.Create(target, typeof(string));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateTReturnsValueFromCreateWhileNotUsingParametersTest()
        {
            var expected = Guid.NewGuid().ToString();

            var target = Substitute.For<IExecuteStrategy<string>>();

            target.Create().Returns(expected);

            var actual = ExecuteStrategyExtensions.Create(target);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateTThrowsExceptionWithNullStrategyTest()
        {
            IExecuteStrategy<string> target = null;

            Action action = () => ExecuteStrategyExtensions.Create(target);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
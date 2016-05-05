using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class ExecuteStrategyExtensionTests
    {
        [Fact]
        public void CreateReturnsValueFromCreateWithWhileNotUsingParametersTest()
        {
            var expected = Guid.NewGuid().ToString();

            var target = Substitute.For<IExecuteStrategy<string>>();

            target.CreateWith().Returns(expected);

            var actual = target.Create();

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateThrowsExceptionWithNullStrategyTest()
        {
            IExecuteStrategy<string> target = null;

            Action action = () => target.Create();

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
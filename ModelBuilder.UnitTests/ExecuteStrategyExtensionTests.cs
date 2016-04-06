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

        [Fact]
        public void IgnoringAppendsIgnoreRuleTest()
        {
            var target = Substitute.For<IExecuteStrategy<Person>>();

            target.IgnoreRules.Returns(new List<IgnoreRule>());

            var actual = target.Ignoring(x => x.Priority);

            actual.Should().BeSameAs(target);
            actual.IgnoreRules.Should().Contain(x => x.PropertyName == "Priority" && x.TargetType == typeof (Person));
        }

        [Fact]
        public void IgnoringThrowsExceptionWithFieldExpressionTest()
        {
            var target = Substitute.For<IExecuteStrategy<Person>>();

            Action action = () => target.Ignoring(x => x.MinAge);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void IgnoringThrowsExceptionWithMethodExpressionTest()
        {
            var target = Substitute.For<IExecuteStrategy<Person>>();

            Action action = () => target.Ignoring(x => x.DoSomething());

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void IgnoringThrowsExceptionWithNullExpressionTest()
        {
            Expression<Func<Person, object>> expression = null;

            var target = Substitute.For<IExecuteStrategy<Person>>();

            Action action = () => target.Ignoring(expression);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringThrowsExceptionWithNullStrategyTest()
        {
            IExecuteStrategy<Person> target = null;

            Action action = () => target.Ignoring(x => x.Priority);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
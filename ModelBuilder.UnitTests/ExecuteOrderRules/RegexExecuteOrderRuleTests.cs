namespace ModelBuilder.UnitTests.ExecuteOrderRules
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.ExecuteOrderRules;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class RegexExecuteOrderRuleTests
    {
        [Fact]
        public void IsMatchReturnsFalseWhenParameterDoesNotMatch()
        {
            var priority = Environment.TickCount;
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new RegexExecuteOrderRule(NameExpression.LastName, priority);

            var actual = sut.IsMatch(parameterInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsFalseWhenPropertyDoesNotMatch()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            var sut = new RegexExecuteOrderRule(NameExpression.LastName, priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenParameterMatches()
        {
            var priority = Environment.TickCount;
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new RegexExecuteOrderRule(NameExpression.FirstName, priority);

            var actual = sut.IsMatch(parameterInfo);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenPropertyMatches()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            var sut = new RegexExecuteOrderRule(NameExpression.FirstName, priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullParameter()
        {
            var priority = Environment.TickCount;

            var sut = new RegexExecuteOrderRule(NameExpression.FirstName, priority);

            Action action = () => sut.IsMatch((ParameterInfo)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullProperty()
        {
            var priority = Environment.TickCount;

            var sut = new RegexExecuteOrderRule(NameExpression.FirstName, priority);

            Action action = () => sut.IsMatch((PropertyInfo)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsConstructorValue()
        {
            var priority = Environment.TickCount;

            var sut = new RegexExecuteOrderRule(NameExpression.LastName, priority);

            sut.Priority.Should().Be(priority);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullExpression()
        {
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new RegexExecuteOrderRule(null!, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ToStringDoesNotReturnTypeName()
        {
            var priority = Environment.TickCount;

            var sut = new RegexExecuteOrderRule(NameExpression.FirstName, priority);

            var actual = sut.ToString();

            actual.Should().NotBe(sut.GetType().ToString());
        }
    }
}
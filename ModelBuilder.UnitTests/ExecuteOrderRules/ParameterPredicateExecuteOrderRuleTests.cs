namespace ModelBuilder.UnitTests.ExecuteOrderRules
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.ExecuteOrderRules;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class ParameterPredicateExecuteOrderRuleTests
    {
        [Fact]
        public void IsMatchReturnsFalseForProperty()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.LastName))!;

            var sut = new ParameterPredicateExecuteOrderRule(x => x.ParameterType == typeof(string), priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsFalseWhenParameterDoesNotMatch()
        {
            var priority = Environment.TickCount;
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new ParameterPredicateExecuteOrderRule(x => x.ParameterType == typeof(bool), priority);

            var actual = sut.IsMatch(parameterInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenParameterMatches()
        {
            var priority = Environment.TickCount;
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new ParameterPredicateExecuteOrderRule(x => x.Name == "firstName", priority);

            var actual = sut.IsMatch(parameterInfo);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullParameter()
        {
            var priority = Environment.TickCount;
            var sut = new ParameterPredicateExecuteOrderRule(x => x.Name == nameof(Person.FirstName), priority);

            Action action = () => sut.IsMatch((ParameterInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsConstructorValue()
        {
            var priority = Environment.TickCount;

            var sut = new ParameterPredicateExecuteOrderRule(x => x.ParameterType == typeof(string), priority);

            sut.Priority.Should().Be(priority);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullExpression()
        {
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ParameterPredicateExecuteOrderRule(null!, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ToStringDoesNotReturnTypeName()
        {
            var priority = Environment.TickCount;

            var sut = new ParameterPredicateExecuteOrderRule(x => x.ParameterType == typeof(string), priority);

            var actual = sut.ToString();

            actual.ToString(CultureInfo.CurrentCulture).Should().NotBe(sut.GetType().FullName);
        }
    }
}
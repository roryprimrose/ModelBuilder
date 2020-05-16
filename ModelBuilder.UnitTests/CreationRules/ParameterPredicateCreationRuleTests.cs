namespace ModelBuilder.UnitTests.CreationRules
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.CreationRules;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class ParameterPredicateCreationRuleTests
    {
        [Fact]
        public void CreateForParameterReturnsExpressionValue()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new ParameterPredicateCreationRule(item => true, () => value, priority);

            var actual = sut.Create(null!, parameterInfo);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateForParameterReturnsLiteralValue()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new ParameterPredicateCreationRule(item => true, value, priority);

            var actual = sut.Create(null!, parameterInfo);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateForPropertyThrowsException()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new ParameterPredicateCreationRule(item => true, () => value, priority);

            Action action = () => sut.Create(null!, propertyInfo);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void CreateForTypeThrowsException()
        {
            var type = typeof(Person);
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new ParameterPredicateCreationRule(item => true, () => value, priority);

            Action action = () => sut.Create(null!, type);

            action.Should().Throw<NotSupportedException>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsMatchForParameterReturnsPredicateResult(bool isMatch)
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new ParameterPredicateCreationRule(item => isMatch, () => value, priority);

            var actual = sut.IsMatch(parameterInfo);

            actual.Should().Be(isMatch);
        }

        [Fact]
        public void IsMatchForParameterThrowsExceptionWithNullParameter()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new ParameterPredicateCreationRule(type => true, () => value, priority);

            Action action = () => sut.IsMatch((ParameterInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyReturnsFalse()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new ParameterPredicateCreationRule(item => true, () => value, priority);

            var actual = sut.IsMatch(propertyInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForTypeReturnsFalse()
        {
            var type = typeof(Person);
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new ParameterPredicateCreationRule(item => true, () => value, priority);

            var actual = sut.IsMatch(type);

            actual.Should().BeFalse();
        }

        [Fact]
        public void PriorityReturnsConstructorValue()
        {
            var priority = Environment.TickCount;

            var sut = new ParameterPredicateCreationRule(item => true, () => null!, priority);

            sut.Priority.Should().Be(priority);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullParameterPredicate()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ParameterPredicateCreationRule(null!, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullValueGeneratorForParameter()
        {
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ParameterPredicateCreationRule(item => true, null!, priority);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
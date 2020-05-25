namespace ModelBuilder.UnitTests.CreationRules
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.CreationRules;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class PropertyPredicateCreationRuleTests
    {
        [Fact]
        public void CreateForParameterThrowsException()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PropertyPredicateCreationRule(item => true, () => value, priority);

            Action action = () => sut.Create(null!, parameterInfo);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void CreateForPropertyReturnsExpressionValue()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PropertyPredicateCreationRule(item => true, () => value, priority);

            var actual = sut.Create(null!, propertyInfo);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateForPropertyReturnsLiteralValue()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PropertyPredicateCreationRule(item => true, value, priority);

            var actual = sut.Create(null!, propertyInfo);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateForTypeThrowsException()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PropertyPredicateCreationRule(item => true, () => value, priority);

            Action action = () => sut.Create(null!, typeof(string));

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void IsMatchForParameterReturnsFalse()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PropertyPredicateCreationRule((item) => true, () => value, priority);

            var actual = sut.IsMatch(parameterInfo);

            actual.Should().BeFalse();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsMatchForPropertyReturnsPredicateResult(bool isMatch)
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PropertyPredicateCreationRule(item => isMatch, () => value, priority);

            var actual = sut.IsMatch(propertyInfo);

            actual.Should().Be(isMatch);
        }

        [Fact]
        public void IsMatchForPropertyThrowsExceptionWithNullProperty()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PropertyPredicateCreationRule(type => true, () => value, priority);

            Action action = () => sut.IsMatch((PropertyInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeReturnsFalse()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PropertyPredicateCreationRule(item => true, () => value, priority);

            var actual = sut.IsMatch(typeof(string));

            actual.Should().BeFalse();
        }

        [Fact]
        public void PriorityReturnsConstructorValue()
        {
            var priority = Environment.TickCount;

            var sut = new PropertyPredicateCreationRule((item) => true, () => null!, priority);

            sut.Priority.Should().Be(priority);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullPropertyPredicate()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PropertyPredicateCreationRule(null!, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullValueGeneratorForProperty()
        {
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PropertyPredicateCreationRule(item => true, null!, priority);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
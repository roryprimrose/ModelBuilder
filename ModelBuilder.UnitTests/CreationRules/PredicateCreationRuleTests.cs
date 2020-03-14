namespace ModelBuilder.UnitTests.CreationRules
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.CreationRules;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class PredicateCreationRuleTests
    {
        [Fact]
        public void CreateForParameterReturnsExpressionValue()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PredicateCreationRule((ParameterInfo item) => true, () => value, priority);

            var actual = sut.Create(parameterInfo, null);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateForParameterReturnsLiteralValue()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PredicateCreationRule((ParameterInfo item) => true, value, priority);

            var actual = sut.Create(parameterInfo, null);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateForPropertyReturnsExpressionValue()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PredicateCreationRule((PropertyInfo item) => true, () => value, priority);

            var actual = sut.Create(propertyInfo, null);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateForPropertyReturnsLiteralValue()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PredicateCreationRule((PropertyInfo item) => true, value, priority);

            var actual = sut.Create(propertyInfo, null);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateForTypeReturnsExpressionValue()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PredicateCreationRule((Type item) => true, () => value, priority);

            var actual = sut.Create(typeof(string), null);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateForTypeReturnsLiteralValue()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PredicateCreationRule((Type item) => true, value, priority);

            var actual = sut.Create(typeof(string), null);

            actual.Should().Be(value);
        }

        [Fact]
        public void IsMatchForParameterReturnsFalseWhenNoParameterPredicateProvided()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PredicateCreationRule((Type item) => true, () => value, priority);

            var actual = sut.IsMatch(parameterInfo);

            actual.Should().BeFalse();
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

            var sut = new PredicateCreationRule((ParameterInfo item) => isMatch, () => value, priority);

            var actual = sut.IsMatch(parameterInfo);

            actual.Should().Be(isMatch);
        }

        [Fact]
        public void IsMatchForParameterThrowsExceptionWithNullParameter()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PredicateCreationRule((ParameterInfo type) => true, () => value, priority);

            Action action = () => sut.IsMatch((ParameterInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyReturnsFalseWhenNoPropertyPredicateProvided()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PredicateCreationRule((Type item) => true, () => value, priority);

            var actual = sut.IsMatch(propertyInfo);

            actual.Should().BeFalse();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsMatchForPropertyReturnsPredicateResult(bool isMatch)
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PredicateCreationRule((PropertyInfo item) => isMatch, () => value, priority);

            var actual = sut.IsMatch(propertyInfo);

            actual.Should().Be(isMatch);
        }

        [Fact]
        public void IsMatchForPropertyThrowsExceptionWithNullProperty()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PredicateCreationRule((PropertyInfo type) => true, () => value, priority);

            Action action = () => sut.IsMatch((PropertyInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeReturnsFalseWhenNoTypePredicateProvided()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PredicateCreationRule((PropertyInfo item) => true, () => value, priority);

            var actual = sut.IsMatch(typeof(string));

            actual.Should().BeFalse();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsMatchForTypeReturnsPredicateResult(bool isMatch)
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PredicateCreationRule((Type item) => isMatch, () => value, priority);

            var actual = sut.IsMatch(typeof(string));

            actual.Should().Be(isMatch);
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullType()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new PredicateCreationRule((Type item) => true, () => value, priority);

            Action action = () => sut.IsMatch((Type) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityWithParameterReturnsConstructorValue()
        {
            var priority = Environment.TickCount;

            var sut = new PredicateCreationRule((ParameterInfo item) => true, () => null, priority);

            sut.Priority.Should().Be(priority);
        }

        [Fact]
        public void PriorityWithPropertyReturnsConstructorValue()
        {
            var priority = Environment.TickCount;

            var sut = new PredicateCreationRule((PropertyInfo item) => true, () => null, priority);

            sut.Priority.Should().Be(priority);
        }

        [Fact]
        public void PriorityWithTypeReturnsConstructorValue()
        {
            var priority = Environment.TickCount;

            var sut = new PredicateCreationRule((Type item) => true, () => null, priority);

            sut.Priority.Should().Be(priority);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullParameterPredicate()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PredicateCreationRule((Predicate<ParameterInfo>) null, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullPropertyPredicate()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PredicateCreationRule((Predicate<PropertyInfo>) null, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullTypePredicate()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PredicateCreationRule((Predicate<Type>) null, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullValueGeneratorForParameter()
        {
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PredicateCreationRule((ParameterInfo item) => true, null, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullValueGeneratorForProperty()
        {
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PredicateCreationRule((PropertyInfo item) => true, null, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullValueGeneratorForType()
        {
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PredicateCreationRule((Type item) => true, null, priority);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using ModelBuilder.CreationRules;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public partial class BuildConfigurationExtensionsTests
    {
        [Fact]
        public void AddCreationRuleAddsRuleToConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddCreationRule<DummyCreationRule>();

            var actual = sut.CreationRules.Single();

            actual.Should().BeOfType<DummyCreationRule>();
        }

        [Fact]
        public void AddCreationRuleThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.AddCreationRule<DummyCreationRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithExpressionAddsRuleToConfiguration()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new BuildConfiguration();

            sut.AddCreationRule<Person>(x => x.FirstName, value, priority);

            var rule = sut.CreationRules.Single();

            rule.Priority.Should().Be(priority);

            var actual = rule.Create(null, propertyInfo);

            actual.Should().Be(value);
        }

        [Fact]
        public void AddCreationRuleWithExpressionThrowsExceptionWithNullConfiguration()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            Action action = () =>
                BuildConfigurationExtensions.AddCreationRule<Person>(null, x => x.FirstName, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithExpressionThrowsExceptionWithNullExpression()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            var sut = new BuildConfiguration();

            Action action = () => sut.AddCreationRule((Expression<Func<Person, object>>) null, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithParameterPredicateAddsRuleToConfiguration()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            sut.AddCreationRule(x => x.ParameterType == typeof(string), value, priority);

            var rule = sut.CreationRules.Single();

            rule.Priority.Should().Be(priority);

            var actual = rule.Create(null, propertyInfo);

            actual.Should().Be(value);
        }

        [Fact]
        public void AddCreationRuleWithParameterPredicateGeneratorAddsRuleToConfiguration()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            sut.AddCreationRule(x => x.ParameterType == typeof(string), () => value, priority);

            var rule = sut.CreationRules.Single();

            rule.Priority.Should().Be(priority);

            var actual = rule.Create(null, propertyInfo);

            actual.Should().Be(value);
        }

        [Fact]
        public void AddCreationRuleWithParameterPredicateGeneratorThrowsExceptionWithNullConfiguration()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            Action action = () =>
                BuildConfigurationExtensions.AddCreationRule(null, x => x.ParameterType == typeof(string), () => value,
                    priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithParameterPredicateGeneratorThrowsExceptionWithNullPredicate()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            var sut = new BuildConfiguration();

            Action action = () => sut.AddCreationRule((Predicate<ParameterInfo>) null, () => value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithParameterPredicateGeneratorThrowsExceptionWithNullValueGenerator()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            var sut = new BuildConfiguration();

            Action action = () => sut.AddCreationRule(x => x.ParameterType == typeof(string), null, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithParameterPredicateThrowsExceptionWithNullConfiguration()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            Action action = () =>
                BuildConfigurationExtensions.AddCreationRule(null, x => x.ParameterType == typeof(string), value,
                    priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithParameterPredicateThrowsExceptionWithNullPredicate()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            var sut = new BuildConfiguration();

            Action action = () => sut.AddCreationRule((Predicate<ParameterInfo>) null, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithPropertyPredicateAddsRuleToConfiguration()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            sut.AddCreationRule(x => x.PropertyType == typeof(string), value, priority);

            var rule = sut.CreationRules.Single();

            rule.Priority.Should().Be(priority);

            var actual = rule.Create(null, propertyInfo);

            actual.Should().Be(value);
        }

        [Fact]
        public void AddCreationRuleWithPropertyPredicateGeneratorAddsRuleToConfiguration()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            sut.AddCreationRule(x => x.PropertyType == typeof(string), () => value, priority);

            var rule = sut.CreationRules.Single();

            rule.Priority.Should().Be(priority);

            var actual = rule.Create(null, propertyInfo);

            actual.Should().Be(value);
        }

        [Fact]
        public void AddCreationRuleWithPropertyPredicateGeneratorThrowsExceptionWithNullConfiguration()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            Action action = () =>
                BuildConfigurationExtensions.AddCreationRule(null, x => x.PropertyType == typeof(string), () => value,
                    priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithPropertyPredicateGeneratorThrowsExceptionWithNullPredicate()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            var sut = new BuildConfiguration();

            Action action = () => sut.AddCreationRule((Predicate<PropertyInfo>) null, () => value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithPropertyPredicateGeneratorThrowsExceptionWithNullValueGenerator()
        {
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            Action action = () => sut.AddCreationRule(x => x.PropertyType == typeof(string), null, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithPropertyPredicateThrowsExceptionWithNullConfiguration()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            Action action = () =>
                BuildConfigurationExtensions.AddCreationRule(null, x => x.PropertyType == typeof(string), value,
                    priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithPropertyPredicateThrowsExceptionWithNullPredicate()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            var sut = new BuildConfiguration();

            Action action = () => sut.AddCreationRule((Predicate<PropertyInfo>) null, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithRegularExpressionAddsRuleToConfiguration()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var targetType = typeof(string);
            var expression = new Regex("FirstName");
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            sut.AddCreationRule(targetType, expression, value, priority);

            var rule = sut.CreationRules.Single();

            rule.Priority.Should().Be(priority);

            var actual = rule.Create(null, propertyInfo);

            actual.Should().Be(value);
        }

        [Fact]
        public void AddCreationRuleWithRegularExpressionThrowsExceptionWithNullConfiguration()
        {
            var targetType = typeof(string);
            var expression = new Regex("FirstName");
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            Action action = () =>
                BuildConfigurationExtensions.AddCreationRule(null, targetType, expression, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithRegularExpressionThrowsExceptionWithNullExpression()
        {
            var targetType = typeof(string);
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            Action action = () => sut.AddCreationRule(targetType, (Regex) null, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithRegularExpressionThrowsExceptionWithNullTargetType()
        {
            var expression = new Regex("FirstName");
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            Action action = () => sut.AddCreationRule(null, expression, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithStringExpressionAddsRuleToConfiguration()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var targetType = typeof(string);
            var expression = "FirstName";
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            sut.AddCreationRule(targetType, expression, value, priority);

            var rule = sut.CreationRules.Single();

            rule.Priority.Should().Be(priority);

            var actual = rule.Create(null, propertyInfo);

            actual.Should().Be(value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void AddCreationRuleWithStringExpressionThrowsExceptionWithInvalidExpression(string expression)
        {
            var targetType = typeof(string);
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            Action action = () => sut.AddCreationRule(targetType, expression, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithStringExpressionThrowsExceptionWithNullConfiguration()
        {
            var targetType = typeof(string);
            var expression = "FirstName";
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            Action action = () =>
                BuildConfigurationExtensions.AddCreationRule(null, targetType, expression, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithStringExpressionThrowsExceptionWithNullTargetType()
        {
            var expression = "FirstName";
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            Action action = () => sut.AddCreationRule(null, expression, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithTypePredicateAddsRuleToConfiguration()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            sut.AddCreationRule((Type x) => x == typeof(string), value, priority);

            var rule = sut.CreationRules.Single();

            rule.Priority.Should().Be(priority);

            var actual = rule.Create(null, propertyInfo);

            actual.Should().Be(value);
        }

        [Fact]
        public void AddCreationRuleWithTypePredicateGeneratorAddsRuleToConfiguration()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            sut.AddCreationRule((Type x) => x == typeof(string), () => value, priority);

            var rule = sut.CreationRules.Single();

            rule.Priority.Should().Be(priority);

            var actual = rule.Create(null, propertyInfo);

            actual.Should().Be(value);
        }

        [Fact]
        public void AddCreationRuleWithTypePredicateGeneratorThrowsExceptionWithNullConfiguration()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            Action action = () =>
                BuildConfigurationExtensions.AddCreationRule(null, (Type x) => x == typeof(string), () => value,
                    priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithTypePredicateGeneratorThrowsExceptionWithNullPredicate()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            var sut = new BuildConfiguration();

            Action action = () => sut.AddCreationRule((Predicate<Type>) null, () => value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithTypePredicateGeneratorThrowsExceptionWithNullValueGenerator()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            var sut = new BuildConfiguration();

            Action action = () => sut.AddCreationRule((Type x) => x == typeof(string), null, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithTypePredicateThrowsExceptionWithNullConfiguration()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            Action action = () =>
                BuildConfigurationExtensions.AddCreationRule(null, (Type x) => x == typeof(string), value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithTypePredicateThrowsExceptionWithNullPredicate()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            var sut = new BuildConfiguration();

            Action action = () => sut.AddCreationRule((Predicate<Type>) null, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithCreationRuleAddsRuleToConfiguration()
        {
            var rule = new ExpressionCreationRule<Person>(x => x.FirstName, (object) null, Environment.TickCount);

            var sut = new BuildConfiguration();

            sut.Add(rule);

            sut.CreationRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithCreationRuleThrowsExceptionWithNullConfiguration()
        {
            var rule = new ExpressionCreationRule<Person>(x => x.FirstName, (object) null, Environment.TickCount);

            Action action = () => BuildConfigurationExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithCreationRuleThrowsExceptionWithNullRule()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Add((ICreationRule) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveCreationRuleRemovesMultipleMatchingRulesFromConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddCreationRule<DummyCreationRule>();
            sut.AddCreationRule<DummyCreationRule>();
            sut.AddCreationRule<DummyCreationRule>();
            sut.RemoveCreationRule<DummyCreationRule>();

            sut.CreationRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveCreationRuleRemovesRulesFromConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddCreationRule<DummyCreationRule>();
            sut.RemoveCreationRule<DummyCreationRule>();

            sut.CreationRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveCreationRuleThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.RemoveCreationRule<DummyCreationRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateCreationRuleThrowsExceptionWhenRuleNotFound()
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.UpdateCreationRule<RegexCreationRule>(x =>
            {
                // Do nothing
            });

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void UpdateCreationRuleThrowsExceptionWithNullAction()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.UpdateCreationRule<RegexCreationRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateCreationRuleThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.UpdateCreationRule<RegexCreationRule>(null, x =>
            {
                // Do nothing
            });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateCreationRuleUpdateMatchingRule()
        {
            var expected = Guid.NewGuid();
            var sut = new BuildConfiguration();
            var rule = new DummyCreationRule
            {
                Value = expected
            };

            sut.CreationRules.Add(rule);

            sut.UpdateCreationRule<DummyCreationRule>(x => { x.Value = expected; });

            rule.Value.Should().Be(expected);
        }
    }
}
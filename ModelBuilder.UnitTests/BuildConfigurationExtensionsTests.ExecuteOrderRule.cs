namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using ModelBuilder.ExecuteOrderRules;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public partial class BuildConfigurationExtensionsTests
    {
        [Fact]
        public void AddExecuteOrderRuleAddsRuleToConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddExecuteOrderRule<DummyExecuteOrderRule>();

            var actual = sut.ExecuteOrderRules.Single();

            actual.Should().BeOfType<DummyExecuteOrderRule>();
        }

        [Fact]
        public void AddExecuteOrderRuleThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.AddExecuteOrderRule<DummyExecuteOrderRule>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithExpressionAddsRuleToConfiguration()
        {
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            sut.AddExecuteOrderRule<Person>(x => x.FirstName!, priority);

            var rule = sut.ExecuteOrderRules.Single();

            rule.Priority.Should().Be(priority);

            var property = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            var actual = rule.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void AddExecuteOrderRuleWithExpressionThrowsExceptionWithNullConfiguration()
        {
            var priority = Environment.TickCount;

            Action action = () =>
                BuildConfigurationExtensions.AddExecuteOrderRule<Person>(null!, x => x.FirstName!, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithExpressionThrowsExceptionWithNullExpression()
        {
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            Action action = () => sut.AddExecuteOrderRule((Expression<Func<Person, object>>) null!, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithPredicateAddsRuleToConfiguration()
        {
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            sut.AddExecuteOrderRule(x => x.Name == nameof(Person.FirstName), priority);

            var rule = sut.ExecuteOrderRules.Single();

            rule.Priority.Should().Be(priority);

            var property = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            var actual = rule.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void AddExecuteOrderRuleWithPredicateThrowsExceptionWithNullConfiguration()
        {
            var priority = Environment.TickCount;

            Action action = () =>
                BuildConfigurationExtensions.AddExecuteOrderRule(null!, x => x.Name == nameof(Person.FirstName),
                    priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithPredicateThrowsExceptionWithNullPredicate()
        {
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            Action action = () => sut.AddExecuteOrderRule((Predicate<PropertyInfo>) null!, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithRegexAddsRuleToConfiguration()
        {
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            var actual = sut.AddExecuteOrderRule(NameExpression.FirstName, priority);

            actual.Should().Be(sut);

            var rule = sut.ExecuteOrderRules.Single();

            rule.Should().BeOfType<RegexExecuteOrderRule>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithRegexThrowsExceptionWithNullConfiguration()
        {
            var priority = Environment.TickCount;

            Action action = () =>
                BuildConfigurationExtensions.AddExecuteOrderRule(null!, NameExpression.LastName, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithRegexThrowsExceptionWithNullExpression()
        {
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            Action action = () => sut.AddExecuteOrderRule((Regex) null!, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithExecuteOrderRuleAddsRuleToConfiguration()
        {
            var rule = new PropertyPredicateExecuteOrderRule(x => x.Name == "FirstName", Environment.TickCount);

            var sut = new BuildConfiguration();

            sut.Add(rule);

            sut.ExecuteOrderRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithExecuteOrderRuleThrowsExceptionWithNullConfiguration()
        {
            var rule = new PropertyPredicateExecuteOrderRule(x => x.Name == "FirstName", Environment.TickCount);

            Action action = () => BuildConfigurationExtensions.Add(null!, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithExecuteOrderRuleThrowsExceptionWithNullRule()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Add((PropertyPredicateExecuteOrderRule) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveExecuteOrderRuleRemovesMultipleMatchingRulesFromConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddExecuteOrderRule<DummyExecuteOrderRule>();
            sut.AddExecuteOrderRule<DummyExecuteOrderRule>();
            sut.AddExecuteOrderRule<DummyExecuteOrderRule>();
            sut.RemoveExecuteOrderRule<DummyExecuteOrderRule>();

            sut.ExecuteOrderRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveExecuteOrderRuleRemovesRulesFromConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddExecuteOrderRule<DummyExecuteOrderRule>();
            sut.RemoveExecuteOrderRule<DummyExecuteOrderRule>();

            sut.ExecuteOrderRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveExecuteOrderRuleThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.RemoveExecuteOrderRule<DummyExecuteOrderRule>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateExecuteOrderRuleThrowsExceptionWhenRuleNotFound()
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.UpdateExecuteOrderRule<RegexExecuteOrderRule>(x =>
            {
                // Do nothing
            });

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void UpdateExecuteOrderRuleThrowsExceptionWithNullAction()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.UpdateExecuteOrderRule<RegexExecuteOrderRule>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateExecuteOrderRuleThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.UpdateExecuteOrderRule<RegexExecuteOrderRule>(null!, x =>
            {
                // Do nothing
            });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateExecuteOrderRuleUpdateMatchingRule()
        {
            var expected = Guid.NewGuid();
            var sut = new BuildConfiguration();
            var rule = new DummyExecuteOrderRule
            {
                Value = expected
            };

            sut.ExecuteOrderRules.Add(rule);

            sut.UpdateExecuteOrderRule<DummyExecuteOrderRule>(x => { x.Value = expected; });

            rule.Value.Should().Be(expected);
        }
    }
}
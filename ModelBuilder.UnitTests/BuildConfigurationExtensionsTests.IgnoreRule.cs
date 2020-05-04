namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using ModelBuilder.IgnoreRules;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public partial class BuildConfigurationExtensionsTests
    {
        [Fact]
        public void AddIgnoreRuleAddsRuleToCompiler()
        {
            var sut = new BuildConfiguration();

            sut.AddIgnoreRule<DummyIgnoreRule>();

            var actual = sut.IgnoreRules.Single();

            actual.Should().BeOfType<DummyIgnoreRule>();
        }

        [Fact]
        public void AddIgnoreRuleThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.AddIgnoreRule<DummyIgnoreRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithExpressionAddsRuleToCompiler()
        {
            var sut = new BuildConfiguration();

            var actual = sut.AddIgnoreRule<Person>(x => x.FirstName);

            actual.Should().Be(sut);

            var rule = sut.IgnoreRules.Single();

            rule.Should().BeOfType<ExpressionIgnoreRule<Person>>();
        }

        [Fact]
        public void AddIgnoreRuleWithExpressionThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.AddIgnoreRule<Person>(null, x => x.FirstName);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithExpressionThrowsExceptionWithNullExpression()
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.AddIgnoreRule((Expression<Func<Person, object>>) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithPredicateAddsRuleToCompiler()
        {
            var sut = new BuildConfiguration();

            var actual = sut.AddIgnoreRule(x => x.PropertyType == typeof(string));

            actual.Should().Be(sut);

            var rule = sut.IgnoreRules.Single();

            rule.Should().BeOfType<PredicateIgnoreRule>();
        }

        [Fact]
        public void AddIgnoreRuleWithPredicateThrowsExceptionWithNullCompiler()
        {
            Action action = () =>
                BuildConfigurationExtensions.AddIgnoreRule(null, info => info.PropertyType == typeof(string));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithPredicateThrowsExceptionWithNullExpression()
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.AddIgnoreRule((Predicate<PropertyInfo>) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithRegexAddsRuleToCompiler()
        {
            var sut = new BuildConfiguration();

            var actual = sut.AddIgnoreRule(NameExpression.FirstName);

            actual.Should().Be(sut);

            var rule = sut.IgnoreRules.Single();

            rule.Should().BeOfType<RegexIgnoreRule>();
        }

        [Fact]
        public void AddIgnoreRuleWithRegexThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.AddIgnoreRule(null, NameExpression.LastName);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithRegexThrowsExceptionWithNullExpression()
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.AddIgnoreRule((Regex) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithIgnoreRuleAddsRuleToCompiler()
        {
            var rule = new ExpressionIgnoreRule<Person>(x => x.FirstName);

            var sut = new BuildConfiguration();

            sut.Add(rule);

            sut.IgnoreRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithIgnoreRuleThrowsExceptionWithNullCompiler()
        {
            var rule = new ExpressionIgnoreRule<Person>(x => x.FirstName);

            Action action = () => BuildConfigurationExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithIgnoreRuleThrowsExceptionWithNullRule()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Add((IIgnoreRule) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringReturnsNewBuildConfigurationWithIgnoreRuleAppended()
        {
            var ignoreRules = new Collection<IIgnoreRule>();

            var sut = Substitute.For<IBuildConfiguration>();

            sut.IgnoreRules.Returns(ignoreRules);

            var actual = sut.Ignoring<Person>(x => x.Priority);

            actual.IgnoreRules.Should().NotBeEmpty();
            actual.IgnoreRules.Single().Should().BeOfType<ExpressionIgnoreRule<Person>>();
        }

        [Fact]
        public void RemoveIgnoreRuleRemovesMultipleMatchingRulesFromCompiler()
        {
            var sut = new BuildConfiguration();

            sut.AddIgnoreRule<DummyIgnoreRule>();
            sut.AddIgnoreRule<DummyIgnoreRule>();
            sut.AddIgnoreRule<DummyIgnoreRule>();
            sut.RemoveIgnoreRule<DummyIgnoreRule>();

            sut.IgnoreRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveIgnoreRuleRemovesRulesFromCompiler()
        {
            var sut = new BuildConfiguration();

            sut.AddIgnoreRule<DummyIgnoreRule>();
            sut.RemoveIgnoreRule<DummyIgnoreRule>();

            sut.IgnoreRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveIgnoreRuleThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.RemoveIgnoreRule<DummyIgnoreRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateIgnoreRuleThrowsExceptionWhenRuleNotFound()
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.UpdateIgnoreRule<RegexIgnoreRule>(x =>
            {
                // Do nothing
            });

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void UpdateIgnoreRuleThrowsExceptionWithNullAction()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.UpdateIgnoreRule<RegexIgnoreRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateIgnoreRuleThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.UpdateIgnoreRule<RegexIgnoreRule>(null, x =>
            {
                // Do nothing
            });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateIgnoreRuleUpdateMatchingRule()
        {
            var expected = Guid.NewGuid();
            var sut = new BuildConfiguration();
            var rule = new DummyIgnoreRule
            {
                Value = expected
            };

            sut.IgnoreRules.Add(rule);

            sut.UpdateIgnoreRule<DummyIgnoreRule>(x => { x.Value = expected; });

            rule.Value.Should().Be(expected);
        }
    }
}
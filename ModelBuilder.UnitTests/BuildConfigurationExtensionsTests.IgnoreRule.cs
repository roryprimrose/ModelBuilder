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
        public void AddIgnoreRuleAddsRuleToConfiguration()
        {
            var sut = new BuildConfiguration();

            var config = sut.AddIgnoreRule<DummyIgnoreRule>();

            config.Should().Be(sut);

            var actual = sut.IgnoreRules.Single();

            actual.Should().BeOfType<DummyIgnoreRule>();
        }

        [Fact]
        public void AddIgnoreRuleSupportsNullablePropertyType()
        {
            var sut = new BuildConfiguration();

            var config = sut.Ignoring<NullableItem>(x => x.Name);

            var actual = config.IgnoreRules.Single();

            actual.Should().BeOfType<ExpressionIgnoreRule<NullableItem>>();
        }

        [Fact]
        public void AddIgnoreRuleThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.AddIgnoreRule<DummyIgnoreRule>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithExpressionAddsRuleToConfiguration()
        {
            var sut = new BuildConfiguration();

            var actual = sut.AddIgnoreRule<Person>(x => x.FirstName);

            actual.Should().Be(sut);

            var rule = sut.IgnoreRules.Single();

            rule.Should().BeOfType<ExpressionIgnoreRule<Person>>();
        }

        [Fact]
        public void AddIgnoreRuleWithExpressionThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.AddIgnoreRule<Person>(null!, x => x.FirstName);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithExpressionThrowsExceptionWithNullExpression()
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.AddIgnoreRule((Expression<Func<Person, object?>>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithPredicateAddsRuleToConfiguration()
        {
            var sut = new BuildConfiguration();

            var actual = sut.AddIgnoreRule(x => x.PropertyType == typeof(string));

            actual.Should().Be(sut);

            var rule = sut.IgnoreRules.Single();

            rule.Should().BeOfType<PredicateIgnoreRule>();
        }

        [Fact]
        public void AddIgnoreRuleWithPredicateThrowsExceptionWithNullConfiguration()
        {
            Action action = () =>
                BuildConfigurationExtensions.AddIgnoreRule(null!, info => info.PropertyType == typeof(string));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithPredicateThrowsExceptionWithNullExpression()
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.AddIgnoreRule((Predicate<PropertyInfo>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithRegexAddsRuleToConfiguration()
        {
            var sut = new BuildConfiguration();

            var actual = sut.AddIgnoreRule(NameExpression.FirstName);

            actual.Should().Be(sut);

            var rule = sut.IgnoreRules.Single();

            rule.Should().BeOfType<RegexIgnoreRule>();
        }

        [Fact]
        public void AddIgnoreRuleWithRegexExpressionThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.AddIgnoreRule(null!, NameExpression.LastName);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithRegexThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.AddIgnoreRule(null!, NameExpression.LastName);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithRegexThrowsExceptionWithNullExpression()
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.AddIgnoreRule((Regex)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithStringExpressionAddsRuleToConfiguration()
        {
            var sut = new BuildConfiguration();

            var actual = sut.AddIgnoreRule("First");

            actual.Should().Be(sut);

            var rule = sut.IgnoreRules.Single();

            rule.Should().BeOfType<RegexIgnoreRule>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void AddIgnoreRuleWithStringExpressionThrowsExceptionWithInvalidExpression(string? expression)
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.AddIgnoreRule(expression!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithStringExpressionThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.AddIgnoreRule(null!, "First");

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithIgnoreRuleAddsRuleToConfiguration()
        {
            var rule = new ExpressionIgnoreRule<Person>(x => x.FirstName);

            var sut = new BuildConfiguration();

            var config = sut.Add(rule);

            config.Should().Be(sut);

            sut.IgnoreRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithIgnoreRuleThrowsExceptionWithNullConfiguration()
        {
            var rule = new ExpressionIgnoreRule<Person>(x => x.FirstName);

            Action action = () => BuildConfigurationExtensions.Add(null!, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithIgnoreRuleThrowsExceptionWithNullRule()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Add((IIgnoreRule)null!);

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
        public void RemoveIgnoreRuleRemovesMultipleMatchingRulesFromConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddIgnoreRule<DummyIgnoreRule>();
            sut.AddIgnoreRule<DummyIgnoreRule>();
            sut.AddIgnoreRule<DummyIgnoreRule>();
            sut.RemoveIgnoreRule<DummyIgnoreRule>();

            sut.IgnoreRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveIgnoreRuleRemovesRulesFromConfiguration()
        {
            var sut = new BuildConfiguration();

            sut.AddIgnoreRule<DummyIgnoreRule>();

            var config = sut.RemoveIgnoreRule<DummyIgnoreRule>();

            config.Should().Be(sut);

            sut.IgnoreRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveIgnoreRuleThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.RemoveIgnoreRule<DummyIgnoreRule>(null!);

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

            Action action = () => sut.UpdateIgnoreRule<RegexIgnoreRule>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UpdateIgnoreRuleThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.UpdateIgnoreRule<RegexIgnoreRule>(null!, x =>
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

            var config = sut.UpdateIgnoreRule<DummyIgnoreRule>(x => { x.Value = expected; });

            config.Should().Be(sut);

            rule.Value.Should().Be(expected);
        }
    }
}
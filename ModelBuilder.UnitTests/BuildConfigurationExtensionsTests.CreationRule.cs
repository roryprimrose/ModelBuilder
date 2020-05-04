namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using FluentAssertions;
    using ModelBuilder.CreationRules;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public partial class BuildConfigurationExtensionsTests
    {
        [Fact]
        public void AddCreationRuleAddsRuleToCompiler()
        {
            var sut = new BuildConfiguration();

            sut.AddCreationRule<DummyCreationRule>();

            var actual = sut.CreationRules.Single();

            actual.Should().BeOfType<DummyCreationRule>();
        }

        [Fact]
        public void AddCreationRuleThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.AddCreationRule<DummyCreationRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithExpressionAddsRuleToCompiler()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new BuildConfiguration();

            sut.AddCreationRule<Person>(x => x.FirstName, priority, value);

            var rule = sut.CreationRules.Single();

            rule.Priority.Should().Be(priority);

            var actual = rule.Create(null, propertyInfo);

            actual.Should().Be(value);
        }

        [Fact]
        public void AddCreationRuleWithExpressionThrowsExceptionWithNullCompiler()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            Action action = () =>
                BuildConfigurationExtensions.AddCreationRule<Person>(null, x => x.FirstName, priority, value);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithExpressionThrowsExceptionWithNullExpression()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            var sut = new BuildConfiguration();

            Action action = () => sut.AddCreationRule((Expression<Func<Person, object>>) null, priority, value);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithCreationRuleAddsRuleToCompiler()
        {
            var rule = new ExpressionCreationRule<Person>(x => x.FirstName, (object) null, Environment.TickCount);

            var sut = new BuildConfiguration();

            sut.Add(rule);

            sut.CreationRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithCreationRuleThrowsExceptionWithNullCompiler()
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
        public void RemoveCreationRuleRemovesMultipleMatchingRulesFromCompiler()
        {
            var sut = new BuildConfiguration();

            sut.AddCreationRule<DummyCreationRule>();
            sut.AddCreationRule<DummyCreationRule>();
            sut.AddCreationRule<DummyCreationRule>();
            sut.RemoveCreationRule<DummyCreationRule>();

            sut.CreationRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveCreationRuleRemovesRulesFromCompiler()
        {
            var sut = new BuildConfiguration();

            sut.AddCreationRule<DummyCreationRule>();
            sut.RemoveCreationRule<DummyCreationRule>();

            sut.CreationRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveCreationRuleThrowsExceptionWithNullCompiler()
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
using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class BuildStrategyCompilerExtensionsTests
    {
        [Fact]
        public void AddCreationRuleAddsRuleToCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddCreationRule<DummyCreationRule>();

            var actual = target.CreationRules.Single();

            actual.Should().BeOfType<DummyCreationRule>();
        }

        [Fact]
        public void AddCreationRuleThrowsExceptionWithNullCompilerTest()
        {
            IBuildStrategyCompiler target = null;

            Action action = () => target.AddCreationRule<DummyCreationRule>();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithExpressionAddsRuleToCompilerTest()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            var target = new BuildStrategyCompiler();

            target.AddCreationRule<Person>(x => x.FirstName, priority, value);

            var rule = target.CreationRules.Single();

            rule.Priority.Should().Be(priority);

            var actual = rule.Create(typeof(Person), nameof(Person.FirstName), null);

            actual.Should().Be(value);
        }

        [Fact]
        public void AddCreationRuleWithExpressionThrowsExceptionWithNullCompilerTest()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            BuildStrategyCompiler target = null;

            Action action = () => target.AddCreationRule<Person>(x => x.FirstName, priority, value);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithExpressionThrowsExceptionWithNullExpressionTest()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            var target = new BuildStrategyCompiler();

            Action action = () => target.AddCreationRule((Expression<Func<Person, object>>) null, priority, value);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleAddsRuleToCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddExecuteOrderRule<ExecuteOrderRuleWrapper>();

            var actual = target.ExecuteOrderRules.Single();

            actual.Should().BeOfType<ExecuteOrderRuleWrapper>();
        }

        [Fact]
        public void AddExecuteOrderRuleThrowsExceptionWithNullCompilerTest()
        {
            IBuildStrategyCompiler target = null;

            Action action = () => target.AddExecuteOrderRule<ExecuteOrderRuleWrapper>();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithExpressionAddsRuleToCompilerTest()
        {
            var priority = Environment.TickCount;

            var target = new BuildStrategyCompiler();

            target.AddExecuteOrderRule<Person>(x => x.FirstName, priority);

            var rule = target.ExecuteOrderRules.Single();

            rule.Priority.Should().Be(priority);

            var actual = rule.IsMatch(typeof(Person), nameof(Person.FirstName));

            actual.Should().BeTrue();
        }

        [Fact]
        public void AddExecuteOrderRuleWithExpressionThrowsExceptionWithNullCompilerTest()
        {
            var priority = Environment.TickCount;

            BuildStrategyCompiler target = null;

            Action action = () => target.AddExecuteOrderRule<Person>(x => x.FirstName, priority);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithExpressionThrowsExceptionWithNullExpressionTest()
        {
            var priority = Environment.TickCount;

            var target = new BuildStrategyCompiler();

            Action action = () => target.AddExecuteOrderRule((Expression<Func<Person, object>>) null, priority);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleAddsRuleToCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddIgnoreRule<IgnoreRuleWrapper>();

            var actual = target.IgnoreRules.Single();

            actual.Should().BeOfType<IgnoreRuleWrapper>();
        }

        [Fact]
        public void AddIgnoreRuleThrowsExceptionWithNullCompilerTest()
        {
            IBuildStrategyCompiler target = null;

            Action action = () => target.AddIgnoreRule<IgnoreRuleWrapper>();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithExpressionAddsRuleToCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddIgnoreRule<Person>(x => x.FirstName);

            var rule = target.IgnoreRules.Single();

            rule.TargetType.Should().Be<Person>();
            rule.PropertyName.Should().Be(nameof(Person.FirstName));
        }

        [Fact]
        public void AddIgnoreRuleWithExpressionThrowsExceptionWithNullCompilerTest()
        {
            BuildStrategyCompiler target = null;

            Action action = () => target.AddIgnoreRule<Person>(x => x.FirstName);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithExpressionThrowsExceptionWithNullExpressionTest()
        {
            var target = new BuildStrategyCompiler();

            Action action = () => target.AddIgnoreRule((Expression<Func<Person, object>>) null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddTypeCreatorAddsRuleToCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddTypeCreator<DefaultTypeCreator>();

            var actual = target.TypeCreators.Single();

            actual.Should().BeOfType<DefaultTypeCreator>();
        }

        [Fact]
        public void AddTypeCreatorThrowsExceptionWithNullCompilerTest()
        {
            IBuildStrategyCompiler target = null;

            Action action = () => target.AddTypeCreator<DefaultTypeCreator>();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddValueGeneratorAddsRuleToCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddValueGenerator<StringValueGenerator>();

            var actual = target.ValueGenerators.Single();

            actual.Should().BeOfType<StringValueGenerator>();
        }

        [Fact]
        public void AddValueGeneratorThrowsExceptionWithNullCompilerTest()
        {
            IBuildStrategyCompiler target = null;

            Action action = () => target.AddValueGenerator<StringValueGenerator>();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddWithCreationRuleAddsRuleToCompilerTest()
        {
            var rule = new CreationRule(typeof(Person), "FirstName", Environment.TickCount, (object) null);

            var target = new BuildStrategyCompiler();

            target.Add(rule);

            target.CreationRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithCreationRuleThrowsExceptionWithNullCompilerTest()
        {
            var rule = new CreationRule(typeof(Person), "FirstName", Environment.TickCount, (object) null);

            IBuildStrategyCompiler target = null;

            Action action = () => target.Add(rule);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddWithCreationRuleThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildStrategyCompiler>();

            Action action = () => target.Add((CreationRule) null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddWithExecuteOrderRuleAddsRuleToCompilerTest()
        {
            var rule = new ExecuteOrderRule(typeof(Person), "FirstName", Environment.TickCount);

            var target = new BuildStrategyCompiler();

            target.Add(rule);

            target.ExecuteOrderRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithExecuteOrderRuleThrowsExceptionWithNullCompilerTest()
        {
            var rule = new ExecuteOrderRule(typeof(Person), "FirstName", Environment.TickCount);

            IBuildStrategyCompiler target = null;

            Action action = () => target.Add(rule);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddWithExecuteOrderRuleThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildStrategyCompiler>();

            Action action = () => target.Add((ExecuteOrderRule) null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddWithIgnoreRuleAddsRuleToCompilerTest()
        {
            var rule = new IgnoreRule(typeof(Person), "FirstName");

            var target = new BuildStrategyCompiler();

            target.Add(rule);

            target.IgnoreRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithIgnoreRuleThrowsExceptionWithNullCompilerTest()
        {
            var rule = new IgnoreRule(typeof(Person), "FirstName");

            IBuildStrategyCompiler target = null;

            Action action = () => target.Add(rule);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddWithIgnoreRuleThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildStrategyCompiler>();

            Action action = () => target.Add((IgnoreRule) null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeCreatorAddsRuleToCompilerTest()
        {
            var rule = new DefaultTypeCreator();

            var target = new BuildStrategyCompiler();

            target.Add(rule);

            target.TypeCreators.Should().Contain(rule);
        }

        [Fact]
        public void AddWithTypeCreatorThrowsExceptionWithNullCompilerTest()
        {
            var rule = new DefaultTypeCreator();

            IBuildStrategyCompiler target = null;

            Action action = () => target.Add(rule);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeCreatorThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildStrategyCompiler>();

            Action action = () => target.Add((ITypeCreator) null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddWithValueGeneratorAddsRuleToCompilerTest()
        {
            var rule = new StringValueGenerator();

            var target = new BuildStrategyCompiler();

            target.Add(rule);

            target.ValueGenerators.Should().Contain(rule);
        }

        [Fact]
        public void AddWithValueGeneratorThrowsExceptionWithNullCompilerTest()
        {
            var rule = new StringValueGenerator();

            IBuildStrategyCompiler target = null;

            Action action = () => target.Add(rule);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddWithValueGeneratorThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildStrategyCompiler>();

            Action action = () => target.Add((IValueGenerator) null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void SetConstructorResolverAssignsResolverToCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.SetConstructorResolver<DefaultConstructorResolver>();

            var actual = target.ConstructorResolver;

            actual.Should().BeOfType<DefaultConstructorResolver>();
        }

        [Fact]
        public void SetConstructorResolverThrowsExceptionWithNullCompilerTest()
        {
            IBuildStrategyCompiler target = null;

            Action action = () => target.SetConstructorResolver<DefaultConstructorResolver>();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void SetConstructorResolverThrowsExceptionWithNullResolverTest()
        {
            var target = Substitute.For<IBuildStrategyCompiler>();

            Action action = () => target.SetConstructorResolver(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void SetConstructorResolverWithResolverAssignsResolverToCompilerTest()
        {
            var resolver = new DefaultConstructorResolver();

            var target = new BuildStrategyCompiler();

            target.SetConstructorResolver(resolver);

            target.ConstructorResolver.Should().Be(resolver);
        }

        [Fact]
        public void SetConstructorResolverWithResolverThrowsExceptionWithNullCompilerTest()
        {
            var resolver = new DefaultConstructorResolver();

            IBuildStrategyCompiler target = null;

            Action action = () => target.SetConstructorResolver(resolver);

            action.ShouldThrow<ArgumentNullException>();
        }

        private class ExecuteOrderRuleWrapper : ExecuteOrderRule
        {
            public ExecuteOrderRuleWrapper() : base(typeof(string), "FirstName", Environment.TickCount)
            {
            }
        }

        private class IgnoreRuleWrapper : IgnoreRule
        {
            public IgnoreRuleWrapper() : base(typeof(string), "FirstName")
            {
            }
        }
    }
}
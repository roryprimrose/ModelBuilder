using System;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class BuildStrategyCompilerExtensionsTests
    {
        [Fact]
        public void AddExecuteOrderRuleAddsRuleToCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddExecutionOrderRule<ExecuteOrderRuleWrapper>();

            var actual = target.ExecuteOrderRules.Single();

            actual.Should().BeOfType<ExecuteOrderRuleWrapper>();
        }

        [Fact]
        public void AddExecuteOrderRuleThrowsExceptionWithNullCompilerTest()
        {
            IBuildStrategyCompiler target = null;

            Action action = () => target.AddExecutionOrderRule<ExecuteOrderRuleWrapper>();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithEvaluatorAndPriorityAddsRuleToCompilerTest()
        {
            var priority = Environment.TickCount;

            var target = new BuildStrategyCompiler();

            target.AddExecuteOrderRule((type, propertyName) => type.IsValueType, priority);

            var actual = target.ExecuteOrderRules.Single();

            actual.IsMatch(typeof(Guid), null).Should().BeTrue();
            actual.Priority.Should().Be(priority);
        }

        [Fact]
        public void AddExecuteOrderRuleWithTargetTypePropertyNameAndPriorityAddsRuleToCompilerTest()
        {
            var priority = Environment.TickCount;

            var target = new BuildStrategyCompiler();

            target.AddExecuteOrderRule(typeof(string), "FirstName", priority);

            var actual = target.ExecuteOrderRules.Single();

            actual.IsMatch(typeof(string), "FirstName").Should().BeTrue();
            actual.IsMatch(typeof(string), "LastName").Should().BeFalse();
            actual.Priority.Should().Be(priority);
        }

        [Fact]
        public void AddExecuteOrderRuleWithTargetTypeRegExAndPriorityAddsRuleToCompilerTest()
        {
            var priority = Environment.TickCount;

            var target = new BuildStrategyCompiler();

            target.AddExecuteOrderRule(typeof(string), new Regex("First.*"), priority);

            var actual = target.ExecuteOrderRules.Single();

            actual.IsMatch(typeof(string), "FirstName").Should().BeTrue();
            actual.IsMatch(typeof(string), "LastName").Should().BeFalse();
            actual.Priority.Should().Be(priority);
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
        public void AddIgnoreRuleWithTargetTypeAndPropertyNameAddsRuleToCompilerTest()
        {
            var targetType = typeof(string);
            var propertyName = "FirstName";

            var target = new BuildStrategyCompiler();

            target.AddIgnoreRule(targetType, propertyName);

            var actual = target.IgnoreRules.Single();

            actual.TargetType.Should().Be(targetType);
            actual.PropertyName.Should().Be(propertyName);
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
        public void AddWithExecuteOrderRuleWithEvaluatorAndPriorityThrowsExceptionWithNullCompilerTest()
        {
            var priority = Environment.TickCount;

            IBuildStrategyCompiler target = null;

            Action action = () => target.AddExecuteOrderRule((type, propertyName) => type.IsValueType, priority);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddWithExecuteOrderRuleWithTargetTypePropertyNameAndPriorityThrowsExceptionWithNullCompilerTest()
        {
            var priority = Environment.TickCount;

            IBuildStrategyCompiler target = null;

            Action action = () => target.AddExecuteOrderRule(typeof(string), "FirstName", priority);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void AddWithExecuteOrderRuleWithTargetTypeRegExAndPriorityThrowsExceptionWithNullCompilerTest()
        {
            var priority = Environment.TickCount;

            IBuildStrategyCompiler target = null;

            Action action = () => target.AddExecuteOrderRule(typeof(string), new Regex("First.*"), priority);

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
        public void AddWithIgnoreRuleWithTargetTypeAndPropertyNameThrowsExceptionWithNullCompilerTest()
        {
            IBuildStrategyCompiler target = null;

            Action action = () => target.AddIgnoreRule(typeof(string), "FirstName");

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
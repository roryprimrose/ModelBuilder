namespace ModelBuilder.UnitTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class BuildStrategyCompilerExtensionsTests
    {
        [Fact]
        public void AddCompilerModuleAddsRuleToCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddCompilerModule<TestCompilerModule>();

            var actual = target.PostBuildActions.OfType<DummyPostBuildAction>();

            actual.Should().NotBeEmpty();
        }

        [Fact]
        public void AddCompilerModuleThrowsExceptionWithNullCompilerTest()
        {
            Action action = () => BuildStrategyCompilerExtensions.AddCompilerModule<TestCompilerModule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

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
            Action action = () => BuildStrategyCompilerExtensions.AddCreationRule<DummyCreationRule>(null);

            action.Should().Throw<ArgumentNullException>();
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

            Action action = () =>
                BuildStrategyCompilerExtensions.AddCreationRule<Person>(null, x => x.FirstName, priority, value);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleWithExpressionThrowsExceptionWithNullExpressionTest()
        {
            var priority = Environment.TickCount;
            var value = Guid.NewGuid().ToString();

            var target = new BuildStrategyCompiler();

            Action action = () => target.AddCreationRule((Expression<Func<Person, object>>)null, priority, value);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleAddsRuleToCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddExecuteOrderRule<DummyExecuteOrderRule>();

            var actual = target.ExecuteOrderRules.Single();

            actual.Should().BeOfType<DummyExecuteOrderRule>();
        }

        [Fact]
        public void AddExecuteOrderRuleThrowsExceptionWithNullCompilerTest()
        {
            Action action = () => BuildStrategyCompilerExtensions.AddExecuteOrderRule<DummyExecuteOrderRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithExpressionAddsRuleToCompilerTest()
        {
            var priority = Environment.TickCount;

            var target = new BuildStrategyCompiler();

            target.AddExecuteOrderRule<Person>(x => x.FirstName, priority);

            var rule = target.ExecuteOrderRules.Single();

            rule.Priority.Should().Be(priority);

            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var actual = rule.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void AddExecuteOrderRuleWithExpressionThrowsExceptionWithNullCompilerTest()
        {
            var priority = Environment.TickCount;

            Action action = () =>
                BuildStrategyCompilerExtensions.AddExecuteOrderRule<Person>(null, x => x.FirstName, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithExpressionThrowsExceptionWithNullExpressionTest()
        {
            var priority = Environment.TickCount;

            var target = new BuildStrategyCompiler();

            Action action = () => target.AddExecuteOrderRule((Expression<Func<Person, object>>)null, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleAddsRuleToCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddIgnoreRule<DummyIgnoreRule>();

            var actual = target.IgnoreRules.Single();

            actual.Should().BeOfType<DummyIgnoreRule>();
        }

        [Fact]
        public void AddIgnoreRuleThrowsExceptionWithNullCompilerTest()
        {
            Action action = () => BuildStrategyCompilerExtensions.AddIgnoreRule<DummyIgnoreRule>(null);

            action.Should().Throw<ArgumentNullException>();
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
            Action action = () => BuildStrategyCompilerExtensions.AddIgnoreRule<Person>(null, x => x.FirstName);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithExpressionThrowsExceptionWithNullExpressionTest()
        {
            var target = new BuildStrategyCompiler();

            Action action = () => target.AddIgnoreRule((Expression<Func<Person, object>>)null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddPostBuildActionAddsRuleToCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddPostBuildAction<DummyPostBuildAction>();

            var actual = target.PostBuildActions.Single();

            actual.Should().BeOfType<DummyPostBuildAction>();
        }

        [Fact]
        public void AddPostBuildActionThrowsExceptionWithNullCompilerTest()
        {
            Action action = () => BuildStrategyCompilerExtensions.AddPostBuildAction<DummyPostBuildAction>(null);

            action.Should().Throw<ArgumentNullException>();
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
            Action action = () => BuildStrategyCompilerExtensions.AddTypeCreator<DefaultTypeCreator>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddTypeMappingRuleAddsRuleToCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddTypeMappingRule<DummyTypeMappingRule>();

            var actual = target.TypeMappingRules.Single();

            actual.Should().BeOfType<DummyTypeMappingRule>();
        }

        [Fact]
        public void AddTypeMappingRuleThrowsExceptionWithNullCompilerTest()
        {
            Action action = () => BuildStrategyCompilerExtensions.AddTypeMappingRule<DummyTypeMappingRule>(null);

            action.Should().Throw<ArgumentNullException>();
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
            Action action = () => BuildStrategyCompilerExtensions.AddValueGenerator<StringValueGenerator>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithCompilerModuleAddsRuleToCompilerTest()
        {
            var module = Substitute.For<ICompilerModule>();

            var target = new BuildStrategyCompiler();

            target.Add(module);

            module.Received().Configure(target);
        }

        [Fact]
        public void AddWithCompilerModuleThrowsExceptionWithNullCompilerTest()
        {
            var module = new TestCompilerModule();

            Action action = () => BuildStrategyCompilerExtensions.Add(null, module);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithCompilerModuleThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildStrategyCompiler>();

            Action action = () => target.Add((ICompilerModule)null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithCreationRuleAddsRuleToCompilerTest()
        {
            var rule = new CreationRule(typeof(Person), "FirstName", Environment.TickCount, (object)null);

            var target = new BuildStrategyCompiler();

            target.Add(rule);

            target.CreationRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithCreationRuleThrowsExceptionWithNullCompilerTest()
        {
            var rule = new CreationRule(typeof(Person), "FirstName", Environment.TickCount, (object)null);

            Action action = () => BuildStrategyCompilerExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithCreationRuleThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildStrategyCompiler>();

            Action action = () => target.Add((CreationRule)null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithExecuteOrderRuleAddsRuleToCompilerTest()
        {
            var rule = new ExecuteOrderRule(typeof(Person), typeof(string), "FirstName", Environment.TickCount);

            var target = new BuildStrategyCompiler();

            target.Add(rule);

            target.ExecuteOrderRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithExecuteOrderRuleThrowsExceptionWithNullCompilerTest()
        {
            var rule = new ExecuteOrderRule(typeof(Person), typeof(string), "FirstName", Environment.TickCount);

            Action action = () => BuildStrategyCompilerExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithExecuteOrderRuleThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildStrategyCompiler>();

            Action action = () => target.Add((ExecuteOrderRule)null);

            action.Should().Throw<ArgumentNullException>();
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

            Action action = () => BuildStrategyCompilerExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithIgnoreRuleThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildStrategyCompiler>();

            Action action = () => target.Add((IgnoreRule)null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithPostBuildActionAddsRuleToCompilerTest()
        {
            var postBuildAction = new DummyPostBuildAction();

            var target = new BuildStrategyCompiler();

            target.Add(postBuildAction);

            target.PostBuildActions.Should().Contain(postBuildAction);
        }

        [Fact]
        public void AddWithPostBuildActionThrowsExceptionWithNullCompilerTest()
        {
            var postBuildAction = new DummyPostBuildAction();

            Action action = () => BuildStrategyCompilerExtensions.Add(null, postBuildAction);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithPostBuildActionThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildStrategyCompiler>();

            Action action = () => target.Add((IPostBuildAction)null);

            action.Should().Throw<ArgumentNullException>();
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

            Action action = () => BuildStrategyCompilerExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeCreatorThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildStrategyCompiler>();

            Action action = () => target.Add((ITypeCreator)null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeMappingRuleAddsRuleToCompilerTest()
        {
            var rule = new TypeMappingRule(typeof(Stream), typeof(MemoryStream));

            var target = new BuildStrategyCompiler();

            target.Add(rule);

            target.TypeMappingRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithTypeMappingRuleThrowsExceptionWithNullCompilerTest()
        {
            var rule = new TypeMappingRule(typeof(Stream), typeof(MemoryStream));

            Action action = () => BuildStrategyCompilerExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeMappingRuleThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildStrategyCompiler>();

            Action action = () => target.Add((TypeMappingRule)null);

            action.Should().Throw<ArgumentNullException>();
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

            Action action = () => BuildStrategyCompilerExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithValueGeneratorThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildStrategyCompiler>();

            Action action = () => target.Add((IValueGenerator)null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveCreationRuleRemovesMultipleMatchingRulesFromCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddCreationRule<DummyCreationRule>();
            target.AddCreationRule<DummyCreationRule>();
            target.AddCreationRule<DummyCreationRule>();
            target.RemoveCreationRule<DummyCreationRule>();

            target.CreationRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveCreationRuleRemovesRulesFromCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddCreationRule<DummyCreationRule>();
            target.RemoveCreationRule<DummyCreationRule>();

            target.CreationRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveCreationRuleThrowsExceptionWithNullCompilerTest()
        {
            Action action = () => BuildStrategyCompilerExtensions.RemoveCreationRule<DummyCreationRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveExecuteOrderRuleRemovesMultipleMatchingRulesFromCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddExecuteOrderRule<DummyExecuteOrderRule>();
            target.AddExecuteOrderRule<DummyExecuteOrderRule>();
            target.AddExecuteOrderRule<DummyExecuteOrderRule>();
            target.RemoveExecuteOrderRule<DummyExecuteOrderRule>();

            target.ExecuteOrderRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveExecuteOrderRuleRemovesRulesFromCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddExecuteOrderRule<DummyExecuteOrderRule>();
            target.RemoveExecuteOrderRule<DummyExecuteOrderRule>();

            target.ExecuteOrderRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveExecuteOrderRuleThrowsExceptionWithNullCompilerTest()
        {
            Action action = () => BuildStrategyCompilerExtensions.RemoveExecuteOrderRule<DummyExecuteOrderRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveIgnoreRuleRemovesMultipleMatchingRulesFromCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddIgnoreRule<DummyIgnoreRule>();
            target.AddIgnoreRule<DummyIgnoreRule>();
            target.AddIgnoreRule<DummyIgnoreRule>();
            target.RemoveIgnoreRule<DummyIgnoreRule>();

            target.IgnoreRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveIgnoreRuleRemovesRulesFromCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddIgnoreRule<DummyIgnoreRule>();
            target.RemoveIgnoreRule<DummyIgnoreRule>();

            target.IgnoreRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveIgnoreRuleThrowsExceptionWithNullCompilerTest()
        {
            Action action = () => BuildStrategyCompilerExtensions.RemoveIgnoreRule<DummyIgnoreRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemovePostBuildActionRemovesMultipleMatchingRulesFromCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddPostBuildAction<DummyPostBuildAction>();
            target.AddPostBuildAction<DummyPostBuildAction>();
            target.AddPostBuildAction<DummyPostBuildAction>();
            target.RemovePostBuildAction<DummyPostBuildAction>();

            target.PostBuildActions.Should().BeEmpty();
        }

        [Fact]
        public void RemovePostBuildActionRemovesRulesFromCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddPostBuildAction<DummyPostBuildAction>();
            target.RemovePostBuildAction<DummyPostBuildAction>();

            target.PostBuildActions.Should().BeEmpty();
        }

        [Fact]
        public void RemovePostBuildActionThrowsExceptionWithNullCompilerTest()
        {
            Action action = () => BuildStrategyCompilerExtensions.RemovePostBuildAction<DummyPostBuildAction>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveTypeCreatorRemovesMultipleMatchingRulesFromCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddTypeCreator<DefaultTypeCreator>();
            target.AddTypeCreator<DefaultTypeCreator>();
            target.AddTypeCreator<DefaultTypeCreator>();
            target.RemoveTypeCreator<DefaultTypeCreator>();

            target.TypeCreators.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTypeCreatorRemovesRulesFromCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddTypeCreator<DefaultTypeCreator>();
            target.RemoveTypeCreator<DefaultTypeCreator>();

            target.TypeCreators.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTypeCreatorThrowsExceptionWithNullCompilerTest()
        {
            Action action = () => BuildStrategyCompilerExtensions.RemoveTypeCreator<DefaultTypeCreator>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveTypeMappingRuleRemovesMultipleMatchingRulesFromCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddTypeMappingRule<DummyTypeMappingRule>();
            target.AddTypeMappingRule<DummyTypeMappingRule>();
            target.AddTypeMappingRule<DummyTypeMappingRule>();
            target.RemoveTypeMappingRule<DummyTypeMappingRule>();

            target.TypeMappingRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTypeMappingRuleRemovesRulesFromCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddTypeMappingRule<DummyTypeMappingRule>();
            target.RemoveTypeMappingRule<DummyTypeMappingRule>();

            target.TypeMappingRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTypeMappingRuleThrowsExceptionWithNullCompilerTest()
        {
            Action action = () => BuildStrategyCompilerExtensions.RemoveTypeMappingRule<DummyTypeMappingRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveValueGeneratorRemovesMultipleMatchingRulesFromCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddValueGenerator<StringValueGenerator>();
            target.AddValueGenerator<StringValueGenerator>();
            target.AddValueGenerator<StringValueGenerator>();
            target.RemoveValueGenerator<StringValueGenerator>();

            target.ValueGenerators.Should().BeEmpty();
        }

        [Fact]
        public void RemoveValueGeneratorRemovesRulesFromCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.AddValueGenerator<StringValueGenerator>();
            target.RemoveValueGenerator<StringValueGenerator>();

            target.ValueGenerators.Should().BeEmpty();
        }

        [Fact]
        public void RemoveValueGeneratorThrowsExceptionWithNullCompilerTest()
        {
            Action action = () => BuildStrategyCompilerExtensions.RemoveValueGenerator<StringValueGenerator>(null);

            action.Should().Throw<ArgumentNullException>();
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
            Action action = () =>
                BuildStrategyCompilerExtensions.SetConstructorResolver<DefaultConstructorResolver>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetConstructorResolverThrowsExceptionWithNullResolverTest()
        {
            var target = Substitute.For<IBuildStrategyCompiler>();

            Action action = () => target.SetConstructorResolver(null);

            action.Should().Throw<ArgumentNullException>();
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

            Action action = () => BuildStrategyCompilerExtensions.SetConstructorResolver(null, resolver);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetPropertyResolverAssignsResolverToCompilerTest()
        {
            var target = new BuildStrategyCompiler();

            target.SetPropertyResolver<DefaultPropertyResolver>();

            var actual = target.PropertyResolver;

            actual.Should().BeOfType<DefaultPropertyResolver>();
        }

        [Fact]
        public void SetPropertyResolverThrowsExceptionWithNullCompilerTest()
        {
            Action action = () => BuildStrategyCompilerExtensions.SetPropertyResolver<DefaultPropertyResolver>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetPropertyResolverThrowsExceptionWithNullResolverTest()
        {
            var target = Substitute.For<IBuildStrategyCompiler>();

            Action action = () => target.SetPropertyResolver(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetPropertyResolverWithResolverAssignsResolverToCompilerTest()
        {
            var resolver = new DefaultPropertyResolver();

            var target = new BuildStrategyCompiler();

            target.SetPropertyResolver(resolver);

            target.PropertyResolver.Should().Be(resolver);
        }

        [Fact]
        public void SetPropertyResolverWithResolverThrowsExceptionWithNullCompilerTest()
        {
            var resolver = new DefaultPropertyResolver();

            Action action = () => BuildStrategyCompilerExtensions.SetPropertyResolver(null, resolver);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
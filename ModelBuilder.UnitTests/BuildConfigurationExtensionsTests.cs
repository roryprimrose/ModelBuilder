namespace ModelBuilder.UnitTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class BuildConfigurationExtensionsTests
    {
        [Fact]
        public void AddCompilerModuleAddsRuleToCompilerTest()
        {
            var target = new BuildConfiguration();

            var config = target.AddCompilerModule<TestConfigurationModule>();

            config.Should().BeSameAs(target);

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
            var target = new BuildConfiguration();

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

            var target = new BuildConfiguration();

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

            var target = new BuildConfiguration();

            Action action = () => target.AddCreationRule((Expression<Func<Person, object>>) null, priority, value);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleAddsRuleToCompilerTest()
        {
            var target = new BuildConfiguration();

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

            var target = new BuildConfiguration();

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

            var target = new BuildConfiguration();

            Action action = () => target.AddExecuteOrderRule((Expression<Func<Person, object>>) null, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleAddsRuleToCompilerTest()
        {
            var target = new BuildConfiguration();

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
            var target = new BuildConfiguration();

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
            var target = new BuildConfiguration();

            Action action = () => target.AddIgnoreRule((Expression<Func<Person, object>>) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddPostBuildActionAddsRuleToCompilerTest()
        {
            var target = new BuildConfiguration();

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
            var target = new BuildConfiguration();

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
            var target = new BuildConfiguration();

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
            var target = new BuildConfiguration();

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

            Action action = () => target.Add((ICompilerModule) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithConfigurationModuleAddsRuleToCompilerTest()
        {
            var module = Substitute.For<IConfigurationModule>();

            var target = new BuildConfiguration();

            target.Add(module);

            module.Received().Configure(target);
        }

        [Fact]
        public void AddWithCreationRuleAddsRuleToCompilerTest()
        {
            var rule = new CreationRule(typeof(Person), "FirstName", Environment.TickCount, (object) null);

            var target = new BuildConfiguration();

            target.Add(rule);

            target.CreationRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithCreationRuleThrowsExceptionWithNullCompilerTest()
        {
            var rule = new CreationRule(typeof(Person), "FirstName", Environment.TickCount, (object) null);

            Action action = () => BuildStrategyCompilerExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithCreationRuleThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildStrategyCompiler>();

            Action action = () => target.Add((CreationRule) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithExecuteOrderRuleAddsRuleToCompilerTest()
        {
            var rule = new ExecuteOrderRule(typeof(Person), typeof(string), "FirstName", Environment.TickCount);

            var target = new BuildConfiguration();

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

            Action action = () => target.Add((ExecuteOrderRule) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithIgnoreRuleAddsRuleToCompilerTest()
        {
            var rule = new IgnoreRule(typeof(Person), "FirstName");

            var target = new BuildConfiguration();

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

            Action action = () => target.Add((IgnoreRule) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithPostBuildActionAddsRuleToCompilerTest()
        {
            var postBuildAction = new DummyPostBuildAction();

            var target = new BuildConfiguration();

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

            Action action = () => target.Add((IPostBuildAction) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeCreatorAddsRuleToCompilerTest()
        {
            var rule = new DefaultTypeCreator();

            var target = new BuildConfiguration();

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

            Action action = () => target.Add((ITypeCreator) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeMappingRuleAddsRuleToCompilerTest()
        {
            var rule = new TypeMappingRule(typeof(Stream), typeof(MemoryStream));

            var target = new BuildConfiguration();

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

            Action action = () => target.Add((TypeMappingRule) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithValueGeneratorAddsRuleToCompilerTest()
        {
            var rule = new StringValueGenerator();

            var target = new BuildConfiguration();

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

            Action action = () => target.Add((IValueGenerator) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveCreationRuleRemovesMultipleMatchingRulesFromCompilerTest()
        {
            var target = new BuildConfiguration();

            target.AddCreationRule<DummyCreationRule>();
            target.AddCreationRule<DummyCreationRule>();
            target.AddCreationRule<DummyCreationRule>();
            target.RemoveCreationRule<DummyCreationRule>();

            target.CreationRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveCreationRuleRemovesRulesFromCompilerTest()
        {
            var target = new BuildConfiguration();

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
            var target = new BuildConfiguration();

            target.AddExecuteOrderRule<DummyExecuteOrderRule>();
            target.AddExecuteOrderRule<DummyExecuteOrderRule>();
            target.AddExecuteOrderRule<DummyExecuteOrderRule>();
            target.RemoveExecuteOrderRule<DummyExecuteOrderRule>();

            target.ExecuteOrderRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveExecuteOrderRuleRemovesRulesFromCompilerTest()
        {
            var target = new BuildConfiguration();

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
            var target = new BuildConfiguration();

            target.AddIgnoreRule<DummyIgnoreRule>();
            target.AddIgnoreRule<DummyIgnoreRule>();
            target.AddIgnoreRule<DummyIgnoreRule>();
            target.RemoveIgnoreRule<DummyIgnoreRule>();

            target.IgnoreRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveIgnoreRuleRemovesRulesFromCompilerTest()
        {
            var target = new BuildConfiguration();

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
            var target = new BuildConfiguration();

            target.AddPostBuildAction<DummyPostBuildAction>();
            target.AddPostBuildAction<DummyPostBuildAction>();
            target.AddPostBuildAction<DummyPostBuildAction>();
            target.RemovePostBuildAction<DummyPostBuildAction>();

            target.PostBuildActions.Should().BeEmpty();
        }

        [Fact]
        public void RemovePostBuildActionRemovesRulesFromCompilerTest()
        {
            var target = new BuildConfiguration();

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
            var target = new BuildConfiguration();

            target.AddTypeCreator<DefaultTypeCreator>();
            target.AddTypeCreator<DefaultTypeCreator>();
            target.AddTypeCreator<DefaultTypeCreator>();
            target.RemoveTypeCreator<DefaultTypeCreator>();

            target.TypeCreators.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTypeCreatorRemovesRulesFromCompilerTest()
        {
            var target = new BuildConfiguration();

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
            var target = new BuildConfiguration();

            target.AddTypeMappingRule<DummyTypeMappingRule>();
            target.AddTypeMappingRule<DummyTypeMappingRule>();
            target.AddTypeMappingRule<DummyTypeMappingRule>();
            target.RemoveTypeMappingRule<DummyTypeMappingRule>();

            target.TypeMappingRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTypeMappingRuleRemovesRulesFromCompilerTest()
        {
            var target = new BuildConfiguration();

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
            var target = new BuildConfiguration();

            target.AddValueGenerator<StringValueGenerator>();
            target.AddValueGenerator<StringValueGenerator>();
            target.AddValueGenerator<StringValueGenerator>();
            target.RemoveValueGenerator<StringValueGenerator>();

            target.ValueGenerators.Should().BeEmpty();
        }

        [Fact]
        public void RemoveValueGeneratorRemovesRulesFromCompilerTest()
        {
            var target = new BuildConfiguration();

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
    }
}
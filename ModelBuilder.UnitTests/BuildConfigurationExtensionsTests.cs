namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using ModelBuilder.CreationRules;
    using ModelBuilder.ExecuteOrderRules;
    using ModelBuilder.IgnoreRules;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class BuildConfigurationExtensionsTests
    {
        [Fact]
        public void AddConfigurationModuleAddsRuleToCompiler()
        {
            var target = new BuildConfiguration();

            var config = target.UsingModule<TestConfigurationModule>();

            config.Should().BeSameAs(target);

            var actual = target.PostBuildActions.OfType<DummyPostBuildAction>();

            actual.Should().NotBeEmpty();
        }

        [Fact]
        public void AddConfigurationModuleThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.UsingModule<TestConfigurationModule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddCreationRuleAddsRuleToCompiler()
        {
            var target = new BuildConfiguration();

            target.AddCreationRule<DummyCreationRule>();

            var actual = target.CreationRules.Single();

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

            var target = new BuildConfiguration();

            target.AddCreationRule<Person>(x => x.FirstName, priority, value);

            var rule = target.CreationRules.Single();

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

            var target = new BuildConfiguration();

            Action action = () => target.AddCreationRule((Expression<Func<Person, object>>) null, priority, value);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleAddsRuleToCompiler()
        {
            var target = new BuildConfiguration();

            target.AddExecuteOrderRule<DummyExecuteOrderRule>();

            var actual = target.ExecuteOrderRules.Single();

            actual.Should().BeOfType<DummyExecuteOrderRule>();
        }

        [Fact]
        public void AddExecuteOrderRuleThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.AddExecuteOrderRule<DummyExecuteOrderRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithExpressionAddsRuleToCompiler()
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
        public void AddExecuteOrderRuleWithExpressionThrowsExceptionWithNullCompiler()
        {
            var priority = Environment.TickCount;

            Action action = () =>
                BuildConfigurationExtensions.AddExecuteOrderRule<Person>(null, x => x.FirstName, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithExpressionThrowsExceptionWithNullExpression()
        {
            var priority = Environment.TickCount;

            var target = new BuildConfiguration();

            Action action = () => target.AddExecuteOrderRule((Expression<Func<Person, object>>) null, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithPredicateAddsRuleToCompiler()
        {
            var priority = Environment.TickCount;

            var target = new BuildConfiguration();

            target.AddExecuteOrderRule(x => x.Name == nameof(Person.FirstName), priority);

            var rule = target.ExecuteOrderRules.Single();

            rule.Priority.Should().Be(priority);

            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var actual = rule.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void AddExecuteOrderRuleWithPredicateThrowsExceptionWithNullCompiler()
        {
            var priority = Environment.TickCount;

            Action action = () =>
                BuildConfigurationExtensions.AddExecuteOrderRule(null, x => x.Name == nameof(Person.FirstName),
                    priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithPredicateThrowsExceptionWithNullPredicate()
        {
            var priority = Environment.TickCount;

            var target = new BuildConfiguration();

            Action action = () => target.AddExecuteOrderRule((Predicate<PropertyInfo>) null, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithRegexAddsRuleToCompiler()
        {
            var priority = Environment.TickCount;

            var target = new BuildConfiguration();

            var actual = target.AddExecuteOrderRule(PropertyExpression.FirstName, priority);

            actual.Should().Be(target);

            var rule = target.ExecuteOrderRules.Single();

            rule.Should().BeOfType<RegexExecuteOrderRule>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithRegexThrowsExceptionWithNullCompiler()
        {
            var priority = Environment.TickCount;

            Action action = () =>
                BuildConfigurationExtensions.AddExecuteOrderRule(null, PropertyExpression.LastName, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithRegexThrowsExceptionWithNullExpression()
        {
            var priority = Environment.TickCount;

            var target = new BuildConfiguration();

            Action action = () => target.AddExecuteOrderRule((Regex) null, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleAddsRuleToCompiler()
        {
            var target = new BuildConfiguration();

            target.AddIgnoreRule<DummyIgnoreRule>();

            var actual = target.IgnoreRules.Single();

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
            var target = new BuildConfiguration();

            var actual = target.AddIgnoreRule<Person>(x => x.FirstName);

            actual.Should().Be(target);

            var rule = target.IgnoreRules.Single();

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
            var target = new BuildConfiguration();

            Action action = () => target.AddIgnoreRule((Expression<Func<Person, object>>) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithPredicateAddsRuleToCompiler()
        {
            var target = new BuildConfiguration();

            var actual = target.AddIgnoreRule(x => x.PropertyType == typeof(string));

            actual.Should().Be(target);

            var rule = target.IgnoreRules.Single();

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
            var target = new BuildConfiguration();

            Action action = () => target.AddIgnoreRule((Predicate<PropertyInfo>) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithRegexAddsRuleToCompiler()
        {
            var target = new BuildConfiguration();

            var actual = target.AddIgnoreRule(PropertyExpression.FirstName);

            actual.Should().Be(target);

            var rule = target.IgnoreRules.Single();

            rule.Should().BeOfType<RegexIgnoreRule>();
        }

        [Fact]
        public void AddIgnoreRuleWithRegexThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.AddIgnoreRule(null, PropertyExpression.LastName);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithRegexThrowsExceptionWithNullExpression()
        {
            var target = new BuildConfiguration();

            Action action = () => target.AddIgnoreRule((Regex) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddPostBuildActionAddsRuleToCompiler()
        {
            var target = new BuildConfiguration();

            target.AddPostBuildAction<DummyPostBuildAction>();

            var actual = target.PostBuildActions.Single();

            actual.Should().BeOfType<DummyPostBuildAction>();
        }

        [Fact]
        public void AddPostBuildActionThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.AddPostBuildAction<DummyPostBuildAction>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddTypeCreatorAddsRuleToCompiler()
        {
            var target = new BuildConfiguration();

            target.AddTypeCreator<DefaultTypeCreator>();

            var actual = target.TypeCreators.Single();

            actual.Should().BeOfType<DefaultTypeCreator>();
        }

        [Fact]
        public void AddTypeCreatorThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.AddTypeCreator<DefaultTypeCreator>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddTypeMappingRuleAddsRuleToCompiler()
        {
            var target = new BuildConfiguration();

            target.AddTypeMappingRule<DummyTypeMappingRule>();

            var actual = target.TypeMappingRules.Single();

            actual.Should().BeOfType<DummyTypeMappingRule>();
        }

        [Fact]
        public void AddTypeMappingRuleThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.AddTypeMappingRule<DummyTypeMappingRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddValueGeneratorAddsRuleToCompiler()
        {
            var target = new BuildConfiguration();

            target.AddValueGenerator<StringValueGenerator>();

            var actual = target.ValueGenerators.Single();

            actual.Should().BeOfType<StringValueGenerator>();
        }

        [Fact]
        public void AddValueGeneratorThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.AddValueGenerator<StringValueGenerator>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithCompilerModuleThrowsExceptionWithNullCompiler()
        {
            var module = new TestConfigurationModule();

            Action action = () => BuildConfigurationExtensions.Add(null, module);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithCompilerModuleThrowsExceptionWithNullRule()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Add((IConfigurationModule) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithConfigurationModuleAddsRuleToCompiler()
        {
            var module = Substitute.For<IConfigurationModule>();

            var target = new BuildConfiguration();

            target.Add(module);

            module.Received().Configure(target);
        }

        [Fact]
        public void AddWithCreationRuleAddsRuleToCompiler()
        {
            var rule = new ExpressionCreationRule<Person>(x => x.FirstName, (object) null, Environment.TickCount);

            var target = new BuildConfiguration();

            target.Add(rule);

            target.CreationRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithCreationRuleThrowsExceptionWithNullCompiler()
        {
            var rule = new ExpressionCreationRule<Person>(x => x.FirstName, (object)null, Environment.TickCount);

            Action action = () => BuildConfigurationExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithCreationRuleThrowsExceptionWithNullRule()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Add((ICreationRule) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithExecuteOrderRuleAddsRuleToCompiler()
        {
            var rule = new PredicateExecuteOrderRule(x => x.Name == "FirstName", Environment.TickCount);

            var target = new BuildConfiguration();

            target.Add(rule);

            target.ExecuteOrderRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithExecuteOrderRuleThrowsExceptionWithNullCompiler()
        {
            var rule = new PredicateExecuteOrderRule(x => x.Name == "FirstName", Environment.TickCount);

            Action action = () => BuildConfigurationExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithExecuteOrderRuleThrowsExceptionWithNullRule()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Add((PredicateExecuteOrderRule) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithIgnoreRuleAddsRuleToCompiler()
        {
            var rule = new ExpressionIgnoreRule<Person>(x => x.FirstName);

            var target = new BuildConfiguration();

            target.Add(rule);

            target.IgnoreRules.Should().Contain(rule);
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
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Add((IIgnoreRule) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithPostBuildActionAddsRuleToCompiler()
        {
            var postBuildAction = new DummyPostBuildAction();

            var target = new BuildConfiguration();

            target.Add(postBuildAction);

            target.PostBuildActions.Should().Contain(postBuildAction);
        }

        [Fact]
        public void AddWithPostBuildActionThrowsExceptionWithNullCompiler()
        {
            var postBuildAction = new DummyPostBuildAction();

            Action action = () => BuildConfigurationExtensions.Add(null, postBuildAction);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithPostBuildActionThrowsExceptionWithNullRule()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Add((IPostBuildAction) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeCreatorAddsRuleToCompiler()
        {
            var rule = new DefaultTypeCreator();

            var target = new BuildConfiguration();

            target.Add(rule);

            target.TypeCreators.Should().Contain(rule);
        }

        [Fact]
        public void AddWithTypeCreatorThrowsExceptionWithNullCompiler()
        {
            var rule = new DefaultTypeCreator();

            Action action = () => BuildConfigurationExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeCreatorThrowsExceptionWithNullRule()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Add((ITypeCreator) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeMappingRuleAddsRuleToCompiler()
        {
            var rule = new TypeMappingRule(typeof(Stream), typeof(MemoryStream));

            var target = new BuildConfiguration();

            target.Add(rule);

            target.TypeMappingRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithTypeMappingRuleThrowsExceptionWithNullCompiler()
        {
            var rule = new TypeMappingRule(typeof(Stream), typeof(MemoryStream));

            Action action = () => BuildConfigurationExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeMappingRuleThrowsExceptionWithNullRule()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Add((TypeMappingRule) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithValueGeneratorAddsRuleToCompiler()
        {
            var rule = new StringValueGenerator();

            var target = new BuildConfiguration();

            target.Add(rule);

            target.ValueGenerators.Should().Contain(rule);
        }

        [Fact]
        public void AddWithValueGeneratorThrowsExceptionWithNullCompiler()
        {
            var rule = new StringValueGenerator();

            Action action = () => BuildConfigurationExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithValueGeneratorThrowsExceptionWithNullRule()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Add((IValueGenerator) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateReturnsInstanceCreatedByDefaultExecuteStrategy()
        {
            var value = Guid.NewGuid();

            var generator = Substitute.For<IValueGenerator>();
            var generators = new Collection<IValueGenerator>
            {
                generator
            };
            var target = Substitute.For<IBuildConfiguration>();

            target.ValueGenerators.Returns(generators);
            generator.IsMatch(Arg.Any<IBuildChain>(), typeof(Guid)).Returns(true);
            generator.Generate(Arg.Any<IExecuteStrategy>(), typeof(Guid)).Returns(value);

            var actual = target.Create(typeof(Guid));

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateThrowsExceptionWithNullBuildConfiguration()
        {
            Action action = () => ((IBuildConfiguration) null).Create(typeof(Guid));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullInstanceType()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Create(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateTReturnsInstanceCreatedByDefaultExecuteStrategy()
        {
            var value = Guid.NewGuid();

            var generator = Substitute.For<IValueGenerator>();
            var generators = new Collection<IValueGenerator>
            {
                generator
            };
            var target = Substitute.For<IBuildConfiguration>();

            target.ValueGenerators.Returns(generators);
            generator.IsMatch(Arg.Any<IBuildChain>(), typeof(Guid)).Returns(true);
            generator.Generate(Arg.Any<IExecuteStrategy>(), typeof(Guid)).Returns(value);

            var actual = target.Create<Guid>();

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateTThrowsExceptionWithNullBuildConfiguration()
        {
            Action action = () => ((IBuildConfiguration) null).Create<Guid>();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringReturnsNewBuildConfigurationWithIgnoreRuleAppended()
        {
            var ignoreRules = new Collection<IIgnoreRule>();

            var target = Substitute.For<IBuildConfiguration>();

            target.IgnoreRules.Returns(ignoreRules);

            var actual = target.Ignoring<Person>(x => x.Priority);

            actual.IgnoreRules.Should().NotBeEmpty();
            actual.IgnoreRules.Single().Should().BeOfType<ExpressionIgnoreRule<Person>>();
        }

        [Fact]
        public void IgnoringThrowsExceptionWithNullBuildConfiguration()
        {
            Action action = () => ((IBuildConfiguration) null).Ignoring<Person>(x => x.Priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringThrowsExceptionWithNullExpression()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Ignoring<Person>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MappingReturnsNewBuildConfigurationWithTypeMappingRuleAppended()
        {
            var typeMappings = new Collection<TypeMappingRule>();

            var target = Substitute.For<IBuildConfiguration>();

            target.TypeMappingRules.Returns(typeMappings);

            var actual = target.Mapping<Stream, MemoryStream>();

            actual.TypeMappingRules.Should().NotBeEmpty();

            var matchingRule = actual.TypeMappingRules.FirstOrDefault(
                x => x.SourceType == typeof(Stream) && x.TargetType == typeof(MemoryStream));

            matchingRule.Should().NotBeNull();
        }

        [Fact]
        public void MappingThrowsExceptionWithNullBuildConfiguration()
        {
            Action action = () => ((IBuildConfiguration) null).Mapping<Stream, MemoryStream>();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullBuildConfiguration()
        {
            var model = new Person();

            Action action = () => ((IBuildConfiguration) null).Populate(model);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstance()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Populate<Person>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateUsesDefaultExecuteStrategyToPopulateInstance()
        {
            var expected = new SlimModel();

            var target = new BuildConfiguration().UsingModule<DefaultConfigurationModule>();

            var actual = target.Populate(expected);

            actual.Should().Be(expected);
            actual.Value.Should().NotBeEmpty();
        }

        [Fact]
        public void RemoveCreationRuleRemovesMultipleMatchingRulesFromCompiler()
        {
            var target = new BuildConfiguration();

            target.AddCreationRule<DummyCreationRule>();
            target.AddCreationRule<DummyCreationRule>();
            target.AddCreationRule<DummyCreationRule>();
            target.RemoveCreationRule<DummyCreationRule>();

            target.CreationRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveCreationRuleRemovesRulesFromCompiler()
        {
            var target = new BuildConfiguration();

            target.AddCreationRule<DummyCreationRule>();
            target.RemoveCreationRule<DummyCreationRule>();

            target.CreationRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveCreationRuleThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.RemoveCreationRule<DummyCreationRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveExecuteOrderRuleRemovesMultipleMatchingRulesFromCompiler()
        {
            var target = new BuildConfiguration();

            target.AddExecuteOrderRule<DummyExecuteOrderRule>();
            target.AddExecuteOrderRule<DummyExecuteOrderRule>();
            target.AddExecuteOrderRule<DummyExecuteOrderRule>();
            target.RemoveExecuteOrderRule<DummyExecuteOrderRule>();

            target.ExecuteOrderRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveExecuteOrderRuleRemovesRulesFromCompiler()
        {
            var target = new BuildConfiguration();

            target.AddExecuteOrderRule<DummyExecuteOrderRule>();
            target.RemoveExecuteOrderRule<DummyExecuteOrderRule>();

            target.ExecuteOrderRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveExecuteOrderRuleThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.RemoveExecuteOrderRule<DummyExecuteOrderRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveIgnoreRuleRemovesMultipleMatchingRulesFromCompiler()
        {
            var target = new BuildConfiguration();

            target.AddIgnoreRule<DummyIgnoreRule>();
            target.AddIgnoreRule<DummyIgnoreRule>();
            target.AddIgnoreRule<DummyIgnoreRule>();
            target.RemoveIgnoreRule<DummyIgnoreRule>();

            target.IgnoreRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveIgnoreRuleRemovesRulesFromCompiler()
        {
            var target = new BuildConfiguration();

            target.AddIgnoreRule<DummyIgnoreRule>();
            target.RemoveIgnoreRule<DummyIgnoreRule>();

            target.IgnoreRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveIgnoreRuleThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.RemoveIgnoreRule<DummyIgnoreRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemovePostBuildActionRemovesMultipleMatchingRulesFromCompiler()
        {
            var target = new BuildConfiguration();

            target.AddPostBuildAction<DummyPostBuildAction>();
            target.AddPostBuildAction<DummyPostBuildAction>();
            target.AddPostBuildAction<DummyPostBuildAction>();
            target.RemovePostBuildAction<DummyPostBuildAction>();

            target.PostBuildActions.Should().BeEmpty();
        }

        [Fact]
        public void RemovePostBuildActionRemovesRulesFromCompiler()
        {
            var target = new BuildConfiguration();

            target.AddPostBuildAction<DummyPostBuildAction>();
            target.RemovePostBuildAction<DummyPostBuildAction>();

            target.PostBuildActions.Should().BeEmpty();
        }

        [Fact]
        public void RemovePostBuildActionThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.RemovePostBuildAction<DummyPostBuildAction>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveTypeCreatorRemovesMultipleMatchingRulesFromCompiler()
        {
            var target = new BuildConfiguration();

            target.AddTypeCreator<DefaultTypeCreator>();
            target.AddTypeCreator<DefaultTypeCreator>();
            target.AddTypeCreator<DefaultTypeCreator>();
            target.RemoveTypeCreator<DefaultTypeCreator>();

            target.TypeCreators.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTypeCreatorRemovesRulesFromCompiler()
        {
            var target = new BuildConfiguration();

            target.AddTypeCreator<DefaultTypeCreator>();
            target.RemoveTypeCreator<DefaultTypeCreator>();

            target.TypeCreators.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTypeCreatorThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.RemoveTypeCreator<DefaultTypeCreator>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveTypeMappingRuleRemovesMultipleMatchingRulesFromCompiler()
        {
            var target = new BuildConfiguration();

            target.AddTypeMappingRule<DummyTypeMappingRule>();
            target.AddTypeMappingRule<DummyTypeMappingRule>();
            target.AddTypeMappingRule<DummyTypeMappingRule>();
            target.RemoveTypeMappingRule<DummyTypeMappingRule>();

            target.TypeMappingRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTypeMappingRuleRemovesRulesFromCompiler()
        {
            var target = new BuildConfiguration();

            target.AddTypeMappingRule<DummyTypeMappingRule>();
            target.RemoveTypeMappingRule<DummyTypeMappingRule>();

            target.TypeMappingRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTypeMappingRuleThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.RemoveTypeMappingRule<DummyTypeMappingRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RemoveValueGeneratorRemovesMultipleMatchingRulesFromCompiler()
        {
            var target = new BuildConfiguration();

            target.AddValueGenerator<StringValueGenerator>();
            target.AddValueGenerator<StringValueGenerator>();
            target.AddValueGenerator<StringValueGenerator>();
            target.RemoveValueGenerator<StringValueGenerator>();

            target.ValueGenerators.Should().BeEmpty();
        }

        [Fact]
        public void RemoveValueGeneratorRemovesRulesFromCompiler()
        {
            var target = new BuildConfiguration();

            target.AddValueGenerator<StringValueGenerator>();
            target.RemoveValueGenerator<StringValueGenerator>();

            target.ValueGenerators.Should().BeEmpty();
        }

        [Fact]
        public void RemoveValueGeneratorThrowsExceptionWithNullCompiler()
        {
            Action action = () => BuildConfigurationExtensions.RemoveValueGenerator<StringValueGenerator>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UsingExecuteStrategyReturnsExecuteStrategyWithBuildConfiguration()
        {
            var target = Substitute.For<IBuildConfiguration>();

            var actual = target.UsingExecuteStrategy<DefaultExecuteStrategy<Person>>();

            actual.Configuration.Should().BeSameAs(target);
            actual.Log.Should().BeOfType<DefaultBuildLog>();
        }

        [Fact]
        public void UsingExecuteStrategyThrowsExceptionWithNullConfiguration()
        {
            Action action = () => ((IBuildConfiguration) null).UsingExecuteStrategy<NullExecuteStrategy>();

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
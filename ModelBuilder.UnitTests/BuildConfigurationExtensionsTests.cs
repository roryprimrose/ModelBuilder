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
            var sut = new BuildConfiguration();

            var config = sut.UsingModule<TestConfigurationModule>();

            config.Should().BeSameAs(sut);

            var actual = sut.PostBuildActions.OfType<DummyPostBuildAction>();

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
        public void AddExecuteOrderRuleAddsRuleToCompiler()
        {
            var sut = new BuildConfiguration();

            sut.AddExecuteOrderRule<DummyExecuteOrderRule>();

            var actual = sut.ExecuteOrderRules.Single();

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

            var sut = new BuildConfiguration();

            sut.AddExecuteOrderRule<Person>(x => x.FirstName, priority);

            var rule = sut.ExecuteOrderRules.Single();

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

            var sut = new BuildConfiguration();

            Action action = () => sut.AddExecuteOrderRule((Expression<Func<Person, object>>) null, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithPredicateAddsRuleToCompiler()
        {
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            sut.AddExecuteOrderRule(x => x.Name == nameof(Person.FirstName), priority);

            var rule = sut.ExecuteOrderRules.Single();

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

            var sut = new BuildConfiguration();

            Action action = () => sut.AddExecuteOrderRule((Predicate<PropertyInfo>) null, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithRegexAddsRuleToCompiler()
        {
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            var actual = sut.AddExecuteOrderRule(NameExpression.FirstName, priority);

            actual.Should().Be(sut);

            var rule = sut.ExecuteOrderRules.Single();

            rule.Should().BeOfType<RegexExecuteOrderRule>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithRegexThrowsExceptionWithNullCompiler()
        {
            var priority = Environment.TickCount;

            Action action = () =>
                BuildConfigurationExtensions.AddExecuteOrderRule(null, NameExpression.LastName, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithRegexThrowsExceptionWithNullExpression()
        {
            var priority = Environment.TickCount;

            var sut = new BuildConfiguration();

            Action action = () => sut.AddExecuteOrderRule((Regex) null, priority);

            action.Should().Throw<ArgumentNullException>();
        }

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
        public void AddPostBuildActionAddsRuleToCompiler()
        {
            var sut = new BuildConfiguration();

            sut.AddPostBuildAction<DummyPostBuildAction>();

            var actual = sut.PostBuildActions.Single();

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
            var sut = new BuildConfiguration();

            sut.AddTypeCreator<DefaultTypeCreator>();

            var actual = sut.TypeCreators.Single();

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
            var sut = new BuildConfiguration();

            sut.AddTypeMappingRule<DummyTypeMappingRule>();

            var actual = sut.TypeMappingRules.Single();

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
            var sut = new BuildConfiguration();

            sut.AddValueGenerator<StringValueGenerator>();

            var actual = sut.ValueGenerators.Single();

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
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Add((IConfigurationModule) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithConfigurationModuleAddsRuleToCompiler()
        {
            var module = Substitute.For<IConfigurationModule>();

            var sut = new BuildConfiguration();

            sut.Add(module);

            module.Received().Configure(sut);
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
            var rule = new ExpressionCreationRule<Person>(x => x.FirstName, (object)null, Environment.TickCount);

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
        public void AddWithExecuteOrderRuleAddsRuleToCompiler()
        {
            var rule = new PropertyPredicateExecuteOrderRule(x => x.Name == "FirstName", Environment.TickCount);

            var sut = new BuildConfiguration();

            sut.Add(rule);

            sut.ExecuteOrderRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithExecuteOrderRuleThrowsExceptionWithNullCompiler()
        {
            var rule = new PropertyPredicateExecuteOrderRule(x => x.Name == "FirstName", Environment.TickCount);

            Action action = () => BuildConfigurationExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithExecuteOrderRuleThrowsExceptionWithNullRule()
        {
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Add((PropertyPredicateExecuteOrderRule) null);

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
        public void AddWithPostBuildActionAddsRuleToCompiler()
        {
            var postBuildAction = new DummyPostBuildAction();

            var sut = new BuildConfiguration();

            sut.Add(postBuildAction);

            sut.PostBuildActions.Should().Contain(postBuildAction);
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
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Add((IPostBuildAction) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeCreatorAddsRuleToCompiler()
        {
            var rule = new DefaultTypeCreator();

            var sut = new BuildConfiguration();

            sut.Add(rule);

            sut.TypeCreators.Should().Contain(rule);
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
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Add((ITypeCreator) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeMappingRuleAddsRuleToCompiler()
        {
            var rule = new TypeMappingRule(typeof(Stream), typeof(MemoryStream));

            var sut = new BuildConfiguration();

            sut.Add(rule);

            sut.TypeMappingRules.Should().Contain(rule);
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
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Add((TypeMappingRule) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithValueGeneratorAddsRuleToCompiler()
        {
            var rule = new StringValueGenerator();

            var sut = new BuildConfiguration();

            sut.Add(rule);

            sut.ValueGenerators.Should().Contain(rule);
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
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Add((IValueGenerator) null);

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
            var sut = Substitute.For<IBuildConfiguration>();

            sut.ValueGenerators.Returns(generators);
            generator.IsMatch(Arg.Any<IBuildChain>(), typeof(Guid)).Returns(true);
            generator.Generate(Arg.Any<IExecuteStrategy>(), typeof(Guid)).Returns(value);

            var actual = sut.Create(typeof(Guid));

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
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Create(null);

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
            var sut = Substitute.For<IBuildConfiguration>();

            sut.ValueGenerators.Returns(generators);
            generator.IsMatch(Arg.Any<IBuildChain>(), typeof(Guid)).Returns(true);
            generator.Generate(Arg.Any<IExecuteStrategy>(), typeof(Guid)).Returns(value);

            var actual = sut.Create<Guid>();

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

            var sut = Substitute.For<IBuildConfiguration>();

            sut.IgnoreRules.Returns(ignoreRules);

            var actual = sut.Ignoring<Person>(x => x.Priority);

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
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Ignoring<Person>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MappingReturnsNewBuildConfigurationWithTypeMappingRuleAppended()
        {
            var typeMappings = new Collection<TypeMappingRule>();

            var sut = Substitute.For<IBuildConfiguration>();

            sut.TypeMappingRules.Returns(typeMappings);

            var actual = sut.Mapping<Stream, MemoryStream>();

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
            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.Populate<Person>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateUsesDefaultExecuteStrategyToPopulateInstance()
        {
            var expected = new SlimModel();

            var sut = new BuildConfiguration().UsingModule<DefaultConfigurationModule>();

            var actual = sut.Populate(expected);

            actual.Should().Be(expected);
            actual.Value.Should().NotBeEmpty();
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
        public void RemoveExecuteOrderRuleRemovesMultipleMatchingRulesFromCompiler()
        {
            var sut = new BuildConfiguration();

            sut.AddExecuteOrderRule<DummyExecuteOrderRule>();
            sut.AddExecuteOrderRule<DummyExecuteOrderRule>();
            sut.AddExecuteOrderRule<DummyExecuteOrderRule>();
            sut.RemoveExecuteOrderRule<DummyExecuteOrderRule>();

            sut.ExecuteOrderRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveExecuteOrderRuleRemovesRulesFromCompiler()
        {
            var sut = new BuildConfiguration();

            sut.AddExecuteOrderRule<DummyExecuteOrderRule>();
            sut.RemoveExecuteOrderRule<DummyExecuteOrderRule>();

            sut.ExecuteOrderRules.Should().BeEmpty();
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
        public void RemovePostBuildActionRemovesMultipleMatchingRulesFromCompiler()
        {
            var sut = new BuildConfiguration();

            sut.AddPostBuildAction<DummyPostBuildAction>();
            sut.AddPostBuildAction<DummyPostBuildAction>();
            sut.AddPostBuildAction<DummyPostBuildAction>();
            sut.RemovePostBuildAction<DummyPostBuildAction>();

            sut.PostBuildActions.Should().BeEmpty();
        }

        [Fact]
        public void RemovePostBuildActionRemovesRulesFromCompiler()
        {
            var sut = new BuildConfiguration();

            sut.AddPostBuildAction<DummyPostBuildAction>();
            sut.RemovePostBuildAction<DummyPostBuildAction>();

            sut.PostBuildActions.Should().BeEmpty();
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
            var sut = new BuildConfiguration();

            sut.AddTypeCreator<DefaultTypeCreator>();
            sut.AddTypeCreator<DefaultTypeCreator>();
            sut.AddTypeCreator<DefaultTypeCreator>();
            sut.RemoveTypeCreator<DefaultTypeCreator>();

            sut.TypeCreators.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTypeCreatorRemovesRulesFromCompiler()
        {
            var sut = new BuildConfiguration();

            sut.AddTypeCreator<DefaultTypeCreator>();
            sut.RemoveTypeCreator<DefaultTypeCreator>();

            sut.TypeCreators.Should().BeEmpty();
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
            var sut = new BuildConfiguration();

            sut.AddTypeMappingRule<DummyTypeMappingRule>();
            sut.AddTypeMappingRule<DummyTypeMappingRule>();
            sut.AddTypeMappingRule<DummyTypeMappingRule>();
            sut.RemoveTypeMappingRule<DummyTypeMappingRule>();

            sut.TypeMappingRules.Should().BeEmpty();
        }

        [Fact]
        public void RemoveTypeMappingRuleRemovesRulesFromCompiler()
        {
            var sut = new BuildConfiguration();

            sut.AddTypeMappingRule<DummyTypeMappingRule>();
            sut.RemoveTypeMappingRule<DummyTypeMappingRule>();

            sut.TypeMappingRules.Should().BeEmpty();
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
            var sut = new BuildConfiguration();

            sut.AddValueGenerator<StringValueGenerator>();
            sut.AddValueGenerator<StringValueGenerator>();
            sut.AddValueGenerator<StringValueGenerator>();
            sut.RemoveValueGenerator<StringValueGenerator>();

            sut.ValueGenerators.Should().BeEmpty();
        }

        [Fact]
        public void RemoveValueGeneratorRemovesRulesFromCompiler()
        {
            var sut = new BuildConfiguration();

            sut.AddValueGenerator<StringValueGenerator>();
            sut.RemoveValueGenerator<StringValueGenerator>();

            sut.ValueGenerators.Should().BeEmpty();
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
            var sut = Substitute.For<IBuildConfiguration>();

            var actual = sut.UsingExecuteStrategy<DefaultExecuteStrategy<Person>>();

            actual.Configuration.Should().BeSameAs(sut);
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
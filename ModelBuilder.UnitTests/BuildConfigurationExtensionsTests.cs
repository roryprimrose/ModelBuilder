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
        public void AddConfigurationModuleAddsRuleToCompilerTest()
        {
            var target = new BuildConfiguration();

            var config = target.UsingModule<TestConfigurationModule>();

            config.Should().BeSameAs(target);

            var actual = target.PostBuildActions.OfType<DummyPostBuildAction>();

            actual.Should().NotBeEmpty();
        }

        [Fact]
        public void AddConfigurationModuleThrowsExceptionWithNullCompilerTest()
        {
            Action action = () => BuildConfigurationExtensions.UsingModule<TestConfigurationModule>(null);

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
            Action action = () => BuildConfigurationExtensions.AddCreationRule<DummyCreationRule>(null);

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
                BuildConfigurationExtensions.AddCreationRule<Person>(null, x => x.FirstName, priority, value);

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
            Action action = () => BuildConfigurationExtensions.AddExecuteOrderRule<DummyExecuteOrderRule>(null);

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
                BuildConfigurationExtensions.AddExecuteOrderRule<Person>(null, x => x.FirstName, priority);

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
        public void AddExecuteOrderRuleWithPredicateAddsRuleToCompilerTest()
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
        public void AddExecuteOrderRuleWithPredicateThrowsExceptionWithNullCompilerTest()
        {
            var priority = Environment.TickCount;

            Action action = () =>
                BuildConfigurationExtensions.AddExecuteOrderRule(null, x => x.Name == nameof(Person.FirstName),
                    priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithPredicateThrowsExceptionWithNullPredicateTest()
        {
            var priority = Environment.TickCount;

            var target = new BuildConfiguration();

            Action action = () => target.AddExecuteOrderRule((Predicate<PropertyInfo>) null, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithRegexAddsRuleToCompilerTest()
        {
            var priority = Environment.TickCount;

            var target = new BuildConfiguration();

            var actual = target.AddExecuteOrderRule(PropertyExpression.FirstName, priority);

            actual.Should().Be(target);

            var rule = target.ExecuteOrderRules.Single();

            rule.Should().BeOfType<RegexExecuteOrderRule>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithRegexThrowsExceptionWithNullCompilerTest()
        {
            var priority = Environment.TickCount;

            Action action = () =>
                BuildConfigurationExtensions.AddExecuteOrderRule(null, PropertyExpression.LastName, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddExecuteOrderRuleWithRegexThrowsExceptionWithNullExpressionTest()
        {
            var priority = Environment.TickCount;

            var target = new BuildConfiguration();

            Action action = () => target.AddExecuteOrderRule((Regex) null, priority);

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
            Action action = () => BuildConfigurationExtensions.AddIgnoreRule<DummyIgnoreRule>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithExpressionAddsRuleToCompilerTest()
        {
            var target = new BuildConfiguration();

            var actual = target.AddIgnoreRule<Person>(x => x.FirstName);

            actual.Should().Be(target);

            var rule = target.IgnoreRules.Single();

            rule.Should().BeOfType<ExpressionIgnoreRule<Person>>();
        }

        [Fact]
        public void AddIgnoreRuleWithExpressionThrowsExceptionWithNullCompilerTest()
        {
            Action action = () => BuildConfigurationExtensions.AddIgnoreRule<Person>(null, x => x.FirstName);

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
        public void AddIgnoreRuleWithPredicateAddsRuleToCompilerTest()
        {
            var target = new BuildConfiguration();

            var actual = target.AddIgnoreRule(x => x.PropertyType == typeof(string));

            actual.Should().Be(target);

            var rule = target.IgnoreRules.Single();

            rule.Should().BeOfType<PredicateIgnoreRule>();
        }

        [Fact]
        public void AddIgnoreRuleWithPredicateThrowsExceptionWithNullCompilerTest()
        {
            Action action = () =>
                BuildConfigurationExtensions.AddIgnoreRule(null, info => info.PropertyType == typeof(string));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithPredicateThrowsExceptionWithNullExpressionTest()
        {
            var target = new BuildConfiguration();

            Action action = () => target.AddIgnoreRule((Predicate<PropertyInfo>) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithRegexAddsRuleToCompilerTest()
        {
            var target = new BuildConfiguration();

            var actual = target.AddIgnoreRule(PropertyExpression.FirstName);

            actual.Should().Be(target);

            var rule = target.IgnoreRules.Single();

            rule.Should().BeOfType<RegexIgnoreRule>();
        }

        [Fact]
        public void AddIgnoreRuleWithRegexThrowsExceptionWithNullCompilerTest()
        {
            Action action = () => BuildConfigurationExtensions.AddIgnoreRule(null, PropertyExpression.LastName);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddIgnoreRuleWithRegexThrowsExceptionWithNullExpressionTest()
        {
            var target = new BuildConfiguration();

            Action action = () => target.AddIgnoreRule((Regex) null);

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
            Action action = () => BuildConfigurationExtensions.AddPostBuildAction<DummyPostBuildAction>(null);

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
            Action action = () => BuildConfigurationExtensions.AddTypeCreator<DefaultTypeCreator>(null);

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
            Action action = () => BuildConfigurationExtensions.AddTypeMappingRule<DummyTypeMappingRule>(null);

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
            Action action = () => BuildConfigurationExtensions.AddValueGenerator<StringValueGenerator>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithCompilerModuleThrowsExceptionWithNullCompilerTest()
        {
            var module = new TestConfigurationModule();

            Action action = () => BuildConfigurationExtensions.Add(null, module);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithCompilerModuleThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Add((IConfigurationModule) null);

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

            Action action = () => BuildConfigurationExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithCreationRuleThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Add((CreationRule) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithExecuteOrderRuleAddsRuleToCompilerTest()
        {
            var rule = new PredicateExecuteOrderRule(x => x.Name == "FirstName", Environment.TickCount);

            var target = new BuildConfiguration();

            target.Add(rule);

            target.ExecuteOrderRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithExecuteOrderRuleThrowsExceptionWithNullCompilerTest()
        {
            var rule = new PredicateExecuteOrderRule(x => x.Name == "FirstName", Environment.TickCount);

            Action action = () => BuildConfigurationExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithExecuteOrderRuleThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Add((PredicateExecuteOrderRule) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithIgnoreRuleAddsRuleToCompilerTest()
        {
            var rule = new ExpressionIgnoreRule<Person>(x => x.FirstName);

            var target = new BuildConfiguration();

            target.Add(rule);

            target.IgnoreRules.Should().Contain(rule);
        }

        [Fact]
        public void AddWithIgnoreRuleThrowsExceptionWithNullCompilerTest()
        {
            var rule = new ExpressionIgnoreRule<Person>(x => x.FirstName);

            Action action = () => BuildConfigurationExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithIgnoreRuleThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Add((IIgnoreRule) null);

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

            Action action = () => BuildConfigurationExtensions.Add(null, postBuildAction);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithPostBuildActionThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildConfiguration>();

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

            Action action = () => BuildConfigurationExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeCreatorThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildConfiguration>();

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

            Action action = () => BuildConfigurationExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithTypeMappingRuleThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildConfiguration>();

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

            Action action = () => BuildConfigurationExtensions.Add(null, rule);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddWithValueGeneratorThrowsExceptionWithNullRuleTest()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Add((IValueGenerator) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateReturnsInstanceCreatedByDefaultExecuteStrategyTest()
        {
            var value = Guid.NewGuid();

            var generator = Substitute.For<IValueGenerator>();
            var generators = new Collection<IValueGenerator>
            {
                generator
            };
            var target = Substitute.For<IBuildConfiguration>();

            target.ValueGenerators.Returns(generators);
            generator.IsMatch(typeof(Guid), null, Arg.Any<IBuildChain>()).Returns(true);
            generator.Generate(typeof(Guid), null, Arg.Any<IExecuteStrategy>()).Returns(value);

            var actual = target.Create(typeof(Guid));

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateThrowsExceptionWithNullBuildConfigurationTest()
        {
            Action action = () => ((IBuildConfiguration) null).Create(typeof(Guid));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullInstanceTypeTest()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Create(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateTReturnsInstanceCreatedByDefaultExecuteStrategyTest()
        {
            var value = Guid.NewGuid();

            var generator = Substitute.For<IValueGenerator>();
            var generators = new Collection<IValueGenerator>
            {
                generator
            };
            var target = Substitute.For<IBuildConfiguration>();

            target.ValueGenerators.Returns(generators);
            generator.IsMatch(typeof(Guid), null, Arg.Any<IBuildChain>()).Returns(true);
            generator.Generate(typeof(Guid), null, Arg.Any<IExecuteStrategy>()).Returns(value);

            var actual = target.Create<Guid>();

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateTThrowsExceptionWithNullBuildConfigurationTest()
        {
            Action action = () => ((IBuildConfiguration) null).Create<Guid>();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildTypeReturnsSourceWhenNoTypeMappingIsFound()
        {
            var source = typeof(string);

            var buildLog = Substitute.For<IBuildLog>();
            var sut = new BuildConfiguration().Mapping<Stream, MemoryStream>();

            var actual = sut.GetBuildType(source, buildLog);

            actual.Should().Be<string>();
        }

        [Fact]
        public void GetBuildTypeReturnsSourceWhenTypeMappingIsEmpty()
        {
            var source = typeof(string);

            var buildLog = Substitute.For<IBuildLog>();
            var sut = new BuildConfiguration();

            var actual = sut.GetBuildType(source, buildLog);

            actual.Should().Be<string>();
        }

        [Fact]
        public void GetBuildTypeReturnsSourceWhenTypeMappingIsNull()
        {
            var source = typeof(string);

            var buildLog = Substitute.For<IBuildLog>();
            var sut = Substitute.For<IBuildConfiguration>();

            var actual = sut.GetBuildType(source, buildLog);

            actual.Should().Be<string>();
        }

        [Fact]
        public void GetBuildTypeReturnsTargetTypeWhenSourceIsAbstractClass()
        {
            var source = typeof(Entity);

            var buildLog = Substitute.For<IBuildLog>();
            var sut = new BuildConfiguration();

            var actual = sut.GetBuildType(source, buildLog);

            actual.Should().Be<Person>();
        }

        [Fact]
        public void GetBuildTypeReturnsTargetTypeWhenSourceIsInterface()
        {
            var source = typeof(ITestItem);

            var buildLog = Substitute.For<IBuildLog>();
            var sut = new BuildConfiguration();

            var actual = sut.GetBuildType(source, buildLog);

            actual.Should().Be<TestItem>();
        }

        [Fact]
        public void GetBuildTypeReturnsTargetTypeWhenTypeMappingFound()
        {
            var source = typeof(Stream);

            var buildLog = Substitute.For<IBuildLog>();
            var sut = new BuildConfiguration().Mapping<Stream, MemoryStream>();

            var actual = sut.GetBuildType(source, buildLog);

            actual.Should().Be<MemoryStream>();
        }

        [Fact]
        public void GetBuildTypeThrowsExceptionWithNullBuildLog()
        {
            var source = typeof(string);

            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.GetBuildType(source, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildTypeThrowsExceptionWithNullConfiguration()
        {
            var source = typeof(string);

            var buildLog = Substitute.For<IBuildLog>();

            IBuildConfiguration config = null;

            Action action = () => config.GetBuildType(source, buildLog);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildTypeThrowsExceptionWithNullSourceType()
        {
            var buildLog = Substitute.For<IBuildLog>();

            var sut = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.GetBuildType(null, buildLog);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringReturnsNewBuildConfigurationWithIgnoreRuleAppendedTest()
        {
            var ignoreRules = new Collection<IIgnoreRule>();

            var target = Substitute.For<IBuildConfiguration>();

            target.IgnoreRules.Returns(ignoreRules);

            var actual = target.Ignoring<Person>(x => x.Priority);

            actual.IgnoreRules.Should().NotBeEmpty();
            actual.IgnoreRules.Single().Should().BeOfType<ExpressionIgnoreRule<Person>>();
        }

        [Fact]
        public void IgnoringThrowsExceptionWithNullBuildConfigurationTest()
        {
            Action action = () => ((IBuildConfiguration) null).Ignoring<Person>(x => x.Priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringThrowsExceptionWithNullExpressionTest()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Ignoring<Person>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MappingReturnsNewBuildConfigurationWithTypeMappingRuleAppendedTest()
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
        public void MappingThrowsExceptionWithNullBuildConfigurationTest()
        {
            Action action = () => ((IBuildConfiguration) null).Mapping<Stream, MemoryStream>();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullBuildConfigurationTest()
        {
            var model = new Person();

            Action action = () => ((IBuildConfiguration) null).Populate(model);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstanceTest()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Populate<Person>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateUsesDefaultExecuteStrategyToPopulateInstanceTest()
        {
            var value = Guid.NewGuid();
            var expected = new SlimModel();

            var target = Substitute.For<IBuildConfiguration>();
            var creator = Substitute.For<ITypeCreator>();
            var creators = new Collection<ITypeCreator>
            {
                creator
            };
            var generator = Substitute.For<IValueGenerator>();
            var generators = new Collection<IValueGenerator>
            {
                generator
            };

            target.TypeCreators.Returns(creators);
            target.ValueGenerators.Returns(generators);
            creator.CanPopulate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(true);
            creator.Populate(expected, Arg.Any<IExecuteStrategy>()).Returns(expected);
            generator.IsMatch(typeof(Guid), "Value", Arg.Is<IBuildChain>(x => x.Last == expected)).Returns(true);
            generator.Generate(typeof(Guid), "Value", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected))
                .Returns(value);

            var actual = target.Populate(expected);

            actual.Should().Be(expected);
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
            Action action = () => BuildConfigurationExtensions.RemoveCreationRule<DummyCreationRule>(null);

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
            Action action = () => BuildConfigurationExtensions.RemoveExecuteOrderRule<DummyExecuteOrderRule>(null);

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
            Action action = () => BuildConfigurationExtensions.RemoveIgnoreRule<DummyIgnoreRule>(null);

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
            Action action = () => BuildConfigurationExtensions.RemovePostBuildAction<DummyPostBuildAction>(null);

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
            Action action = () => BuildConfigurationExtensions.RemoveTypeCreator<DefaultTypeCreator>(null);

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
            Action action = () => BuildConfigurationExtensions.RemoveTypeMappingRule<DummyTypeMappingRule>(null);

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
            Action action = () => BuildConfigurationExtensions.RemoveValueGenerator<StringValueGenerator>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UsingExecuteStrategyReturnsExecuteStrategyWithBuildConfigurationTest()
        {
            var target = Substitute.For<IBuildConfiguration>();

            var actual = target.UsingExecuteStrategy<DefaultExecuteStrategy<Person>>();

            actual.Configuration.Should().BeSameAs(target);
            actual.Log.Should().BeOfType<DefaultBuildLog>();
        }

        [Fact]
        public void UsingExecuteStrategyThrowsExceptionWithNullConfigurationTest()
        {
            Action action = () => ((IBuildConfiguration) null).UsingExecuteStrategy<NullExecuteStrategy>();

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
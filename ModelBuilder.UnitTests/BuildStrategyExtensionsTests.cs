﻿namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class BuildStrategyExtensionsTests
    {
        [Fact]
        public void CloneReturnsCompilerWithBuildStrategyConfigurationTest()
        {
            var target = new DefaultBuildStrategyCompiler().AddIgnoreRule<Person>(x => x.Address)
                .AddCreationRule<Company>(x => x.Address, 100, "stuff").Compile();

            var actual = target.Clone();

            actual.ConstructorResolver.Should().Be(target.ConstructorResolver);
            actual.PropertyResolver.Should().Be(target.PropertyResolver);
            actual.CreationRules.Should().BeEquivalentTo(target.CreationRules);
            actual.TypeCreators.Should().BeEquivalentTo(target.TypeCreators);
            actual.ValueGenerators.Should().BeEquivalentTo(target.ValueGenerators);
            actual.IgnoreRules.Should().BeEquivalentTo(target.IgnoreRules);
            actual.TypeMappingRules.Should().BeEquivalentTo(target.TypeMappingRules);
            actual.ExecuteOrderRules.Should().BeEquivalentTo(target.ExecuteOrderRules);
        }

        [Fact]
        public void CloneThrowsExceptionWithNullBuildStrategyTest()
        {
            Action action = () => ((IBuildStrategy) null).Clone();

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
            var target = Substitute.For<IBuildStrategy>();

            target.ValueGenerators.Returns(generators);
            generator.IsSupported(typeof(Guid), null, Arg.Any<IBuildChain>()).Returns(true);
            generator.Generate(typeof(Guid), null, Arg.Any<IExecuteStrategy>()).Returns(value);

            var actual = target.Create(typeof(Guid));

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateThrowsExceptionWithNullInstanceTypeTest()
        {
            var target = Substitute.For<IBuildStrategy>();

            Action action = () => target.Create(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullStrategyTest()
        {
            Action action = () => ((IBuildStrategy) null).Create(typeof(Guid));

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
            var target = Substitute.For<IBuildStrategy>();

            target.ValueGenerators.Returns(generators);
            generator.IsSupported(typeof(Guid), null, Arg.Any<IBuildChain>()).Returns(true);
            generator.Generate(typeof(Guid), null, Arg.Any<IExecuteStrategy>()).Returns(value);

            var actual = target.Create<Guid>();

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateTThrowsExceptionWithNullStrategyTest()
        {
            Action action = () => ((IBuildStrategy) null).Create<Guid>();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringReturnsNewBuildStrategyWithIgnoreRuleAppendedTest()
        {
            var target = Model.BuildStrategy;

            var actual = target.Ignoring<Person>(x => x.Priority);

            actual.Should().NotBeSameAs(target);
            actual.IgnoreRules.Should().NotBeEmpty();

            var matchingRule =
                actual.IgnoreRules.FirstOrDefault(x => x.PropertyName == "Priority" && x.TargetType == typeof(Person));

            matchingRule.Should().NotBeNull();
        }

        [Fact]
        public void IgnoringThrowsExceptionWithNullExpressionTest()
        {
            var target = Substitute.For<IBuildStrategy>();

            Action action = () => target.Ignoring<Person>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringThrowsExceptionWithNullStrategyTest()
        {
            Action action = () => ((IBuildStrategy) null).Ignoring<Person>(x => x.Priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MappingReturnsNewBuildStrategyWithTypeMappingRuleAppendedTest()
        {
            var target = Model.BuildStrategy;

            var actual = target.Mapping<Stream, MemoryStream>();

            actual.Should().NotBeSameAs(target);
            actual.TypeMappingRules.Should().NotBeEmpty();

            var matchingRule = actual.TypeMappingRules.FirstOrDefault(
                x => x.SourceType == typeof(Stream) && x.TargetType == typeof(MemoryStream));

            matchingRule.Should().NotBeNull();
        }

        [Fact]
        public void MappingThrowsExceptionWithNullStrategyTest()
        {
            Action action = () => ((IBuildStrategy) null).Mapping<Stream, MemoryStream>();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullBuildStrategyTest()
        {
            var model = new Person();

            Action action = () => ((IBuildStrategy) null).Populate(model);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstanceTest()
        {
            var target = Substitute.For<IBuildStrategy>();

            Action action = () => target.Populate<Person>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateUsesDefaultExecuteStrategyToPopulateInstanceTest()
        {
            var value = Guid.NewGuid();
            var expected = new SlimModel();

            var target = Substitute.For<IBuildStrategy>();
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
            generator.IsSupported(typeof(Guid), "Value", Arg.Is<IBuildChain>(x => x.Last == expected)).Returns(true);
            generator.Generate(typeof(Guid), "Value", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected))
                .Returns(value);

            var actual = target.Populate(expected);

            actual.Should().Be(expected);
        }

        [Fact]
        public void UsingExecuteStrategyReturnsExecuteStrategyWithBuildStrategyConfigurationsTest()
        {
            var strategy = Model.BuildStrategy;
            var ignoreRules = new Collection<IgnoreRule>
            {
                new IgnoreRule(typeof(string), "Stuff")
            };

            var buildLog = Substitute.For<IBuildLog>();
            var target = Substitute.For<IBuildStrategy>();

            target.GetBuildLog().Returns(buildLog);

            target.ConstructorResolver.Returns(strategy.ConstructorResolver);
            target.ExecuteOrderRules.Returns(strategy.ExecuteOrderRules);
            target.TypeCreators.Returns(strategy.TypeCreators);
            target.ValueGenerators.Returns(strategy.ValueGenerators);
            target.IgnoreRules.Returns(ignoreRules);

            var actual = target.UsingExecuteStrategy<DefaultExecuteStrategy<Person>>();

            actual.Configuration.Should().BeSameAs(target);
            actual.Log.Should().BeSameAs(buildLog);
        }

        [Fact]
        public void UsingExecuteStrategyThrowsExceptionWhenStrategyReturnsNullBuildLogTest()
        {
            var target = Substitute.For<IBuildStrategy>();

            target.GetBuildLog().Returns((IBuildLog) null);

            Action action = () => target.UsingExecuteStrategy<DefaultExecuteStrategy>();

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void UsingExecuteStrategyThrowsExceptionWithNullStrategyTest()
        {
            Action action = () => ((IBuildStrategy) null).UsingExecuteStrategy<NullExecuteStrategy>();

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
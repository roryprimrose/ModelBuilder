namespace ModelBuilder.UnitTests
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

    public class BuildConfigurationExtensionsTests
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
            var target = Substitute.For<IBuildConfiguration>();

            target.ValueGenerators.Returns(generators);
            generator.IsSupported(typeof(Guid), null, Arg.Any<IBuildChain>()).Returns(true);
            generator.Generate(typeof(Guid), null, Arg.Any<IExecuteStrategy>()).Returns(value);

            var actual = target.Create(typeof(Guid));

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateThrowsExceptionWithNullInstanceTypeTest()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Create(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullStrategyTest()
        {
            Action action = () => ((IBuildConfiguration) null).Create(typeof(Guid));

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
            generator.IsSupported(typeof(Guid), null, Arg.Any<IBuildChain>()).Returns(true);
            generator.Generate(typeof(Guid), null, Arg.Any<IExecuteStrategy>()).Returns(value);

            var actual = target.Create<Guid>();

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateTThrowsExceptionWithNullStrategyTest()
        {
            Action action = () => ((IBuildConfiguration) null).Create<Guid>();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringReturnsNewBuildConfigurationWithIgnoreRuleAppendedTest()
        {
            var ignoreRules = new Collection<IgnoreRule>();

            var target = Substitute.For<IBuildConfiguration>();

            target.IgnoreRules.Returns(ignoreRules);

            var actual = target.Ignoring<Person>(x => x.Priority);

            actual.IgnoreRules.Should().NotBeEmpty();

            var matchingRule =
                actual.IgnoreRules.FirstOrDefault(x => x.PropertyName == "Priority" && x.TargetType == typeof(Person));

            matchingRule.Should().NotBeNull();
        }

        [Fact]
        public void IgnoringThrowsExceptionWithNullExpressionTest()
        {
            var target = Substitute.For<IBuildConfiguration>();

            Action action = () => target.Ignoring<Person>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringThrowsExceptionWithNullStrategyTest()
        {
            Action action = () => ((IBuildConfiguration) null).Ignoring<Person>(x => x.Priority);

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
        public void MappingThrowsExceptionWithNullStrategyTest()
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
            generator.IsSupported(typeof(Guid), "Value", Arg.Is<IBuildChain>(x => x.Last == expected)).Returns(true);
            generator.Generate(typeof(Guid), "Value", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected))
                .Returns(value);

            var actual = target.Populate(expected);

            actual.Should().Be(expected);
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
namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class BuildStrategyExtensionsTests
    {
        [Fact]
        public void CreateReturnsInstanceCreatedByDefaultExecuteStrategyTest()
        {
            var value = Guid.NewGuid();

            var generator = Substitute.For<IValueGenerator>();
            var generators = new List<IValueGenerator>
            {
                generator
            }.AsReadOnly();
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
            IBuildStrategy target = null;

            Action action = () => target.Create(typeof(Guid));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateTReturnsInstanceCreatedByDefaultExecuteStrategyTest()
        {
            var value = Guid.NewGuid();

            var generator = Substitute.For<IValueGenerator>();
            var generators = new List<IValueGenerator>
            {
                generator
            }.AsReadOnly();
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
            IBuildStrategy target = null;

            Action action = () => target.Create<Guid>();

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
            IBuildStrategy target = null;

            Action action = () => target.Ignoring<Person>(x => x.Priority);

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
            IBuildStrategy target = null;

            Action action = () => target.Mapping<Stream, MemoryStream>();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullBuildStrategyTest()
        {
            var model = new Person();

            IBuildStrategy target = null;

            Action action = () => target.Populate(model);

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
            var creators = new List<ITypeCreator>
            {
                creator
            }.AsReadOnly();
            var generator = Substitute.For<IValueGenerator>();
            var generators = new List<IValueGenerator>
            {
                generator
            }.AsReadOnly();

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
            var ignoreRules = new List<IgnoreRule>
            {
                new IgnoreRule(typeof(string), "Stuff")
            };

            var buildLog = Substitute.For<IBuildLog>();
            var target = Substitute.For<IBuildStrategy>();

            target.GetBuildLog().Returns(buildLog);

            target.ConstructorResolver.Returns(strategy.ConstructorResolver);
            target.ExecuteOrderRules.Returns(
                new ReadOnlyCollection<ExecuteOrderRule>(strategy.ExecuteOrderRules.ToList()));
            target.TypeCreators.Returns(new ReadOnlyCollection<ITypeCreator>(strategy.TypeCreators.ToList()));
            target.ValueGenerators.Returns(new ReadOnlyCollection<IValueGenerator>(strategy.ValueGenerators.ToList()));
            target.IgnoreRules.Returns(new ReadOnlyCollection<IgnoreRule>(ignoreRules));

            var actual = target.UsingExecuteStrategy<DefaultExecuteStrategy<Person>>();

            actual.Configuration.Should().BeSameAs(target);
            actual.Log.Should().BeSameAs(buildLog);
        }

        [Fact]
        public void UsingExecuteStrategyThrowsExceptionWhenStrategyReturnsNullBuildLogTest()
        {
            var target = Substitute.For<IBuildStrategy>();

            target.GetBuildLog().Returns((IBuildLog)null);

            Action action = () => target.UsingExecuteStrategy<DefaultExecuteStrategy>();

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void UsingExecuteStrategyThrowsExceptionWithNullStrategyTest()
        {
            IBuildStrategy target = null;

            Action action = () => target.UsingExecuteStrategy<NullExecuteStrategy>();

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
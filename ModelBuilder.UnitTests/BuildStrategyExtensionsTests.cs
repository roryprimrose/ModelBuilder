using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class BuildStrategyExtensionsTests
    {
        [Fact]
        public void CloneReturnsCompilerWithBuildStrategyConfigurationTest()
        {
            var target = new DefaultBuildStrategy().Clone().Compile();

            var actual = target.Clone();

            actual.BuildLog.Should().Be(target.BuildLog);
            actual.ConstructorResolver.Should().Be(target.ConstructorResolver);
            actual.TypeCreators.ShouldBeEquivalentTo(target.TypeCreators);
            actual.ValueGenerators.ShouldBeEquivalentTo(target.ValueGenerators);
            actual.IgnoreRules.ShouldBeEquivalentTo(target.IgnoreRules);
            actual.ExecuteOrderRules.ShouldBeEquivalentTo(target.ExecuteOrderRules);
        }

        [Fact]
        public void CloneThrowsExceptionWithNullBuildStrategyTest()
        {
            IBuildStrategy target = null;

            Action action = () => target.Clone();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreateReturnsInstanceCreatedByDefaultExecuteStrategyTest()
        {
            var value = Guid.NewGuid();

            var buildLog = Substitute.For<IBuildLog>();
            var generator = Substitute.For<IValueGenerator>();
            var generators = new List<IValueGenerator> {generator}.AsReadOnly();
            var target = Substitute.For<IBuildStrategy>();

            target.ValueGenerators.Returns(generators);
            target.BuildLog.Returns(buildLog);
            generator.IsSupported(typeof(Guid), null, Arg.Any<LinkedList<object>>()).Returns(true);
            generator.Generate(typeof(Guid), null, Arg.Any<LinkedList<object>>()).Returns(value);

            var actual = target.Create<Guid>();

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateThrowsExceptionWithNullStrategyTest()
        {
            IBuildStrategy target = null;

            Action action = () => target.Create<Guid>();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreateWithReturnsInstanceCreatedByDefaultExecuteStrategyTest()
        {
            var value = Guid.NewGuid();

            var generator = Substitute.For<IValueGenerator>();
            var generators = new List<IValueGenerator> {generator}.AsReadOnly();
            var target = Substitute.For<IBuildStrategy>();

            target.ValueGenerators.Returns(generators);
            generator.IsSupported(typeof(Guid), null, Arg.Any<LinkedList<object>>()).Returns(true);
            generator.Generate(typeof(Guid), null, Arg.Any<LinkedList<object>>()).Returns(value);

            var actual = target.CreateWith<Guid>(null);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateWithThrowsExceptionWithNullStrategyTest()
        {
            IBuildStrategy target = null;

            Action action = () => target.CreateWith<Guid>();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringReturnsNewBuildStrategyWithIgnoreRuleAppendedTest()
        {
            var target = new DefaultBuildStrategy();

            var actual = target.Ignoring<Person>(x => x.Priority);

            actual.Should().NotBeSameAs(target);
            actual.IgnoreRules.Should().NotBeEmpty();
            actual.IgnoreRules.First().PropertyName.Should().Be("Priority");
            actual.IgnoreRules.First().TargetType.Should().Be<Person>();
        }

        [Fact]
        public void IgnoringThrowsExceptionWithNullExpressionTest()
        {
            var target = Substitute.For<IBuildStrategy>();

            Action action = () => target.Ignoring<Person>(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringThrowsExceptionWithNullStrategyTest()
        {
            IBuildStrategy target = null;

            Action action = () => target.Ignoring<Person>(x => x.Priority);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullBuildStrategyTest()
        {
            var model = new Person();

            IBuildStrategy target = null;

            Action action = () => target.Populate(model);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstanceTest()
        {
            var target = Substitute.For<IBuildStrategy>();

            Action action = () => target.Populate<Person>(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PopulateUsesDefaultExecuteStrategyToPopulateInstanceTest()
        {
            var value = Guid.NewGuid();
            var expected = new SlimModel();

            var target = Substitute.For<IBuildStrategy>();
            var generator = Substitute.For<IValueGenerator>();
            var generators = new List<IValueGenerator> {generator}.AsReadOnly();

            target.ValueGenerators.Returns(generators);
            generator.IsSupported(typeof(Guid), "Value", Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            generator.Generate(typeof(Guid), "Value", Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(value);

            var actual = target.Populate(expected);

            actual.Should().Be(expected);
        }

        [Fact]
        public void WithReturnsExecuteStrategyWithBuildStrategyConfigurationsTest()
        {
            var ignoreRules = new List<IgnoreRule>
            {
                new IgnoreRule(typeof(string), "Stuff")
            };

            var target = Substitute.For<IBuildStrategy>();

            target.BuildLog.Returns(DefaultBuildStrategy.DefaultBuildLog);
            target.ConstructorResolver.Returns(DefaultBuildStrategy.DefaultConstructorResolver);
            target.ExecuteOrderRules.Returns(
                new ReadOnlyCollection<ExecuteOrderRule>(DefaultBuildStrategy.DefaultExecuteOrderRules.ToList()));
            target.TypeCreators.Returns(
                new ReadOnlyCollection<ITypeCreator>(DefaultBuildStrategy.DefaultTypeCreators.ToList()));
            target.ValueGenerators.Returns(
                new ReadOnlyCollection<IValueGenerator>(DefaultBuildStrategy.DefaultValueGenerators.ToList()));
            target.IgnoreRules.Returns(new ReadOnlyCollection<IgnoreRule>(ignoreRules));

            var actual = target.With<DefaultExecuteStrategy<Person>>();

            actual.BuildStrategy.Should().BeSameAs(target);
        }

        [Fact]
        public void WithThrowsExceptionWithNullStrategyTest()
        {
            IBuildStrategy target = null;

            Action action = () => target.With<NullExecuteStrategy>();

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
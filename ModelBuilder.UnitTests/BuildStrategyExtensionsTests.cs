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
        public void CreateReturnsInstanceCreatedByDefaultExecuteStrategyTest()
        {
            var value = Guid.NewGuid();

            var generator = Substitute.For<IValueGenerator>();
            var generators = new List<IValueGenerator> {generator}.AsReadOnly();
            var target = Substitute.For<IBuildStrategy>();

            target.ValueGenerators.Returns(generators);
            generator.IsSupported(typeof(Guid), null, null).Returns(true);
            generator.Generate(typeof(Guid), null, null).Returns(value);

            target.Create<Guid>().Returns(value);

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
            generator.IsSupported(typeof(Guid), null, null).Returns(true);
            generator.Generate(typeof(Guid), null, null).Returns(value);

            target.CreateWith<Guid>(null).Returns(value);

            var actual = target.Create<Guid>();

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
        public void IgnoringReturnsNewExecuteStrategyWithIgnoreRuleAppendedTest()
        {
            var ignoreRules = new Collection<IgnoreRule>();

            var executeStrategy = Substitute.For<IExecuteStrategy<Person>>();
            var target = Substitute.For<IBuildStrategy>();

            target.GetExecuteStrategy<Person>().Returns(executeStrategy);
            executeStrategy.IgnoreRules.Returns(ignoreRules);

            var actual = target.Ignoring<Person>(x => x.Priority);

            actual.Should().BeSameAs(executeStrategy);
            ignoreRules.Should().NotBeEmpty();
            ignoreRules.First().PropertyName.Should().Be("Priority");
            ignoreRules.First().TargetType.Should().Be<Person>();
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
            generator.IsSupported(typeof(Guid), "Value", expected).Returns(true);
            generator.Generate(typeof(Guid), "Value", expected).Returns(value);

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

            target.ConstructorResolver.Returns(DefaultBuildStrategy.DefaultConstructorResolver);
            target.ExecuteOrderRules.Returns(new ReadOnlyCollection<ExecuteOrderRule>(DefaultBuildStrategy.DefaultExecuteOrderRules.ToList()));
            target.TypeCreators.Returns(new ReadOnlyCollection<ITypeCreator>(DefaultBuildStrategy.DefaultTypeCreators.ToList()));
            target.ValueGenerators.Returns(new ReadOnlyCollection<IValueGenerator>(DefaultBuildStrategy.DefaultValueGenerators.ToList()));
            target.IgnoreRules.Returns(new ReadOnlyCollection<IgnoreRule>(ignoreRules));

            var actual = target.With<DefaultExecuteStrategy<Person>>();

            actual.ConstructorResolver.Should().BeSameAs(target.ConstructorResolver);
            actual.ExecuteOrderRules.ShouldAllBeEquivalentTo(target.ExecuteOrderRules);
            actual.IgnoreRules.ShouldAllBeEquivalentTo(target.IgnoreRules);
            actual.TypeCreators.ShouldAllBeEquivalentTo(target.TypeCreators);
            actual.ValueGenerators.ShouldAllBeEquivalentTo(target.ValueGenerators);
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
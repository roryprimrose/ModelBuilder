using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class ModelTests
    {
        [Fact]
        public void BuildStrategyThrowsExceptionWithNullStrategyTest()
        {
            Action action = () => Model.BuildStrategy = null;

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CanAssignBuildStrategyTest()
        {
            var strategy = Substitute.For<IBuildStrategy>();

            var existingStrategy = Model.BuildStrategy;

            try
            {
                Model.BuildStrategy = strategy;

                var actual = Model.BuildStrategy;

                actual.Should().BeSameAs(strategy);
            }
            finally
            {
                Model.BuildStrategy = existingStrategy;
            }
        }

        [Fact]
        public void CanAssignNewBuildStrategyTest()
        {
            var buildStrategy = Substitute.For<IBuildStrategy>();

            try
            {
                Model.BuildStrategy = buildStrategy;

                var actual = Model.BuildStrategy;

                actual.Should().BeSameAs(buildStrategy);
            }
            finally
            {
                Model.BuildStrategy = Model.DefaultBuildStrategy;
            }
        }

        [Fact]
        public void CreateUsesBuildStrategyToCreateInstanceTest()
        {
            var value = Guid.NewGuid();

            var build = Substitute.For<IBuildStrategy>();
            var generator = Substitute.For<IValueGenerator>();
            var generators = new List<IValueGenerator> {generator}.AsReadOnly();

            build.ValueGenerators.Returns(generators);
            generator.IsSupported(typeof(Guid), null, null).Returns(true);
            generator.Generate(typeof(Guid), null, null).Returns(value);

            try
            {
                Model.BuildStrategy = build;

                var actual = Model.Create<Guid>();

                actual.Should().Be(value);
            }
            finally
            {
                Model.BuildStrategy = Model.DefaultBuildStrategy;
            }
        }

        [Fact]
        public void CreateWithUsesBuildStrategyToCreateInstanceWithParametersTest()
        {
            var value = Guid.NewGuid();
            var expected = new ReadOnlyModel(value);

            var build = Substitute.For<IBuildStrategy>();
            var creator = Substitute.For<ITypeCreator>();
            var creators = new List<ITypeCreator> {creator}.AsReadOnly();

            build.TypeCreators.Returns(creators);
            creator.IsSupported(typeof(ReadOnlyModel), null, null).Returns(true);
            creator.Create(typeof(ReadOnlyModel), null, null, value).Returns(expected);
            creator.Populate(expected, Arg.Any<IExecuteStrategy>()).Returns(expected);

            try
            {
                Model.BuildStrategy = build;

                var actual = Model.CreateWith<ReadOnlyModel>(value);

                actual.Value.Should().Be(value);
            }
            finally
            {
                Model.BuildStrategy = Model.DefaultBuildStrategy;
            }
        }

        [Fact]
        public void DefaultBuildStrategyReturnsSameInstanceTest()
        {
            var firstActual = Model.DefaultBuildStrategy;
            var secondActual = Model.DefaultBuildStrategy;

            firstActual.Should().BeSameAs(secondActual);
        }

        [Fact]
        public void ForReturnsDefaultExecuteStrategyWithBuildStrategyConfigurationTest()
        {
            var build = Substitute.For<IBuildStrategy>();
            var generator = Substitute.For<IValueGenerator>();
            var generators = new List<IValueGenerator> {generator}.AsReadOnly();
            var creator = Substitute.For<ITypeCreator>();
            var creators = new List<ITypeCreator> {creator}.AsReadOnly();
            var ignoreRules = new List<IgnoreRule>().AsReadOnly();
            var resolver = Substitute.For<IConstructorResolver>();

            build.ValueGenerators.Returns(generators);
            build.TypeCreators.Returns(creators);
            build.IgnoreRules.Returns(ignoreRules);
            build.ConstructorResolver.Returns(resolver);

            try
            {
                Model.BuildStrategy = build;

                var actual = Model.For<ReadOnlyModel>();

                actual.ConstructorResolver.Should().Be(build.ConstructorResolver);
                actual.IgnoreRules.ShouldAllBeEquivalentTo(build.IgnoreRules);
                actual.TypeCreators.ShouldAllBeEquivalentTo(build.TypeCreators);
                actual.ValueGenerators.ShouldAllBeEquivalentTo(build.ValueGenerators);
            }
            finally
            {
                Model.BuildStrategy = Model.DefaultBuildStrategy;
            }
        }

        [Fact]
        public void IgnoringThrowsExceptionWithNullExpressionTest()
        {
            Action action = () => Model.Ignoring<Person>(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringUsesBuildStrategyToCreateInstanceTest()
        {
            var actual = Model.Ignoring<Person>(x => x.FirstName).Create();

            actual.FirstName.Should().BeNull();
        }

        [Fact]
        public void PopulateUsesBuildStrategyToPopulateInstanceTest()
        {
            var value = Guid.NewGuid();
            var expected = new SlimModel();

            var build = Substitute.For<IBuildStrategy>();
            var generator = Substitute.For<IValueGenerator>();
            var generators = new List<IValueGenerator> {generator}.AsReadOnly();

            build.ValueGenerators.Returns(generators);
            generator.IsSupported(typeof(Guid), "Value", expected).Returns(true);
            generator.Generate(typeof(Guid), "Value", expected).Returns(value);

            try
            {
                Model.BuildStrategy = build;

                var actual = Model.Populate(expected);

                actual.Should().Be(expected);
            }
            finally
            {
                Model.BuildStrategy = Model.DefaultBuildStrategy;
            }
        }

        [Fact]
        public void UsingReturnsNewBuilderStrategyTest()
        {
            var actual = Model.Using<NullBuildStrategy>();

            actual.Should().BeOfType<NullBuildStrategy>();
        }

        [Fact]
        public void UsingReturnsSpecifiedBuildStrategyTest()
        {
            var actual = Model.Using<DummyBuildStrategy>();

            actual.Should().BeOfType(typeof(DummyBuildStrategy));
        }

        [Fact]
        public void WithReturnsNewExecuteStrategyUsingBuilderStrategyTest()
        {
            var buildStrategy = Substitute.For<IBuildStrategy>();

            try
            {
                Model.BuildStrategy = buildStrategy;

                var actual = Model.With<NullExecuteStrategy>();

                actual.Should().BeOfType<NullExecuteStrategy>();
            }
            finally
            {
                Model.BuildStrategy = Model.DefaultBuildStrategy;
            }
        }

        [Fact]
        public void WithReturnsSpecifiedExecuteStrategyTest()
        {
            var actual = Model.With<DummyExecuteStrategy>();

            actual.Should().BeOfType(typeof(DummyExecuteStrategy));
        }
    }
}
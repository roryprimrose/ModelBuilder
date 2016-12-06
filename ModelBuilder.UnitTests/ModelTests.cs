namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

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
        public void CreateThrowsExceptionWithNullInstanceTypeTest()
        {
            Action action = () => Model.Create(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreateTUsesBuildStrategyToCreateInstanceTest()
        {
            var value = Guid.NewGuid();

            var build = Substitute.For<IBuildStrategy>();
            var generator = Substitute.For<IValueGenerator>();
            var generators = new List<IValueGenerator> {generator}.AsReadOnly();

            build.ValueGenerators.Returns(generators);
            generator.IsSupported(typeof(Guid), null, Arg.Any<LinkedList<object>>()).Returns(true);
            generator.Generate(typeof(Guid), null, Arg.Any<LinkedList<object>>()).Returns(value);

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
        public void CreateUsesBuildStrategyToCreateInstanceTest()
        {
            var value = Guid.NewGuid();

            var build = Substitute.For<IBuildStrategy>();
            var generator = Substitute.For<IValueGenerator>();
            var generators = new List<IValueGenerator> {generator}.AsReadOnly();

            build.ValueGenerators.Returns(generators);
            generator.IsSupported(typeof(Guid), null, Arg.Any<LinkedList<object>>()).Returns(true);
            generator.Generate(typeof(Guid), null, Arg.Any<LinkedList<object>>()).Returns(value);

            try
            {
                Model.BuildStrategy = build;

                var actual = Model.Create(typeof(Guid));

                actual.Should().Be(value);
            }
            finally
            {
                Model.BuildStrategy = Model.DefaultBuildStrategy;
            }
        }

        [Fact]
        public void CreateWithThrowsExceptionWithNullInstanceTypeTest()
        {
            Action action = () => Model.CreateWith(null);

            action.ShouldThrow<ArgumentNullException>();
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
            creator.CanCreate(typeof(ReadOnlyModel), null, Arg.Any<LinkedList<object>>()).Returns(true);
            creator.Create(typeof(ReadOnlyModel), null, Arg.Any<IExecuteStrategy>(), value).Returns(expected);
            creator.Populate(expected, Arg.Any<IExecuteStrategy>()).Returns(expected);

            try
            {
                Model.BuildStrategy = build;

                var actual = (ReadOnlyModel) Model.CreateWith(typeof(ReadOnlyModel), value);

                actual.Value.Should().Be(value);
            }
            finally
            {
                Model.BuildStrategy = Model.DefaultBuildStrategy;
            }
        }

        [Fact]
        public void CreateWitThUsesBuildStrategyToCreateInstanceWithParametersTest()
        {
            var value = Guid.NewGuid();
            var expected = new ReadOnlyModel(value);

            var build = Substitute.For<IBuildStrategy>();
            var creator = Substitute.For<ITypeCreator>();
            var creators = new List<ITypeCreator> {creator}.AsReadOnly();

            build.TypeCreators.Returns(creators);
            creator.CanCreate(typeof(ReadOnlyModel), null, Arg.Any<LinkedList<object>>()).Returns(true);
            creator.CanPopulate(typeof(ReadOnlyModel), null, Arg.Any<LinkedList<object>>()).Returns(true);
            creator.Create(typeof(ReadOnlyModel), null, Arg.Any<IExecuteStrategy>(), value).Returns(expected);
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
        public void ForReturnsDefaultExecuteStrategyWithDefaultBuildStrategyConfigurationTest()
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

                actual.Configuration.Should().Be(build);
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
            var actual = Model.Ignoring<Person>(x => x.FirstName).Create<Person>();

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
            generator.IsSupported(typeof(Guid), "Value", Arg.Any<LinkedList<object>>()).Returns(true);
            generator.Generate(typeof(Guid), "Value", Arg.Any<LinkedList<object>>()).Returns(value);

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
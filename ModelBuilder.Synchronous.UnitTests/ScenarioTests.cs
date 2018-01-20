namespace ModelBuilder.Synchronous.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.UnitTests;
    using NSubstitute;
    using Xunit;

    public class ScenarioTests
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
        public void CreateTUsesBuildStrategyToCreateInstanceTest()
        {
            var value = Guid.NewGuid();

            var build = Substitute.For<IBuildStrategy>();
            var generator = Substitute.For<IValueGenerator>();
            var generators = new List<IValueGenerator>
            {
                generator
            }.AsReadOnly();

            build.ValueGenerators.Returns(generators);
            generator.IsSupported(typeof(Guid), null, Arg.Any<LinkedList<object>>()).Returns(true);
            generator.Generate(typeof(Guid), null, Arg.Any<IExecuteStrategy>()).Returns(value);

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
            var generators = new List<IValueGenerator>
            {
                generator
            }.AsReadOnly();

            build.ValueGenerators.Returns(generators);
            generator.IsSupported(typeof(Guid), null, Arg.Any<LinkedList<object>>()).Returns(true);
            generator.Generate(typeof(Guid), null, Arg.Any<IExecuteStrategy>()).Returns(value);

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
        public void CreateWithUsesBuildStrategyToCreateInstanceWithParametersTest()
        {
            var value = Guid.NewGuid();
            var expected = new ReadOnlyModel(value);

            var build = Substitute.For<IBuildStrategy>();
            var creator = Substitute.For<ITypeCreator>();
            var creators = new List<ITypeCreator>
            {
                creator
            }.AsReadOnly();

            build.TypeCreators.Returns(creators);
            creator.CanCreate(typeof(ReadOnlyModel), null, Arg.Any<LinkedList<object>>()).Returns(true);
            creator.Create(typeof(ReadOnlyModel), null, Arg.Any<IExecuteStrategy>(), value).Returns(expected);
            creator.Populate(expected, Arg.Any<IExecuteStrategy>()).Returns(expected);

            try
            {
                Model.BuildStrategy = build;

                var actual = (ReadOnlyModel)Model.CreateWith(typeof(ReadOnlyModel), value);

                actual.Value.Should().Be(value);
            }
            finally
            {
                Model.BuildStrategy = Model.DefaultBuildStrategy;
            }
        }

        [Fact]
        public void CreateWithTUsesBuildStrategyToCreateInstanceWithParametersTest()
        {
            var value = Guid.NewGuid();
            var expected = new ReadOnlyModel(value);

            var build = Substitute.For<IBuildStrategy>();
            var creator = Substitute.For<ITypeCreator>();
            var creators = new List<ITypeCreator>
            {
                creator
            }.AsReadOnly();

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
        public void ForReturnsDefaultExecuteStrategyWithDefaultBuildStrategyConfigurationTest()
        {
            var build = Substitute.For<IBuildStrategy>();
            var generator = Substitute.For<IValueGenerator>();
            var generators = new List<IValueGenerator>
            {
                generator
            }.AsReadOnly();
            var creator = Substitute.For<ITypeCreator>();
            var creators = new List<ITypeCreator>
            {
                creator
            }.AsReadOnly();
            var ignoreRules = new List<IgnoreRule>().AsReadOnly();
            var resolver = Substitute.For<IConstructorResolver>();

            build.ValueGenerators.Returns(generators);
            build.TypeCreators.Returns(creators);
            build.IgnoreRules.Returns(ignoreRules);
            build.ConstructorResolver.Returns(resolver);

            try
            {
                Model.BuildStrategy = build;

                var actual = Model.UsingExecuteStrategy<DefaultExecuteStrategy<ReadOnlyModel>>();

                actual.Configuration.Should().Be(build);
            }
            finally
            {
                Model.BuildStrategy = Model.DefaultBuildStrategy;
            }
        }

        [Fact]
        public void PopulateUsesBuildStrategyToPopulateInstanceTest()
        {
            var value = Guid.NewGuid();
            var expected = new SlimModel();

            var build = Substitute.For<IBuildStrategy>();
            var creator = Substitute.For<ITypeCreator>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            var creators = new List<ITypeCreator>
            {
                creator
            }.AsReadOnly();
            var generator = Substitute.For<IValueGenerator>();
            var generators = new List<IValueGenerator>
            {
                generator
            }.AsReadOnly();

            build.PropertyResolver.Returns(propertyResolver);
            build.TypeCreators.Returns(creators);
            build.ValueGenerators.Returns(generators);
            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            creator.CanPopulate(typeof(SlimModel), null, Arg.Any<LinkedList<object>>()).Returns(true);
            creator.Populate(expected, Arg.Any<IExecuteStrategy>()).Returns(expected);
            creator.AutoPopulate.Returns(true);
            generator.IsSupported(typeof(Guid), "Value", Arg.Any<LinkedList<object>>()).Returns(true);
            generator.Generate(typeof(Guid), "Value", Arg.Any<IExecuteStrategy>()).Returns(value);

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
        public void WithReturnsNewExecuteStrategyUsingBuilderStrategyTest()
        {
            var buildStrategy = Substitute.For<IBuildStrategy>();

            try
            {
                Model.BuildStrategy = buildStrategy;

                var actual = Model.UsingExecuteStrategy<NullExecuteStrategy>();

                actual.Should().BeOfType<NullExecuteStrategy>();
            }
            finally
            {
                Model.BuildStrategy = Model.DefaultBuildStrategy;
            }
        }
    }
}
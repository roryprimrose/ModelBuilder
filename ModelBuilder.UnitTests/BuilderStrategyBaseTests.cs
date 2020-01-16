namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class BuilderStrategyBaseTests
    {
        [Fact]
        public void ReturnsValuesFromConstructorTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var propertyResolver = Model.BuildStrategy.PropertyResolver;
            var creationRules = Model.BuildStrategy.CreationRules.ToList();
            var typeCreators = Model.BuildStrategy.TypeCreators.ToList();
            var valueGenerators = Model.BuildStrategy.ValueGenerators.ToList();
            var ignoreRules = new List<IgnoreRule>
            {
                new IgnoreRule(typeof(Person), "FirstName")
            };
            var typeMappingRules = Model.BuildStrategy.TypeMappingRules;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules.ToList();
            var postBuildActions = new List<IPostBuildAction>
            {
                Substitute.For<IPostBuildAction>()
            };

            var actual = new BuilderStrategyWrapper(
                constructorResolver,
                propertyResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                typeMappingRules,
                executeOrderRules,
                postBuildActions);

            actual.ConstructorResolver.Should().Be(constructorResolver);
            actual.PropertyResolver.Should().Be(propertyResolver);
            actual.ExecuteOrderRules.Should().BeEquivalentTo(executeOrderRules);
            actual.PostBuildActions.Should().BeEquivalentTo(postBuildActions);
            actual.CreationRules.Should().BeEquivalentTo(creationRules);
            actual.IgnoreRules.Should().BeEquivalentTo(ignoreRules);
            actual.TypeCreators.Should().BeEquivalentTo(typeCreators);
            actual.ValueGenerators.Should().BeEquivalentTo(valueGenerators);
        }

        [Fact]
        public void ReturnsValuesFromCopyConstructorTest()
        {
            var expected = new DefaultBuildStrategyCompiler().Compile();

            var actual = new BuilderStrategyWrapper(expected);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullConstructorResolverTest()
        {
            var propertyResolver = Model.BuildStrategy.PropertyResolver;
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var creationRules = Model.BuildStrategy.CreationRules;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var typeMappingRules = Model.BuildStrategy.TypeMappingRules;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuilderStrategyWrapper(
                null,
                propertyResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                typeMappingRules,
                executeOrderRules,
                postBuildActions);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullCreationRulesTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var propertyResolver = Model.BuildStrategy.PropertyResolver;
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var typeMappingRules = Model.BuildStrategy.TypeMappingRules;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuilderStrategyWrapper(
                constructorResolver,
                propertyResolver,
                null,
                typeCreators,
                valueGenerators,
                ignoreRules,
                typeMappingRules,
                executeOrderRules,
                postBuildActions);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullExecuteOrderRulesTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var propertyResolver = Model.BuildStrategy.PropertyResolver;
            var creationRules = Model.BuildStrategy.CreationRules;
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var typeMappingRules = Model.BuildStrategy.TypeMappingRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuilderStrategyWrapper(
                constructorResolver,
                propertyResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                typeMappingRules,
                null,
                postBuildActions);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullIgnoreRulesTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var propertyResolver = Model.BuildStrategy.PropertyResolver;
            var creationRules = Model.BuildStrategy.CreationRules;
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var typeMappingRules = Model.BuildStrategy.TypeMappingRules;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuilderStrategyWrapper(
                constructorResolver,
                propertyResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                null,
                typeMappingRules,
                executeOrderRules,
                postBuildActions);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullPostBuildActionsTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var propertyResolver = Model.BuildStrategy.PropertyResolver;
            var creationRules = Model.BuildStrategy.CreationRules;
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var typeMappingRules = Model.BuildStrategy.TypeMappingRules;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuilderStrategyWrapper(
                constructorResolver,
                propertyResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                typeMappingRules,
                executeOrderRules,
                null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullPropertyResolverTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var creationRules = Model.BuildStrategy.CreationRules;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var typeMappingRules = Model.BuildStrategy.TypeMappingRules;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuilderStrategyWrapper(
                constructorResolver,
                null,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                typeMappingRules,
                executeOrderRules,
                postBuildActions);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullStrategyTest()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuilderStrategyWrapper(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullTypeCreatorsTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var propertyResolver = Model.BuildStrategy.PropertyResolver;
            var creationRules = Model.BuildStrategy.CreationRules;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var typeMappingRules = Model.BuildStrategy.TypeMappingRules;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuilderStrategyWrapper(
                constructorResolver,
                propertyResolver,
                creationRules,
                null,
                valueGenerators,
                ignoreRules,
                typeMappingRules,
                executeOrderRules,
                postBuildActions);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullTypeMappingRulesTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var propertyResolver = Model.BuildStrategy.PropertyResolver;
            var creationRules = Model.BuildStrategy.CreationRules;
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuilderStrategyWrapper(
                constructorResolver,
                propertyResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                null,
                executeOrderRules,
                postBuildActions);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullValueGeneratorsTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var propertyResolver = Model.BuildStrategy.PropertyResolver;
            var creationRules = Model.BuildStrategy.CreationRules;
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var typeMappingRules = Model.BuildStrategy.TypeMappingRules;

            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuilderStrategyWrapper(
                constructorResolver,
                propertyResolver,
                creationRules,
                typeCreators,
                null,
                ignoreRules,
                typeMappingRules,
                executeOrderRules,
                postBuildActions);

            action.Should().Throw<ArgumentNullException>();
        }

        private class BuilderStrategyWrapper : BuildStrategyBase
        {
            public BuilderStrategyWrapper(IBuildStrategy strategy) : base(strategy)
            {
            }

            public BuilderStrategyWrapper(
                IConstructorResolver constructorResolver,
                IPropertyResolver propertyResolver,
                IEnumerable<CreationRule> creationRules,
                IEnumerable<ITypeCreator> typeCreators,
                IEnumerable<IValueGenerator> valueGenerators,
                IEnumerable<IgnoreRule> ignoreRules,
                IEnumerable<TypeMappingRule> typeMappingRules,
                IEnumerable<ExecuteOrderRule> executeOrderRules,
                IEnumerable<IPostBuildAction> postBuildActions) : base(
                constructorResolver,
                propertyResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                typeMappingRules,
                executeOrderRules,
                postBuildActions)
            {
            }

            public override IBuildLog GetBuildLog()
            {
                throw new NotImplementedException();
            }

            public override IExecuteStrategy<T> GetExecuteStrategy<T>()
            {
                throw new NotImplementedException();
            }
        }
    }
}
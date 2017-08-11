﻿namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
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
                executeOrderRules,
                postBuildActions);

            actual.ConstructorResolver.Should().Be(constructorResolver);
            actual.PropertyResolver.Should().Be(propertyResolver);
            actual.ExecuteOrderRules.ShouldAllBeEquivalentTo(executeOrderRules);
            actual.PostBuildActions.ShouldAllBeEquivalentTo(postBuildActions);
            actual.CreationRules.ShouldAllBeEquivalentTo(creationRules);
            actual.IgnoreRules.ShouldAllBeEquivalentTo(ignoreRules);
            actual.TypeCreators.ShouldAllBeEquivalentTo(typeCreators);
            actual.ValueGenerators.ShouldAllBeEquivalentTo(valueGenerators);
        }

        [Fact]
        public void ReturnsValuesFromCopyConstructorTest()
        {
            var expected = new DefaultBuildStrategyCompiler().Compile();

            var actual = new BuilderStrategyWrapper(expected);

            actual.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullConstructorResolverTest()
        {
            var propertyResolver = Model.BuildStrategy.PropertyResolver;
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var creationRules = Model.BuildStrategy.CreationRules;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            Action action = () => new BuilderStrategyWrapper(
                null,
                propertyResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                executeOrderRules,
                postBuildActions);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullCreationRulesTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var propertyResolver = Model.BuildStrategy.PropertyResolver;
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            Action action = () => new BuilderStrategyWrapper(
                constructorResolver,
                propertyResolver,
                null,
                typeCreators,
                valueGenerators,
                ignoreRules,
                executeOrderRules,
                postBuildActions);

            action.ShouldThrow<ArgumentNullException>();
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
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            Action action = () => new BuilderStrategyWrapper(
                constructorResolver,
                propertyResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                null,
                postBuildActions);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullIgnoreRulesTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var propertyResolver = Model.BuildStrategy.PropertyResolver;
            var creationRules = Model.BuildStrategy.CreationRules;
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            Action action = () => new BuilderStrategyWrapper(
                constructorResolver,
                propertyResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                null,
                executeOrderRules,
                postBuildActions);

            action.ShouldThrow<ArgumentNullException>();
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
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;

            Action action = () => new BuilderStrategyWrapper(
                constructorResolver,
                propertyResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                executeOrderRules,
                null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullPropertyResolverTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var creationRules = Model.BuildStrategy.CreationRules;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            Action action = () => new BuilderStrategyWrapper(
                constructorResolver,
                null,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                executeOrderRules,
                postBuildActions);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullStrategyTest()
        {
            Action action = () => new BuilderStrategyWrapper(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullTypeCreatorsTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var propertyResolver = Model.BuildStrategy.PropertyResolver;
            var creationRules = Model.BuildStrategy.CreationRules;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            Action action = () => new BuilderStrategyWrapper(
                constructorResolver,
                propertyResolver,
                creationRules,
                null,
                valueGenerators,
                ignoreRules,
                executeOrderRules,
                postBuildActions);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullValueGeneratorsTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var propertyResolver = Model.BuildStrategy.PropertyResolver;
            var creationRules = Model.BuildStrategy.CreationRules;
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            Action action = () => new BuilderStrategyWrapper(
                constructorResolver,
                propertyResolver,
                creationRules,
                typeCreators,
                null,
                ignoreRules,
                executeOrderRules,
                postBuildActions);

            action.ShouldThrow<ArgumentNullException>();
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
                IEnumerable<ExecuteOrderRule> executeOrderRules,
                IEnumerable<IPostBuildAction> postBuildActions) : base(
                constructorResolver,
                propertyResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
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
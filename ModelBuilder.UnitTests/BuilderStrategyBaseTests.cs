namespace ModelBuilder.UnitTests
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
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                executeOrderRules, postBuildActions);
            
            actual.ConstructorResolver.Should().Be(constructorResolver);
            actual.ExecuteOrderRules.ShouldAllBeEquivalentTo(executeOrderRules);
            actual.PostBuildActions.ShouldAllBeEquivalentTo(postBuildActions);
            actual.CreationRules.ShouldAllBeEquivalentTo(creationRules);
            actual.IgnoreRules.ShouldAllBeEquivalentTo(ignoreRules);
            actual.TypeCreators.ShouldAllBeEquivalentTo(typeCreators);
            actual.ValueGenerators.ShouldAllBeEquivalentTo(valueGenerators);
        }
        
        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullCreationRulesTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            Action action =
                () =>
                    new BuilderStrategyWrapper(
                        constructorResolver,
                        null,
                        typeCreators,
                        valueGenerators,
                        ignoreRules,
                        executeOrderRules, postBuildActions);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullExecuteOrderRulesTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var creationRules = Model.BuildStrategy.CreationRules;
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            Action action =
                () =>
                    new BuilderStrategyWrapper(
                        constructorResolver,
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
            var creationRules = Model.BuildStrategy.CreationRules;
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            Action action =
                () =>
                    new BuilderStrategyWrapper(
                        constructorResolver,
                        creationRules,
                        typeCreators,
                        valueGenerators,
                        null,
                        executeOrderRules, postBuildActions);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullPostBuildActionsTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var creationRules = Model.BuildStrategy.CreationRules;
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;

            Action action =
                () =>
                    new BuilderStrategyWrapper(
                        constructorResolver,
                        creationRules,
                        typeCreators,
                        valueGenerators,
                        ignoreRules,
                        executeOrderRules,
                        null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullResolverTest()
        {
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var creationRules = Model.BuildStrategy.CreationRules;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            Action action =
                () =>
                    new BuilderStrategyWrapper(
                        null,
                        creationRules,
                        typeCreators,
                        valueGenerators,
                        ignoreRules,
                        executeOrderRules, postBuildActions);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullTypeCreatorsTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var creationRules = Model.BuildStrategy.CreationRules;
            var valueGenerators = Model.BuildStrategy.ValueGenerators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            Action action =
                () =>
                    new BuilderStrategyWrapper(
                        constructorResolver,
                        creationRules,
                        null,
                        valueGenerators,
                        ignoreRules,
                        executeOrderRules, postBuildActions);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullValueGeneratorsTest()
        {
            var constructorResolver = Model.BuildStrategy.ConstructorResolver;
            var creationRules = Model.BuildStrategy.CreationRules;
            var typeCreators = Model.BuildStrategy.TypeCreators;
            var ignoreRules = Model.BuildStrategy.IgnoreRules;
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;
            var postBuildActions = Model.BuildStrategy.PostBuildActions;

            Action action =
                () =>
                    new BuilderStrategyWrapper(
                        constructorResolver,
                        creationRules,
                        typeCreators,
                        null,
                        ignoreRules,
                        executeOrderRules, postBuildActions);

            action.ShouldThrow<ArgumentNullException>();
        }

        private class BuilderStrategyWrapper : BuildStrategyBase
        {
            public BuilderStrategyWrapper(
                IConstructorResolver constructorResolver,
                IEnumerable<CreationRule> creationRules,
                IEnumerable<ITypeCreator> typeCreators,
                IEnumerable<IValueGenerator> valueGenerators,
                IEnumerable<IgnoreRule> ignoreRules,
                IEnumerable<ExecuteOrderRule> executeOrderRules,
                IEnumerable<IPostBuildAction> postBuildActions)
                : base(
                    constructorResolver,
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
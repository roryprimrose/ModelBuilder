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
            var constructorResolver = DefaultBuildStrategy.DefaultConstructorResolver;
            var creationRules = DefaultBuildStrategy.DefaultCreationRules.ToList();
            var typeCreators = DefaultBuildStrategy.DefaultTypeCreators.ToList();
            var valueGenerators = DefaultBuildStrategy.DefaultValueGenerators.ToList();
            var ignoreRules = new List<IgnoreRule>
            {
                new IgnoreRule(typeof(Person), "FirstName")
            };
            var executeOrderRules = DefaultBuildStrategy.DefaultExecuteOrderRules.ToList();
            var postBuildActions = new List<IPostBuildAction>
            {
                Substitute.For<IPostBuildAction>()
            };
            var buildLog = DefaultBuildStrategy.DefaultBuildLog;

            var actual = new BuilderStrategyWrapper(
                constructorResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                executeOrderRules, postBuildActions,
                buildLog);

            actual.BuildLog.Should().Be(buildLog);
            actual.ConstructorResolver.Should().Be(constructorResolver);
            actual.ExecuteOrderRules.ShouldAllBeEquivalentTo(executeOrderRules);
            actual.PostBuildActions.ShouldAllBeEquivalentTo(postBuildActions);
            actual.CreationRules.ShouldAllBeEquivalentTo(creationRules);
            actual.IgnoreRules.ShouldAllBeEquivalentTo(ignoreRules);
            actual.TypeCreators.ShouldAllBeEquivalentTo(typeCreators);
            actual.ValueGenerators.ShouldAllBeEquivalentTo(valueGenerators);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullBuildLogTest()
        {
            var constructorResolver = DefaultBuildStrategy.DefaultConstructorResolver;
            var creationRules = DefaultBuildStrategy.DefaultCreationRules;
            var typeCreators = DefaultBuildStrategy.DefaultTypeCreators;
            var valueGenerators = DefaultBuildStrategy.DefaultValueGenerators;
            var ignoreRules = DefaultBuildStrategy.DefaultIgnoreRules;
            var executeOrderRules = DefaultBuildStrategy.DefaultExecuteOrderRules;
            var postBuildActions = DefaultBuildStrategy.DefaultPostBuildActions;

            Action action =
                () =>
                    new BuilderStrategyWrapper(
                        constructorResolver,
                        creationRules,
                        typeCreators,
                        valueGenerators,
                        ignoreRules,
                        executeOrderRules, postBuildActions,
                        null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullCreationRulesTest()
        {
            var constructorResolver = DefaultBuildStrategy.DefaultConstructorResolver;
            var typeCreators = DefaultBuildStrategy.DefaultTypeCreators;
            var valueGenerators = DefaultBuildStrategy.DefaultValueGenerators;
            var ignoreRules = DefaultBuildStrategy.DefaultIgnoreRules;
            var executeOrderRules = DefaultBuildStrategy.DefaultExecuteOrderRules;
            var postBuildActions = DefaultBuildStrategy.DefaultPostBuildActions;
            var buildLog = DefaultBuildStrategy.DefaultBuildLog;

            Action action =
                () =>
                    new BuilderStrategyWrapper(
                        constructorResolver,
                        null,
                        typeCreators,
                        valueGenerators,
                        ignoreRules,
                        executeOrderRules, postBuildActions,
                        buildLog);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullExecuteOrderRulesTest()
        {
            var constructorResolver = DefaultBuildStrategy.DefaultConstructorResolver;
            var creationRules = DefaultBuildStrategy.DefaultCreationRules;
            var typeCreators = DefaultBuildStrategy.DefaultTypeCreators;
            var valueGenerators = DefaultBuildStrategy.DefaultValueGenerators;
            var ignoreRules = DefaultBuildStrategy.DefaultIgnoreRules;
            var postBuildActions = DefaultBuildStrategy.DefaultPostBuildActions;
            var buildLog = DefaultBuildStrategy.DefaultBuildLog;

            Action action =
                () =>
                    new BuilderStrategyWrapper(
                        constructorResolver,
                        creationRules,
                        typeCreators,
                        valueGenerators,
                        ignoreRules,
                        null,
                        postBuildActions,
                        buildLog);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullIgnoreRulesTest()
        {
            var constructorResolver = DefaultBuildStrategy.DefaultConstructorResolver;
            var creationRules = DefaultBuildStrategy.DefaultCreationRules;
            var typeCreators = DefaultBuildStrategy.DefaultTypeCreators;
            var valueGenerators = DefaultBuildStrategy.DefaultValueGenerators;
            var executeOrderRules = DefaultBuildStrategy.DefaultExecuteOrderRules;
            var postBuildActions = DefaultBuildStrategy.DefaultPostBuildActions;
            var buildLog = DefaultBuildStrategy.DefaultBuildLog;

            Action action =
                () =>
                    new BuilderStrategyWrapper(
                        constructorResolver,
                        creationRules,
                        typeCreators,
                        valueGenerators,
                        null,
                        executeOrderRules, postBuildActions,
                        buildLog);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullPostBuildActionsTest()
        {
            var constructorResolver = DefaultBuildStrategy.DefaultConstructorResolver;
            var creationRules = DefaultBuildStrategy.DefaultCreationRules;
            var typeCreators = DefaultBuildStrategy.DefaultTypeCreators;
            var valueGenerators = DefaultBuildStrategy.DefaultValueGenerators;
            var ignoreRules = DefaultBuildStrategy.DefaultIgnoreRules;
            var executeOrderRules = DefaultBuildStrategy.DefaultExecuteOrderRules;
            var buildLog = DefaultBuildStrategy.DefaultBuildLog;

            Action action =
                () =>
                    new BuilderStrategyWrapper(
                        constructorResolver,
                        creationRules,
                        typeCreators,
                        valueGenerators,
                        ignoreRules,
                        executeOrderRules,
                        null,
                        buildLog);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullResolverTest()
        {
            var typeCreators = DefaultBuildStrategy.DefaultTypeCreators;
            var creationRules = DefaultBuildStrategy.DefaultCreationRules;
            var valueGenerators = DefaultBuildStrategy.DefaultValueGenerators;
            var ignoreRules = DefaultBuildStrategy.DefaultIgnoreRules;
            var executeOrderRules = DefaultBuildStrategy.DefaultExecuteOrderRules;
            var postBuildActions = DefaultBuildStrategy.DefaultPostBuildActions;
            var buildLog = DefaultBuildStrategy.DefaultBuildLog;

            Action action =
                () =>
                    new BuilderStrategyWrapper(
                        null,
                        creationRules,
                        typeCreators,
                        valueGenerators,
                        ignoreRules,
                        executeOrderRules, postBuildActions,
                        buildLog);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullTypeCreatorsTest()
        {
            var constructorResolver = DefaultBuildStrategy.DefaultConstructorResolver;
            var creationRules = DefaultBuildStrategy.DefaultCreationRules;
            var valueGenerators = DefaultBuildStrategy.DefaultValueGenerators;
            var ignoreRules = DefaultBuildStrategy.DefaultIgnoreRules;
            var executeOrderRules = DefaultBuildStrategy.DefaultExecuteOrderRules;
            var postBuildActions = DefaultBuildStrategy.DefaultPostBuildActions;
            var buildLog = DefaultBuildStrategy.DefaultBuildLog;

            Action action =
                () =>
                    new BuilderStrategyWrapper(
                        constructorResolver,
                        creationRules,
                        null,
                        valueGenerators,
                        ignoreRules,
                        executeOrderRules, postBuildActions,
                        buildLog);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullValueGeneratorsTest()
        {
            var constructorResolver = DefaultBuildStrategy.DefaultConstructorResolver;
            var creationRules = DefaultBuildStrategy.DefaultCreationRules;
            var typeCreators = DefaultBuildStrategy.DefaultTypeCreators;
            var ignoreRules = DefaultBuildStrategy.DefaultIgnoreRules;
            var executeOrderRules = DefaultBuildStrategy.DefaultExecuteOrderRules;
            var postBuildActions = DefaultBuildStrategy.DefaultPostBuildActions;
            var buildLog = DefaultBuildStrategy.DefaultBuildLog;

            Action action =
                () =>
                    new BuilderStrategyWrapper(
                        constructorResolver,
                        creationRules,
                        typeCreators,
                        null,
                        ignoreRules,
                        executeOrderRules, postBuildActions,
                        buildLog);

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
                IEnumerable<IPostBuildAction> postBuildActions,
                IBuildLog buildLog)
                : base(
                    constructorResolver,
                    creationRules,
                    typeCreators,
                    valueGenerators,
                    ignoreRules,
                    executeOrderRules,
                    postBuildActions,
                    buildLog)
            {
            }

            public override IExecuteStrategy<T> GetExecuteStrategy<T>()
            {
                throw new NotImplementedException();
            }
        }
    }
}
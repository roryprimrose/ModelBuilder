using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class BuilderStrategyBaseTests
    {
        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullExecuteOrderRulesTest()
        {
            var constructorResolver = DefaultBuildStrategy.DefaultConstructorResolver;
            var typeCreators = DefaultBuildStrategy.DefaultTypeCreators;
            var valueGenerators = DefaultBuildStrategy.DefaultValueGenerators;
            var ignoreRules = DefaultBuildStrategy.DefaultIgnoreRules;

            Action action =
                () => new BuilderStrategyWrapper(constructorResolver, typeCreators, valueGenerators, ignoreRules, null);

            action.ShouldThrow<ArgumentNullException>();
        }


        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullIgnoreRulesTest()
        {
            var constructorResolver = DefaultBuildStrategy.DefaultConstructorResolver;
            var typeCreators = DefaultBuildStrategy.DefaultTypeCreators;
            var valueGenerators = DefaultBuildStrategy.DefaultValueGenerators;
            var executeOrderRules = DefaultBuildStrategy.DefaultExecuteOrderRules;

            Action action =
                () =>
                    new BuilderStrategyWrapper(constructorResolver, typeCreators, valueGenerators, null,
                        executeOrderRules);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullResolverTest()
        {
            var typeCreators = DefaultBuildStrategy.DefaultTypeCreators;
            var valueGenerators = DefaultBuildStrategy.DefaultValueGenerators;
            var ignoreRules = DefaultBuildStrategy.DefaultIgnoreRules;
            var executeOrderRules = DefaultBuildStrategy.DefaultExecuteOrderRules;

            Action action =
                () => new BuilderStrategyWrapper(null, typeCreators, valueGenerators, ignoreRules, executeOrderRules);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullTypeCreatorsTest()
        {
            var constructorResolver = DefaultBuildStrategy.DefaultConstructorResolver;
            var valueGenerators = DefaultBuildStrategy.DefaultValueGenerators;
            var ignoreRules = DefaultBuildStrategy.DefaultIgnoreRules;
            var executeOrderRules = DefaultBuildStrategy.DefaultExecuteOrderRules;

            Action action =
                () =>
                    new BuilderStrategyWrapper(constructorResolver, null, valueGenerators, ignoreRules,
                        executeOrderRules);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullValueCreatorsTest()
        {
            var constructorResolver = DefaultBuildStrategy.DefaultConstructorResolver;
            var typeCreators = DefaultBuildStrategy.DefaultTypeCreators;
            var ignoreRules = DefaultBuildStrategy.DefaultIgnoreRules;
            var executeOrderRules = DefaultBuildStrategy.DefaultExecuteOrderRules;

            Action action =
                () =>
                    new BuilderStrategyWrapper(constructorResolver, typeCreators, null, ignoreRules, executeOrderRules);

            action.ShouldThrow<ArgumentNullException>();
        }


        private class BuilderStrategyWrapper : BuildStrategyBase
        {
            public BuilderStrategyWrapper(IConstructorResolver constructorResolver,
                IEnumerable<ITypeCreator> typeCreators, IEnumerable<IValueGenerator> valueGenerators,
                IEnumerable<IgnoreRule> ignoreRules, IEnumerable<ExecuteOrderRule> executeOrderRules)
                : base(constructorResolver, typeCreators, valueGenerators, ignoreRules, executeOrderRules)
            {
            }

            public override IExecuteStrategy<T> GetExecuteStrategy<T>()
            {
                throw new NotImplementedException();
            }
        }
    }
}
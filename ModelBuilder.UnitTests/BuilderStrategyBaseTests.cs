using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class BuilderStrategyBaseTests
    {
        [Fact]
        public void ReturnsValuesFromConstructorTest()
        {
            var constructorResolver = DefaultBuildStrategy.DefaultConstructorResolver;
            var typeCreators = DefaultBuildStrategy.DefaultTypeCreators.ToList();
            var valueGenerators = DefaultBuildStrategy.DefaultValueGenerators.ToList();
            var ignoreRules = new List<IgnoreRule>
            {
                new IgnoreRule(typeof(Person), "FirstName")
            };
            var executeOrderRules = DefaultBuildStrategy.DefaultExecuteOrderRules.ToList();
            var buildLog = DefaultBuildStrategy.DefaultBuildLog;

            var actual = new BuilderStrategyWrapper(constructorResolver, typeCreators, valueGenerators, ignoreRules,
                executeOrderRules,
                buildLog);

            actual.BuildLog.Should().Be(buildLog);
            actual.ConstructorResolver.Should().Be(constructorResolver);
            actual.ExecuteOrderRules.ShouldAllBeEquivalentTo(executeOrderRules);
            actual.IgnoreRules.ShouldAllBeEquivalentTo(ignoreRules);
            actual.TypeCreators.ShouldAllBeEquivalentTo(typeCreators);
            actual.ValueGenerators.ShouldAllBeEquivalentTo(valueGenerators);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullBuildLogTest()
        {
            var constructorResolver = DefaultBuildStrategy.DefaultConstructorResolver;
            var typeCreators = DefaultBuildStrategy.DefaultTypeCreators;
            var valueGenerators = DefaultBuildStrategy.DefaultValueGenerators;
            var ignoreRules = DefaultBuildStrategy.DefaultIgnoreRules;
            var executeOrderRules = DefaultBuildStrategy.DefaultExecuteOrderRules;

            Action action =
                () =>
                    new BuilderStrategyWrapper(constructorResolver, typeCreators, valueGenerators, ignoreRules,
                        executeOrderRules, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullExecuteOrderRulesTest()
        {
            var constructorResolver = DefaultBuildStrategy.DefaultConstructorResolver;
            var typeCreators = DefaultBuildStrategy.DefaultTypeCreators;
            var valueGenerators = DefaultBuildStrategy.DefaultValueGenerators;
            var ignoreRules = DefaultBuildStrategy.DefaultIgnoreRules;
            var buildLog = DefaultBuildStrategy.DefaultBuildLog;

            Action action =
                () =>
                    new BuilderStrategyWrapper(constructorResolver, typeCreators, valueGenerators, ignoreRules, null,
                        buildLog);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullIgnoreRulesTest()
        {
            var constructorResolver = DefaultBuildStrategy.DefaultConstructorResolver;
            var typeCreators = DefaultBuildStrategy.DefaultTypeCreators;
            var valueGenerators = DefaultBuildStrategy.DefaultValueGenerators;
            var executeOrderRules = DefaultBuildStrategy.DefaultExecuteOrderRules;
            var buildLog = DefaultBuildStrategy.DefaultBuildLog;

            Action action =
                () =>
                    new BuilderStrategyWrapper(constructorResolver, typeCreators, valueGenerators, null,
                        executeOrderRules, buildLog);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullResolverTest()
        {
            var typeCreators = DefaultBuildStrategy.DefaultTypeCreators;
            var valueGenerators = DefaultBuildStrategy.DefaultValueGenerators;
            var ignoreRules = DefaultBuildStrategy.DefaultIgnoreRules;
            var executeOrderRules = DefaultBuildStrategy.DefaultExecuteOrderRules;
            var buildLog = DefaultBuildStrategy.DefaultBuildLog;

            Action action =
                () =>
                    new BuilderStrategyWrapper(null, typeCreators, valueGenerators, ignoreRules, executeOrderRules,
                        buildLog);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullTypeCreatorsTest()
        {
            var constructorResolver = DefaultBuildStrategy.DefaultConstructorResolver;
            var valueGenerators = DefaultBuildStrategy.DefaultValueGenerators;
            var ignoreRules = DefaultBuildStrategy.DefaultIgnoreRules;
            var executeOrderRules = DefaultBuildStrategy.DefaultExecuteOrderRules;
            var buildLog = DefaultBuildStrategy.DefaultBuildLog;

            Action action =
                () =>
                    new BuilderStrategyWrapper(constructorResolver, null, valueGenerators, ignoreRules,
                        executeOrderRules, buildLog);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullValueCreatorsTest()
        {
            var constructorResolver = DefaultBuildStrategy.DefaultConstructorResolver;
            var typeCreators = DefaultBuildStrategy.DefaultTypeCreators;
            var ignoreRules = DefaultBuildStrategy.DefaultIgnoreRules;
            var executeOrderRules = DefaultBuildStrategy.DefaultExecuteOrderRules;
            var buildLog = DefaultBuildStrategy.DefaultBuildLog;

            Action action =
                () =>
                    new BuilderStrategyWrapper(constructorResolver, typeCreators, null, ignoreRules, executeOrderRules,
                        buildLog);

            action.ShouldThrow<ArgumentNullException>();
        }
        
        private class BuilderStrategyWrapper : BuildStrategyBase
        {
            public BuilderStrategyWrapper(IConstructorResolver constructorResolver,
                IEnumerable<ITypeCreator> typeCreators, IEnumerable<IValueGenerator> valueGenerators,
                IEnumerable<IgnoreRule> ignoreRules, IEnumerable<ExecuteOrderRule> executeOrderRules, IBuildLog buildLog)
                : base(constructorResolver, typeCreators, valueGenerators, ignoreRules, executeOrderRules, buildLog)
            {
            }

            public override IExecuteStrategy<T> GetExecuteStrategy<T>()
            {
                throw new NotImplementedException();
            }
        }
    }
}
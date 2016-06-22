namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class BuildStrategyTests
    {
        [Fact]
        public void GetExecuteStrategyReturnsDefaultStrategyTest()
        {
            var buildLog = Substitute.For<IBuildLog>();
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var creationRules = new List<CreationRule>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();
            var postBuildActions = new List<IPostBuildAction>();

            var target = new BuildStrategy(
                constructorResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                executeOrderRules,
                postBuildActions,
                buildLog);

            var actual = target.GetExecuteStrategy<Person>();

            actual.Should().BeOfType<DefaultExecuteStrategy<Person>>();
        }

        [Fact]
        public void ReturnsConstructorValuesTest()
        {
            var buildLog = Substitute.For<IBuildLog>();
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var creationRules = new List<CreationRule>
            {
                new CreationRule(typeof(string), "Test", int.MaxValue, "Stuff")
            };
            var typeCreators = new List<ITypeCreator>
            {
                new DefaultTypeCreator()
            };
            var valueGenerators = new List<IValueGenerator>
            {
                new AddressValueGenerator()
            };
            var ignoreRules = new List<IgnoreRule>
            {
                new IgnoreRule(typeof(Person), "FirstName")
            };
            var executeOrderRules = new List<ExecuteOrderRule>
            {
                new ExecuteOrderRule(typeof(Person), "LastName", int.MinValue)
            };
            var postBuildActions = new List<IPostBuildAction>
            {
                Substitute.For<IPostBuildAction>()
            };

            var target = new BuildStrategy(
                constructorResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                executeOrderRules,
                postBuildActions,
                buildLog);

            target.BuildLog.Should().Be(buildLog);
            target.ConstructorResolver.Should().Be(constructorResolver);
            target.CreationRules.ShouldBeEquivalentTo(creationRules);
            target.TypeCreators.ShouldBeEquivalentTo(typeCreators);
            target.ValueGenerators.ShouldBeEquivalentTo(valueGenerators);
            target.IgnoreRules.ShouldBeEquivalentTo(ignoreRules);
            target.ExecuteOrderRules.ShouldBeEquivalentTo(executeOrderRules);
            target.PostBuildActions.ShouldBeEquivalentTo(postBuildActions);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullBuildLogTest()
        {
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var creationRules = new List<CreationRule>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();
            var postBuildActions = new List<IPostBuildAction>();

            Action action =
                () =>
                    new BuildStrategy(
                        constructorResolver,
                        creationRules,
                        typeCreators,
                        valueGenerators,
                        ignoreRules,
                        executeOrderRules,
                        postBuildActions,
                        null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullConstructorResolverTest()
        {
            var buildLog = Substitute.For<IBuildLog>();
            var creationRules = new List<CreationRule>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();
            var postBuildActions = new List<IPostBuildAction>();

            Action action =
                () =>
                    new BuildStrategy(
                        null,
                        creationRules,
                        typeCreators,
                        valueGenerators,
                        ignoreRules,
                        executeOrderRules,
                        postBuildActions,
                        buildLog);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullCreationRulesTest()
        {
            var buildLog = Substitute.For<IBuildLog>();
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();
            var postBuildActions = new List<IPostBuildAction>();

            Action action =
                () =>
                    new BuildStrategy(
                        constructorResolver,
                        null,
                        typeCreators,
                        valueGenerators,
                        ignoreRules,
                        executeOrderRules,
                        postBuildActions,
                        buildLog);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullExecuteOrderRulesTest()
        {
            var buildLog = Substitute.For<IBuildLog>();
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var creationRules = new List<CreationRule>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var postBuildActions = new List<IPostBuildAction>();

            Action action =
                () =>
                    new BuildStrategy(
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
            var buildLog = Substitute.For<IBuildLog>();
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var creationRules = new List<CreationRule>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var executeOrderRules = new List<ExecuteOrderRule>();
            var postBuildActions = new List<IPostBuildAction>();

            Action action =
                () =>
                    new BuildStrategy(
                        constructorResolver,
                        creationRules,
                        typeCreators,
                        valueGenerators,
                        null,
                        executeOrderRules,
                        postBuildActions,
                        buildLog);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullPostBuildActionsTest()
        {
            var buildLog = Substitute.For<IBuildLog>();
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var creationRules = new List<CreationRule>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();

            Action action =
                () =>
                    new BuildStrategy(
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
        public void ThrowsExceptionWhenCreatedWithNullTypeCreatorsTest()
        {
            var buildLog = Substitute.For<IBuildLog>();
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var creationRules = new List<CreationRule>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();
            var postBuildActions = new List<IPostBuildAction>();

            Action action =
                () =>
                    new BuildStrategy(
                        constructorResolver,
                        creationRules,
                        null,
                        valueGenerators,
                        ignoreRules,
                        executeOrderRules,
                        postBuildActions,
                        buildLog);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullValueGeneratorsTest()
        {
            var buildLog = Substitute.For<IBuildLog>();
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var creationRules = new List<CreationRule>();
            var typeCreators = new List<ITypeCreator>();
            var ignoreRules = new List<IgnoreRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();
            var postBuildActions = new List<IPostBuildAction>();

            Action action =
                () =>
                    new BuildStrategy(
                        constructorResolver,
                        creationRules,
                        typeCreators,
                        null,
                        ignoreRules,
                        executeOrderRules,
                        postBuildActions,
                        buildLog);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
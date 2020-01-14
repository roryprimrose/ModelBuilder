// ReSharper disable CollectionNeverUpdated.Local

namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using FluentAssertions;
    using Models;
    using NSubstitute;
    using Xunit;

    public class BuildStrategyTests
    {
        [Fact]
        public void GetBuildLogReturnsDefaultBuildLogTest()
        {
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            var creationRules = new List<CreationRule>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var typeMappingRules = new List<TypeMappingRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();
            var postBuildActions = new List<IPostBuildAction>();

            var target = new BuildStrategy(
                constructorResolver,
                propertyResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                typeMappingRules,
                executeOrderRules,
                postBuildActions);

            var actual = target.GetBuildLog();

            actual.Should().BeOfType<DefaultBuildLog>();
        }

        [Fact]
        public void GetBuildLogReturnsUniqueInstanceTest()
        {
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            var creationRules = new List<CreationRule>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var typeMappingRules = new List<TypeMappingRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();
            var postBuildActions = new List<IPostBuildAction>();

            var target = new BuildStrategy(
                constructorResolver,
                propertyResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                typeMappingRules,
                executeOrderRules,
                postBuildActions);

            var first = target.GetBuildLog();
            var second = target.GetBuildLog();

            first.Should().NotBeSameAs(second);
        }

        [Fact]
        public void GetExecuteStrategyReturnsDefaultStrategyTest()
        {
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            var creationRules = new List<CreationRule>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var typeMappingRules = new List<TypeMappingRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();
            var postBuildActions = new List<IPostBuildAction>();

            var target = new BuildStrategy(
                constructorResolver,
                propertyResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                typeMappingRules,
                executeOrderRules,
                postBuildActions);

            var actual = target.GetExecuteStrategy<Person>();

            actual.Should().BeOfType<DefaultExecuteStrategy<Person>>();
        }

        [Fact]
        public void ReturnsConstructorValuesTest()
        {
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
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
            var typeMappingRules = new List<TypeMappingRule>
            {
                new TypeMappingRule(typeof(Stream), typeof(MemoryStream))
            };
            var executeOrderRules = new List<ExecuteOrderRule>
            {
                new ExecuteOrderRule(typeof(Person), typeof(string), "LastName", int.MinValue)
            };
            var postBuildActions = new List<IPostBuildAction>
            {
                Substitute.For<IPostBuildAction>()
            };

            var target = new BuildStrategy(
                constructorResolver,
                propertyResolver,
                creationRules,
                typeCreators,
                valueGenerators,
                ignoreRules,
                typeMappingRules,
                executeOrderRules,
                postBuildActions);

            target.ConstructorResolver.Should().Be(constructorResolver);
            target.PropertyResolver.Should().Be(propertyResolver);
            target.CreationRules.Should().BeEquivalentTo(creationRules);
            target.TypeCreators.Should().BeEquivalentTo(typeCreators);
            target.ValueGenerators.Should().BeEquivalentTo(valueGenerators);
            target.IgnoreRules.Should().BeEquivalentTo(ignoreRules);
            target.TypeMappingRules.Should().BeEquivalentTo(typeMappingRules);
            target.ExecuteOrderRules.Should().BeEquivalentTo(executeOrderRules);
            target.PostBuildActions.Should().BeEquivalentTo(postBuildActions);
        }

        [Fact]
        public void ReturnsCopyConstructorValuesTest()
        {
            var expected = new DefaultBuildStrategyCompiler().Compile();

            var target = new BuildStrategy(expected);

            target.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullConstructorResolverTest()
        {
            var propertyResolver = Substitute.For<IPropertyResolver>();
            var creationRules = new List<CreationRule>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var typeMappingRules = new List<TypeMappingRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();
            var postBuildActions = new List<IPostBuildAction>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuildStrategy(
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
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var typeMappingRules = new List<TypeMappingRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();
            var postBuildActions = new List<IPostBuildAction>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuildStrategy(
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
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            var creationRules = new List<CreationRule>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var typeMappingRules = new List<TypeMappingRule>();
            var postBuildActions = new List<IPostBuildAction>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuildStrategy(
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
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            var creationRules = new List<CreationRule>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var typeMappingRules = new List<TypeMappingRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();
            var postBuildActions = new List<IPostBuildAction>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuildStrategy(
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
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            var creationRules = new List<CreationRule>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var typeMappingRules = new List<TypeMappingRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuildStrategy(
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
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var creationRules = new List<CreationRule>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var typeMappingRules = new List<TypeMappingRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();
            var postBuildActions = new List<IPostBuildAction>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuildStrategy(
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
            Action action = () => new BuildStrategy(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullTypeCreatorsTest()
        {
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            var creationRules = new List<CreationRule>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var typeMappingRules = new List<TypeMappingRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();
            var postBuildActions = new List<IPostBuildAction>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuildStrategy(
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
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            var creationRules = new List<CreationRule>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();
            var postBuildActions = new List<IPostBuildAction>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuildStrategy(
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
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            var creationRules = new List<CreationRule>();
            var typeCreators = new List<ITypeCreator>();
            var ignoreRules = new List<IgnoreRule>();
            var typeMappingRules = new List<TypeMappingRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();
            var postBuildActions = new List<IPostBuildAction>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuildStrategy(
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
    }
}
namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class BuildStrategyCompilerTests
    {
        [Fact]
        public void CompileReturnsNewBuildStrategyTest()
        {
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            var creationRules = new List<CreationRule>
            {
                new CreationRule(typeof(string), "Test", int.MaxValue, "Stuff")
            };
            var typeCreators = new List<ITypeCreator> {new DefaultTypeCreator()};
            var valueGenerators = new List<IValueGenerator> {new AddressValueGenerator()};
            var ignoreRules = new List<IgnoreRule> {new IgnoreRule(typeof(Person), "FirstName")};
            var typeMappingRules =
                new List<TypeMappingRule> {new TypeMappingRule(typeof(Stream), typeof(MemoryStream))};
            var executeOrderRules = new List<ExecuteOrderRule>
            {
                new ExecuteOrderRule(typeof(Person), typeof(string), "LastName", int.MinValue)
            };
            var postBuildActions = new List<IPostBuildAction> {Substitute.For<IPostBuildAction>()};

            var target = new BuildStrategyCompiler
            {
                ConstructorResolver = constructorResolver, PropertyResolver = propertyResolver
            };

            target.CreationRules.Add(creationRules[0]);
            target.TypeCreators.Add(typeCreators[0]);
            target.ValueGenerators.Add(valueGenerators[0]);
            target.IgnoreRules.Add(ignoreRules[0]);
            target.TypeMappingRules.Add(typeMappingRules[0]);
            target.ExecuteOrderRules.Add(executeOrderRules[0]);
            target.PostBuildActions.Add(postBuildActions[0]);

            var actual = target.Compile();

            actual.ConstructorResolver.Should().Be(constructorResolver);
            actual.PropertyResolver.Should().Be(propertyResolver);
            actual.CreationRules.Should().BeEquivalentTo(creationRules);
            actual.TypeCreators.Should().BeEquivalentTo(typeCreators);
            actual.ValueGenerators.Should().BeEquivalentTo(valueGenerators);
            actual.IgnoreRules.Should().BeEquivalentTo(ignoreRules);
            actual.TypeMappingRules.Should().BeEquivalentTo(typeMappingRules);
            actual.ExecuteOrderRules.Should().BeEquivalentTo(executeOrderRules);
            actual.PostBuildActions.Should().BeEquivalentTo(postBuildActions);
        }

        [Fact]
        public void CompileThrowsExceptionWithNullConstructorResolverTest()
        {
            var propertyResolver = Substitute.For<IPropertyResolver>();

            var target = new BuildStrategyCompiler {PropertyResolver = propertyResolver};

            Action action = () => target.Compile();

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void CompileThrowsExceptionWithNullPropertyResolverTest()
        {
            var constructorResolver = Substitute.For<IConstructorResolver>();

            var target = new BuildStrategyCompiler {ConstructorResolver = constructorResolver};

            Action action = () => target.Compile();

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void CreatesDefaultsForPropertiesTest()
        {
            var target = new BuildStrategyCompiler();

            target.ConstructorResolver.Should().BeNull();
            target.PropertyResolver.Should().BeNull();
            target.CreationRules.Should().NotBeNull();
            target.ExecuteOrderRules.Should().NotBeNull();
            target.IgnoreRules.Should().NotBeNull();
            target.TypeMappingRules.Should().NotBeNull();
            target.TypeCreators.Should().NotBeNull();
            target.ValueGenerators.Should().NotBeNull();
            target.PostBuildActions.Should().NotBeNull();
        }
    }
}
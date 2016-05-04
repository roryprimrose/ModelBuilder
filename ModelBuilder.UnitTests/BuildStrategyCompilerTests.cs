using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class BuildStrategyCompilerTests
    {
        [Fact]
        public void CreatesDefaultsForPropertiesTest()
        {
            var target = new BuildStrategyCompiler();

            target.ConstructorResolver.Should().BeNull();
            target.ExecuteOrderRules.Should().NotBeNull();
            target.IgnoreRules.Should().NotBeNull();
            target.TypeCreators.Should().NotBeNull();
            target.ValueGenerators.Should().NotBeNull();
        }

        [Fact]
        public void CompileThrowsExceptionWithNullConstructorResolverTest()
        {
            var target = new BuildStrategyCompiler();

            Action action = () => target.Compile();

            action.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void CompileReturnsNewBuildStrategyTest()
        {
            var constructorResolver = Substitute.For<IConstructorResolver>();
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

            var target = new BuildStrategyCompiler();

            target.ConstructorResolver = constructorResolver;
            target.TypeCreators.Add(typeCreators[0]);
            target.ValueGenerators.Add(valueGenerators[0]);
            target.IgnoreRules.Add(ignoreRules[0]);
            target.ExecuteOrderRules.Add(executeOrderRules[0]);

            var actual = target.Compile();

            actual.ConstructorResolver.Should().Be(constructorResolver);
            actual.TypeCreators.ShouldBeEquivalentTo(typeCreators);
            actual.ValueGenerators.ShouldBeEquivalentTo(valueGenerators);
            actual.IgnoreRules.ShouldBeEquivalentTo(ignoreRules);
            actual.ExecuteOrderRules.ShouldBeEquivalentTo(executeOrderRules);
        }
    }
}
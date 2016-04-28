using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class BuildStrategyTests
    {
        [Fact]
        public void GetExecuteStrategyReturnsDefaultStrategyTest()
        {
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();

            var target = new BuildStrategy(constructorResolver, typeCreators, valueGenerators, ignoreRules,
                executeOrderRules);

            var actual = target.GetExecuteStrategy<Person>();

            actual.Should().BeOfType<DefaultExecuteStrategy<Person>>();
        }

        [Fact]
        public void ReturnsConstructorValuesTest()
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

            var target = new BuildStrategy(constructorResolver, typeCreators, valueGenerators, ignoreRules,
                executeOrderRules);

            target.ConstructorResolver.Should().Be(constructorResolver);
            target.TypeCreators.ShouldBeEquivalentTo(typeCreators);
            target.ValueGenerators.ShouldBeEquivalentTo(valueGenerators);
            target.IgnoreRules.ShouldBeEquivalentTo(ignoreRules);
            target.ExecuteOrderRules.ShouldBeEquivalentTo(executeOrderRules);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithConstructorResolverTest()
        {
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();

            Action action = () => new BuildStrategy(null, typeCreators, valueGenerators, ignoreRules, executeOrderRules);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithExecuteOrderRulesTest()
        {
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();

            Action action =
                () => new BuildStrategy(constructorResolver, typeCreators, valueGenerators, ignoreRules, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithIgnoreRulesTest()
        {
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();
            var executeOrderRules = new List<ExecuteOrderRule>();

            Action action =
                () => new BuildStrategy(constructorResolver, typeCreators, valueGenerators, null, executeOrderRules);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithTypeCreatorsTest()
        {
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var valueGenerators = new List<IValueGenerator>();
            var ignoreRules = new List<IgnoreRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();

            Action action =
                () => new BuildStrategy(constructorResolver, null, valueGenerators, ignoreRules, executeOrderRules);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithValueGeneratorsTest()
        {
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var typeCreators = new List<ITypeCreator>();
            var ignoreRules = new List<IgnoreRule>();
            var executeOrderRules = new List<ExecuteOrderRule>();

            Action action =
                () => new BuildStrategy(constructorResolver, typeCreators, null, ignoreRules, executeOrderRules);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class BuildStrategyCompilerTests
    {
        [Fact]
        public void CompileReturnsNewBuildStrategyTest()
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

            var target = new BuildStrategyCompiler
            {
                BuildLog = buildLog,
                ConstructorResolver = constructorResolver
            };

            target.CreationRules.Add(creationRules[0]);
            target.TypeCreators.Add(typeCreators[0]);
            target.ValueGenerators.Add(valueGenerators[0]);
            target.IgnoreRules.Add(ignoreRules[0]);
            target.ExecuteOrderRules.Add(executeOrderRules[0]);
            target.PostBuildActions.Add(postBuildActions[0]);

            var actual = target.Compile();

            actual.BuildLog.Should().Be(buildLog);
            actual.ConstructorResolver.Should().Be(constructorResolver);
            actual.CreationRules.ShouldBeEquivalentTo(creationRules);
            actual.TypeCreators.ShouldBeEquivalentTo(typeCreators);
            actual.ValueGenerators.ShouldBeEquivalentTo(valueGenerators);
            actual.IgnoreRules.ShouldBeEquivalentTo(ignoreRules);
            actual.ExecuteOrderRules.ShouldBeEquivalentTo(executeOrderRules);
            actual.PostBuildActions.ShouldBeEquivalentTo(postBuildActions);
        }

        [Fact]
        public void CompileThrowsExceptionWithNullConstructorResolverTest()
        {
            var target = new BuildStrategyCompiler();

            Action action = () => target.Compile();

            action.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void CreatesDefaultsForPropertiesTest()
        {
            var target = new BuildStrategyCompiler();

            target.BuildLog.Should().BeNull();
            target.ConstructorResolver.Should().BeNull();
            target.CreationRules.Should().NotBeNull();
            target.ExecuteOrderRules.Should().NotBeNull();
            target.IgnoreRules.Should().NotBeNull();
            target.TypeCreators.Should().NotBeNull();
            target.ValueGenerators.Should().NotBeNull();
            target.PostBuildActions.Should().NotBeNull();
        }
    }
}
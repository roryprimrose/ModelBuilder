﻿namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class BuildConfigurationExtensionsTests
    {
        [Fact]
        public void CloneReturnsCompilerWithBuildStrategyConfigurationTest()
        {
            var target = new DefaultBuildStrategyCompiler().AddIgnoreRule<Person>(x => x.Address)
                .AddCreationRule<Company>(x => x.Address, 100, "stuff").Compile();

            var actual = target.Clone();

            actual.ConstructorResolver.Should().Be(target.ConstructorResolver);
            actual.PropertyResolver.Should().Be(target.PropertyResolver);
            actual.CreationRules.Should().BeEquivalentTo(target.CreationRules);
            actual.TypeCreators.Should().BeEquivalentTo(target.TypeCreators);
            actual.ValueGenerators.Should().BeEquivalentTo(target.ValueGenerators);
            actual.IgnoreRules.Should().BeEquivalentTo(target.IgnoreRules);
            actual.TypeMappingRules.Should().BeEquivalentTo(target.TypeMappingRules);
            actual.ExecuteOrderRules.Should().BeEquivalentTo(target.ExecuteOrderRules);
        }

        [Fact]
        public void CloneThrowsExceptionWithNullBuildStrategyTest()
        {
            Action action = () => ((IBuildStrategy)null).Clone();

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
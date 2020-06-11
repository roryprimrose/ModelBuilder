namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.ExecuteOrderRules;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class DefaultParameterResolverTests
    {
        [Theory]
        [InlineData(CacheLevel.Global)]
        [InlineData(CacheLevel.PerInstance)]
        [InlineData(CacheLevel.None)]
        public void CacheLevelReturnsConstructorValue(CacheLevel cacheLevel)
        {
            var sut = new DefaultParameterResolver(cacheLevel);

            sut.CacheLevel.Should().Be(cacheLevel);
        }

        [Theory]
        [InlineData(CacheLevel.Global)]
        [InlineData(CacheLevel.PerInstance)]
        public void GetOrderedParametersReturnsCachedParameters(CacheLevel cacheLevel)
        {
            var configuration = new BuildConfiguration();
            var constructor = typeof(SimpleConstructor).GetConstructors().First();

            var sut = new DefaultParameterResolver(cacheLevel);

            var first = sut.GetOrderedParameters(configuration, constructor);
            var second = sut.GetOrderedParameters(configuration, constructor);

            first.Should().BeSameAs(second);
        }

        [Fact]
        public void GetOrderedParametersReturnsEmptyWhenConstructorHasNoParameters()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var constructor = typeof(Numbers).GetConstructors().First();

            var sut = new DefaultParameterResolver(CacheLevel.PerInstance);

            var actual = sut.GetOrderedParameters(configuration, constructor).ToList();

            actual.Should().BeEmpty();
        }

        [Fact]
        public void GetOrderedParametersReturnsNewInstanceWhenCachedDisabled()
        {
            var configuration = new BuildConfiguration();
            var constructor = typeof(OrderedConstructorParameters).GetConstructors().First();

            var sut = new DefaultParameterResolver(CacheLevel.None);

            var first = sut.GetOrderedParameters(configuration, constructor).ToList();
            var second = sut.GetOrderedParameters(configuration, constructor).ToList();

            first.Should().NotBeSameAs(second);
            first.Should().BeEquivalentTo(second);
        }

        [Fact]
        public void GetOrderedParametersReturnsParametersInDeclaredOrderWhenNoExecuteOrderRulesMatch()
        {
            var configuration = new BuildConfiguration();
            var constructor = typeof(OrderedConstructorParameters).GetConstructors().First();

            var sut = new DefaultParameterResolver(CacheLevel.None);

            var actual = sut.GetOrderedParameters(configuration, constructor).ToList();

            actual[0].Name.Should().Be("email");
            actual[1].Name.Should().Be("domain");
            actual[2].Name.Should().Be("lastName");
            actual[3].Name.Should().Be("firstName");
            actual[4].Name.Should().Be("gender");
        }

        [Fact]
        public void GetOrderedParametersReturnsParametersWhenExecuteOrderRulesIsNull()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var constructor = typeof(OrderedConstructorParameters).GetConstructors().First();

            configuration.ExecuteOrderRules.Returns((ICollection<IExecuteOrderRule>) null!);

            var sut = new DefaultParameterResolver(CacheLevel.PerInstance);

            var actual = sut.GetOrderedParameters(configuration, constructor).ToList();

            actual.Should().HaveCount(5);
        }

        [Fact]
        public void GetOrderedParametersReturnsPropertiesInDescendingOrder()
        {
            var configuration = new BuildConfiguration()
                .AddExecuteOrderRule(NameExpression.Gender, 50)
                .AddExecuteOrderRule(NameExpression.FirstName, 40)
                .AddExecuteOrderRule(NameExpression.LastName, 30)
                .AddExecuteOrderRule(NameExpression.Domain, 20)
                .AddExecuteOrderRule(NameExpression.Email, 10);
            var constructor = typeof(OrderedConstructorParameters).GetConstructors().First();

            var sut = new DefaultParameterResolver(CacheLevel.PerInstance);

            var actual = sut.GetOrderedParameters(configuration, constructor).ToList();

            actual[0].Name.Should().Be("gender");
            actual[1].Name.Should().Be("firstName");
            actual[2].Name.Should().Be("lastName");
            actual[3].Name.Should().Be("domain");
            actual[4].Name.Should().Be("email");
        }

        [Fact]
        public void GetOrderedParametersThrowsExceptionWithNullConfiguration()
        {
            var constructor = typeof(Person).GetConstructors().First();

            var sut = new DefaultParameterResolver(CacheLevel.PerInstance);

            Action action = () => sut.GetOrderedParameters(null!, constructor);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetOrderedParametersThrowsExceptionWithNullMethod()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var sut = new DefaultParameterResolver(CacheLevel.PerInstance);

            Action action = () => sut.GetOrderedParameters(configuration, null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
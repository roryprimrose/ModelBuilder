namespace ModelBuilder.UnitTests.TypeCreators
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class TypeCreatorBaseTests
    {
        [Theory]
        [InlineData(typeof(TypeCreatorBase), false)]
        [InlineData(typeof(IBuildConfiguration), false)]
        [InlineData(typeof(Environment), false)]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(MemoryStream), true)]
        [InlineData(typeof(List<Person>), true)]
        [InlineData(typeof(Person), true)]
        [InlineData(typeof(KeyValuePair<string, Person>), true)]
        public void CanCreateReturnsWhetherTypeCanBeCreatedTest(Type targetType, bool expected)
        {
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, targetType).Returns(targetType);

            var target = new DefaultTypeCreator();

            var actual = target.CanCreate(targetType, configuration, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CanCreateThrowsExceptionWithNullParameterTest()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var target = new TypeCreatorWrapper();

            Action action = () => target.CanCreate((ParameterInfo) null, configuration, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanCreateThrowsExceptionWithNullPropertyTest()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var target = new TypeCreatorWrapper();

            Action action = () => target.CanCreate((PropertyInfo) null, configuration, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanCreateThrowsExceptionWithNullTypeTest()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var target = new TypeCreatorWrapper();

            Action action = () => target.CanCreate((Type) null, configuration, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullParameterTest()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var buildChain = new BuildHistory();

            var target = new TypeCreatorWrapper();

            Action action = () => target.CanPopulate((ParameterInfo) null, configuration, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullPropertyTest()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var buildChain = new BuildHistory();

            var target = new TypeCreatorWrapper();

            Action action = () => target.CanPopulate((PropertyInfo) null, configuration, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullTypeTest()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var buildChain = new BuildHistory();

            var target = new TypeCreatorWrapper();

            Action action = () => target.CanPopulate((Type) null, configuration, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateForParameterThrowsExceptionWithNullStrategyTest()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Create(parameterInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateForPropertyThrowsExceptionWithNullStrategyTest()
        {
            var property = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Create(property, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateForTypeThrowsExceptionWithNullStrategyTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Create(typeof(string), null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateReturnsValueTest()
        {
            var targetType = typeof(List<string>);

            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, targetType).Returns(targetType);
            executeStrategy.Configuration.Returns(configuration);

            var target = new TypeCreatorWrapper();

            var actual = target.Create(targetType, executeStrategy);

            actual.Should().NotBeNull();
        }

        [Fact]
        public void CreateThrowsExceptionWhenTypeNotSupported()
        {
            var targetType = typeof(Environment);

            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, targetType).Returns(targetType);
            executeStrategy.Configuration.Returns(configuration);

            var target = new DefaultTypeCreator();

            Action action = () => target.Create(targetType, executeStrategy);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullParameterTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Create((ParameterInfo) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullPropertyTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Create((PropertyInfo) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullStrategyBuildChainTest()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns((IBuildChain) null);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Create(typeof(string), executeStrategy);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullTypeTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Create((Type) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GeneratorReturnsInstanceTest()
        {
            var target = new TypeCreatorWrapper();

            target.Random.Should().NotBeNull();
        }

        [Fact]
        public void PopulateDoesNotThrowsExceptionWhenPopulateVerificationPassesTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var value = new List<string>();

            var target = new TypeCreatorWrapper();

            Action action = () => target.Populate(value, executeStrategy);

            action.Should().NotThrow();
        }

        [Fact]
        public void PopulateReturnsProvidedInstanceTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var expected = new List<string>();

            var target = new TypeCreatorWrapper();

            var actual = target.Populate(expected, executeStrategy);

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void PopulateThrowsExceptionWhenPopulateVerificationFailsTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Populate(false, executeStrategy);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullExecuteStrategyTest()
        {
            var person = new Person();

            var target = new TypeCreatorWrapper();

            Action action = () => target.Populate(person, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstanceTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Populate(null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullStrategyBuildChainTest()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns((IBuildChain) null);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Populate(typeof(string), executeStrategy);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullTypeTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Populate(null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ResolveBuildTypeReturnsValueFromTypeResolver()
        {
            var requestedType = typeof(IEnumerable<string>);
            var expected = typeof(List<string>);

            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, requestedType).Returns(expected);
            executeStrategy.Configuration.Returns(configuration);

            var target = new ResolveBuildTypeWrapper();

            var actual = target.RunResolveBuildType(requestedType, configuration);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ResolveBuildTypeThrowsExceptionWithNullConfiguration()
        {
            var sut = new ResolveBuildTypeWrapper();

            Action action = () => sut.RunResolveBuildType(typeof(Stream), null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ResolveBuildTypeThrowsExceptionWithNullRequestedType()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var sut = new ResolveBuildTypeWrapper();

            Action action = () => sut.RunResolveBuildType(null, configuration);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetsDefaultConfigurationForCreatorsTest()
        {
            var target = new DummyTypeCreator();

            target.AutoDetectConstructor.Should().BeTrue();
            target.AutoPopulate.Should().BeTrue();
            target.Priority.Should().Be(0);
        }

        private class ResolveBuildTypeWrapper : TypeCreatorBase
        {
            public Type RunResolveBuildType(Type requestedType, IBuildConfiguration buildConfiguration)
            {
                return base.ResolveBuildType(requestedType, buildConfiguration);
            }

            protected override object CreateInstance(Type type, string referenceName, IExecuteStrategy executeStrategy,
                params object[] args)
            {
                throw new NotImplementedException();
            }

            protected override object PopulateInstance(object instance, IExecuteStrategy executeStrategy)
            {
                throw new NotImplementedException();
            }
        }

        private class TypeCreatorWrapper : TypeCreatorBase
        {
            protected override bool CanCreate(Type type, string referenceName, IBuildConfiguration configuration,
                IBuildChain buildChain)
            {
                var canCreate = base.CanCreate(type, referenceName, configuration, buildChain);

                if (canCreate == false)
                {
                    return false;
                }

                return type == typeof(List<string>);
            }

            protected override bool CanPopulate(Type type, string referenceName, IBuildConfiguration configuration,
                IBuildChain buildChain)
            {
                var canPopulate = base.CanPopulate(type, referenceName, configuration, buildChain);

                if (canPopulate == false)
                {
                    return false;
                }

                return type == typeof(List<string>);
            }

            protected override object CreateInstance(
                Type type,
                string referenceName,
                IExecuteStrategy executeStrategy,
                params object[] args)
            {
                return new List<string>();
            }

            protected override object PopulateInstance(object instance, IExecuteStrategy executeStrategy)
            {
                return instance;
            }

            public IRandomGenerator Random => Generator;
        }
    }
}
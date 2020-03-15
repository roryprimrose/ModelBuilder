﻿namespace ModelBuilder.UnitTests.TypeCreators
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

            var actual = target.CanCreate(configuration, null, targetType);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CanCreateThrowsExceptionWithNullParameter()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var target = new TypeCreatorWrapper();

            Action action = () => target.CanCreate(configuration, null, (ParameterInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanCreateThrowsExceptionWithNullProperty()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var target = new TypeCreatorWrapper();

            Action action = () => target.CanCreate(configuration, null, (PropertyInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanCreateThrowsExceptionWithNullType()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var target = new TypeCreatorWrapper();

            Action action = () => target.CanCreate(configuration, null, (Type) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullParameter()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var buildChain = new BuildHistory();

            var target = new TypeCreatorWrapper();

            Action action = () => target.CanPopulate(configuration, buildChain, (ParameterInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullProperty()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var buildChain = new BuildHistory();

            var target = new TypeCreatorWrapper();

            Action action = () => target.CanPopulate(configuration, buildChain, (PropertyInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullType()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var buildChain = new BuildHistory();

            var target = new TypeCreatorWrapper();

            Action action = () => target.CanPopulate(configuration, buildChain, (Type) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateForParameterThrowsExceptionWithNullStrategy()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Create(null, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateForPropertyThrowsExceptionWithNullStrategy()
        {
            var property = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Create(null, property);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateForTypeThrowsExceptionWithNullStrategy()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Create(null, typeof(string));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateReturnsValue()
        {
            var targetType = typeof(List<string>);

            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, targetType).Returns(targetType);
            executeStrategy.Configuration.Returns(configuration);

            var target = new TypeCreatorWrapper();

            var actual = target.Create(executeStrategy, targetType);

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

            Action action = () => target.Create(executeStrategy, targetType);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullParameter()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Create(executeStrategy, (ParameterInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullProperty()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Create(executeStrategy, (PropertyInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullStrategyBuildChain()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns((IBuildChain) null);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Create(executeStrategy, typeof(string));

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullType()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Create(executeStrategy, (Type) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GeneratorReturnsInstance()
        {
            var target = new TypeCreatorWrapper();

            target.Random.Should().NotBeNull();
        }

        [Fact]
        public void PopulateDoesNotThrowsExceptionWhenPopulateVerificationPasses()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var value = new List<string>();

            var target = new TypeCreatorWrapper();

            Action action = () => target.Populate(executeStrategy, value);

            action.Should().NotThrow();
        }

        [Fact]
        public void PopulateReturnsProvidedInstance()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var expected = new List<string>();

            var target = new TypeCreatorWrapper();

            var actual = target.Populate(executeStrategy, expected);

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void PopulateThrowsExceptionWhenPopulateVerificationFails()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Populate(executeStrategy, false);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullExecuteStrategy()
        {
            var person = new Person();

            var target = new TypeCreatorWrapper();

            Action action = () => target.Populate(null, person);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstance()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Populate(executeStrategy, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullStrategyBuildChain()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns((IBuildChain) null);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Populate(executeStrategy, typeof(string));

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullType()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new TypeCreatorWrapper();

            Action action = () => target.Populate(executeStrategy, null);

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
        public void SetsDefaultConfigurationForCreators()
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
                return base.ResolveBuildType(buildConfiguration, requestedType);
            }

            protected override object CreateInstance(IExecuteStrategy executeStrategy,
                Type type, string referenceName,
                params object[] args)
            {
                throw new NotImplementedException();
            }

            protected override object PopulateInstance(IExecuteStrategy executeStrategy, object instance)
            {
                throw new NotImplementedException();
            }
        }

        private class TypeCreatorWrapper : TypeCreatorBase
        {
            protected override bool CanCreate(IBuildConfiguration configuration,
                IBuildChain buildChain, Type type, string referenceName)
            {
                var canCreate = base.CanCreate(configuration, buildChain, type, referenceName);

                if (canCreate == false)
                {
                    return false;
                }

                return type == typeof(List<string>);
            }

            protected override bool CanPopulate(IBuildConfiguration configuration,
                IBuildChain buildChain, Type type, string referenceName)
            {
                var canPopulate = base.CanPopulate(configuration, buildChain, type, referenceName);

                if (canPopulate == false)
                {
                    return false;
                }

                return type == typeof(List<string>);
            }

            protected override object CreateInstance(IExecuteStrategy executeStrategy,
                Type type,
                string referenceName,
                params object[] args)
            {
                return new List<string>();
            }

            protected override object PopulateInstance(IExecuteStrategy executeStrategy, object instance)
            {
                return instance;
            }

            public IRandomGenerator Random => Generator;
        }
    }
}
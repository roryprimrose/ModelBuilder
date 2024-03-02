namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class DefaultTypeResolverTests
    {
        private readonly ITestOutputHelper _output;

        public DefaultTypeResolverTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(typeof(IEnumerable), typeof(List<object>))]
        [InlineData(typeof(ICollection), typeof(List<object>))]
        [InlineData(typeof(IList), typeof(List<object>))]
        [InlineData(typeof(IEnumerable<Person>), typeof(List<Person>))]
        [InlineData(typeof(ICollection<Person>), typeof(List<Person>))]
        [InlineData(typeof(IList<Person>), typeof(List<Person>))]
        [InlineData(typeof(IReadOnlyCollection<Person>), typeof(List<Person>))]
        [InlineData(typeof(IDictionary<Guid, Person>), typeof(Dictionary<Guid, Person>))]
        public void CanCreateEnumerableTypesFromInterfacesWithExpectedResolutionPriority(Type requestedType,
            Type expectedType)
        {
            var configuration = new BuildConfiguration();

            var sut = new DefaultTypeResolver();

            var actual = sut.GetBuildType(configuration, requestedType);

            _output.WriteLine(actual.FullName);

            actual.Should().Be(expectedType);
        }

        [Fact]
        public void GetBuildTypeReturnsCachedType()
        {
            var configuration = new BuildConfiguration();

            var sut = new DefaultTypeResolver();

            var first = sut.GetBuildType(configuration, typeof(INoMatch));
            var second = sut.GetBuildType(configuration, typeof(INoMatch));

            first.Should().BeSameAs(second);
        }

        [Fact]
        public void GetBuildTypeReturnsClassBasedOnGenericEnumerableNonListInterface()
        {
            var configuration = new BuildConfiguration();

            var sut = new DefaultTypeResolver();

            var actual = sut.GetBuildType(configuration, typeof(ICustomCollection<string>));

            actual.Should().Be<CustomCollection<string>>();
        }

        [Fact]
        public void GetBuildTypeReturnsClassBasedOnGenericNonListInterface()
        {
            var configuration = new BuildConfiguration();

            var sut = new DefaultTypeResolver();

            var actual = sut.GetBuildType(configuration, typeof(IGenericContainer<Office>));

            actual.Should().Be<GenericContainer<Office>>();
        }

        [Fact]
        public void GetBuildTypeReturnsDerivedTypeMatchingOnAbstractBaseName()
        {
            var configuration = new BuildConfiguration();

            var sut = new DefaultTypeResolver();

            var actual = sut.GetBuildType(configuration, typeof(SomeClassBase));

            _output.WriteLine(actual.FullName);

            actual.Should().Be<SomeClass>();
        }

        [Fact]
        public void GetBuildTypeReturnsInternalType()
        {
            var configuration = new BuildConfiguration();

            var sut = new DefaultTypeResolver();

            var actual = sut.GetBuildType(configuration, typeof(IInternalItem));

            _output.WriteLine(actual.FullName);

            actual.Should().Be<InternalItem>();
        }

        [Fact]
        public void GetBuildTypeReturnsOriginalTypeWhenNoBetterTypeFound()
        {
            var configuration = new BuildConfiguration();

            var sut = new DefaultTypeResolver();

            var actual = sut.GetBuildType(configuration, typeof(INoMatch));

            _output.WriteLine(actual.FullName);

            actual.Should().Be<INoMatch>();
        }

        [Fact]
        public void GetBuildTypeReturnsOriginalTypeWhenNoBetterTypeFoundWithNullTypeMappingRules()
        {
            var defaultConfiguration = Model.UsingDefaultConfiguration();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.PropertyResolver.Returns(defaultConfiguration.PropertyResolver);
            configuration.ConstructorResolver.Returns(defaultConfiguration.ConstructorResolver);
            configuration.CreationRules.Returns(defaultConfiguration.CreationRules);
            configuration.ExecuteOrderRules.Returns(defaultConfiguration.ExecuteOrderRules);
            configuration.IgnoreRules.Returns(defaultConfiguration.IgnoreRules);
            configuration.PostBuildActions.Returns(defaultConfiguration.PostBuildActions);
            configuration.TypeCreators.Returns(defaultConfiguration.TypeCreators);
            configuration.TypeMappingRules.Returns((ICollection<TypeMappingRule>)null!);
            configuration.TypeResolver.Returns(defaultConfiguration.TypeResolver);
            configuration.ValueGenerators.Returns(defaultConfiguration.ValueGenerators);

            var sut = new DefaultTypeResolver();

            var actual = sut.GetBuildType(configuration, typeof(INoMatch));

            _output.WriteLine(actual.FullName);

            actual.Should().Be<INoMatch>();
        }

        [Fact]
        public void GetBuildTypeReturnsPossibleTypeFromInterface()
        {
            var configuration = new BuildConfiguration();

            var sut = new DefaultTypeResolver();

            var actual = sut.GetBuildType(configuration, typeof(ITestInterface));

            _output.WriteLine(actual.FullName);

            actual.Should().Implement<ITestInterface>();
        }

        [Fact]
        public void GetBuildTypeReturnsPublicTypeOverInternalType()
        {
            var configuration = new BuildConfiguration();

            var sut = new Wrapper(new[] { typeof(InternalNotPickedItem), typeof(PublicPickedItem) });

            var actual = sut.GetBuildType(configuration, typeof(IPublicOverInternal));

            _output.WriteLine(actual.FullName);

            actual.Should().Be<PublicPickedItem>();
        }

        [Fact]
        public void GetBuildTypeReturnsSourceWhenNoTypeMappingIsFound()
        {
            var source = typeof(string);
            var configuration = new BuildConfiguration();

            var sut = new DefaultTypeResolver();

            configuration.Mapping<Stream, MemoryStream>();

            var actual = sut.GetBuildType(configuration, source);

            actual.Should().Be<string>();
        }

        [Fact]
        public void GetBuildTypeReturnsSourceWhenTypeMappingIsEmpty()
        {
            var source = typeof(string);
            var configuration = new BuildConfiguration();

            var sut = new DefaultTypeResolver();

            var actual = sut.GetBuildType(configuration, source);

            actual.Should().Be<string>();
        }

        [Fact]
        public void GetBuildTypeReturnsSourceWhenTypeMappingIsNull()
        {
            var source = typeof(string);

            var configuration = Substitute.For<IBuildConfiguration>();

            var sut = new DefaultTypeResolver();

            var actual = sut.GetBuildType(configuration, source);

            actual.Should().Be<string>();
        }

        [Fact]
        public void GetBuildTypeReturnsTargetTypeWhenSourceIsAbstractClass()
        {
            var source = typeof(Entity);
            var configuration = new BuildConfiguration();

            var sut = new DefaultTypeResolver();

            var actual = sut.GetBuildType(configuration, source);

            actual.Should().Be<Person>();
        }

        [Fact]
        public void GetBuildTypeReturnsTargetTypeWhenSourceIsInterface()
        {
            var source = typeof(ITestItem);
            var configuration = new BuildConfiguration();

            var sut = new DefaultTypeResolver();

            var actual = sut.GetBuildType(configuration, source);

            actual.Should().Be<TestItem>();
        }

        [Fact]
        public void GetBuildTypeReturnsTargetTypeWhenTypeMappingFound()
        {
            var source = typeof(Stream);
            var configuration = new BuildConfiguration();

            configuration.Mapping<Stream, MemoryStream>();

            var sut = new DefaultTypeResolver();

            var actual = sut.GetBuildType(configuration, source);

            actual.Should().Be<MemoryStream>();
        }

        [Fact]
        public void GetBuildTypeThrowsExceptionWithNullConfiguration()
        {
            var source = typeof(string);

            var sut = new DefaultTypeResolver();

            Action action = () => sut.GetBuildType(null!, source);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildTypeThrowsExceptionWithNullSourceType()
        {
            var configuration = new BuildConfiguration();

            var sut = new DefaultTypeResolver();

            Action action = () => sut.GetBuildType(configuration, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : DefaultTypeResolver
        {
            private readonly IEnumerable<Type> _types;

            public Wrapper(IEnumerable<Type> types)
            {
                _types = types;
            }

            protected override IEnumerable<Type> GetTypesToEvaluate(Type requestedType)
            {
                return _types;
            }
        }
    }

    [SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "The interface is used for testing")]
    public interface INoMatch
    {
    }

    public abstract class SomeClassBase
    {
    }

    public class SomeClass : SomeClassBase
    {
    }

    public class NotThisClass : SomeClassBase
    {
    }

    [SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "The interface is used for testing")]
    public interface ITestInterface
    {
    }

    public class FirstClass : ITestInterface
    {
    }

    public class SecondClass : ITestInterface
    {
    }
}
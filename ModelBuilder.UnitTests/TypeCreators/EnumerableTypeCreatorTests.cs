﻿namespace ModelBuilder.UnitTests.TypeCreators
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class EnumerableTypeCreatorTests
    {
        [Fact]
        public void AutoDetectConstructorReturnsFalse()
        {
            var sut = new EnumerableTypeCreator();

            sut.AutoDetectConstructor.Should().BeFalse();
        }

        [Fact]
        public void AutoPopulateReturnsFalse()
        {
            var sut = new EnumerableTypeCreator();

            sut.AutoPopulate.Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(ReadOnlyCollection<int>), false)]
        [InlineData(typeof(IDictionary<string, int>), false)]
        [InlineData(typeof(Tuple<string, bool>), false)]
        [InlineData(typeof(AbstractCollection<Person>), false)]
        [InlineData(typeof(IReadOnlyCollection<int>), false)]
        [InlineData(typeof(IReadOnlyList<int>), false)]
        [InlineData(typeof(ArraySegment<string>), false)]
        [InlineData(typeof(IPAddressCollection), false)]
        [InlineData(typeof(GatewayIPAddressInformationCollection), false)]
        [InlineData(typeof(IPAddressInformationCollection), false)]
        [InlineData(typeof(MulticastIPAddressInformationCollection), false)]
        [InlineData(typeof(UnicastIPAddressInformationCollection), false)]
        [InlineData(typeof(Dictionary<,>.KeyCollection), false)]
        [InlineData(typeof(Dictionary<,>.ValueCollection), false)]
        [InlineData(typeof(Dictionary<string, int>), true)]
        [InlineData(typeof(SortedDictionary<,>.KeyCollection), false)]
        [InlineData(typeof(SortedDictionary<,>.ValueCollection), false)]
        [InlineData(typeof(IEnumerable<string>), true)]
        [InlineData(typeof(ICollection<string>), true)]
        [InlineData(typeof(Collection<string>), true)]
        [InlineData(typeof(IList<string>), true)]
        [InlineData(typeof(List<string>), true)]
        [InlineData(typeof(HashSet<string>), true)]
        [InlineData(typeof(LinkedList<string>), true)]
        [InlineData(typeof(InheritedGenericCollection), true)]
        public void CanCreateReturnsWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.Configuration.Returns(configuration);

            var sut = new EnumerableTypeCreator();

            var actual = sut.CanCreate(configuration, null, type);

            actual.Should().Be(supported);
        }

        [Fact]
        public void CanCreateThrowsExceptionWithNullParameter()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.Configuration.Returns(configuration);

            var sut = new EnumerableTypeCreator();

            Action action = () => sut.CanCreate(configuration, null, (ParameterInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanCreateThrowsExceptionWithNullProperty()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.Configuration.Returns(configuration);

            var sut = new EnumerableTypeCreator();

            Action action = () => sut.CanCreate(configuration, null, (PropertyInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanCreateThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.Configuration.Returns(configuration);

            var sut = new EnumerableTypeCreator();

            Action action = () => sut.CanCreate(configuration, null, (Type) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(ReadOnlyCollection<int>), false)]
        [InlineData(typeof(Tuple<string, bool>), false)]
        [InlineData(typeof(IReadOnlyCollection<int>), false)]
        [InlineData(typeof(IReadOnlyList<int>), false)]
        [InlineData(typeof(IEnumerable<string>), false)]
        [InlineData(typeof(ArraySegment<string>), false)]
        [InlineData(typeof(IPAddressCollection), false)]
        [InlineData(typeof(GatewayIPAddressInformationCollection), false)]
        [InlineData(typeof(IPAddressInformationCollection), false)]
        [InlineData(typeof(MulticastIPAddressInformationCollection), false)]
        [InlineData(typeof(UnicastIPAddressInformationCollection), false)]
        [InlineData(typeof(Dictionary<,>.KeyCollection), false)]
        [InlineData(typeof(Dictionary<,>.ValueCollection), false)]
        [InlineData(typeof(SortedDictionary<,>.KeyCollection), false)]
        [InlineData(typeof(SortedDictionary<,>.ValueCollection), false)]
        [InlineData(typeof(AbstractCollection<Person>), true)]
        [InlineData(typeof(IDictionary<string, int>), true)]
        [InlineData(typeof(Dictionary<string, int>), true)]
        [InlineData(typeof(ICollection<string>), true)]
        [InlineData(typeof(Collection<string>), true)]
        [InlineData(typeof(IList<string>), true)]
        [InlineData(typeof(List<string>), true)]
        [InlineData(typeof(HashSet<string>), true)]
        [InlineData(typeof(LinkedList<string>), true)]
        [InlineData(typeof(InheritedGenericCollection), true)]
        public void CanPopulateReturnsWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var typeResolver = Substitute.For<ITypeResolver>();

            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            configuration.TypeResolver.Returns(typeResolver);

            var sut = new EnumerableTypeCreator();

            var actual = sut.CanPopulate(configuration, null, type);

            actual.Should().Be(supported);
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullConfiguration()
        {
            var sut = new EnumerableTypeCreator();

            Action action = () => sut.CanPopulate(null, null, typeof(string));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullParameter()
        {
            var sut = new EnumerableTypeCreator();
            var configuration = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.CanPopulate(configuration, null, (ParameterInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullProperty()
        {
            var sut = new EnumerableTypeCreator();
            var configuration = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.CanPopulate(configuration, null, (PropertyInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullType()
        {
            var sut = new EnumerableTypeCreator();
            var configuration = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.CanPopulate(configuration, null, (Type) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateChildItemThrowsExceptionWithNullExecuteStrategy()
        {
            var person = new Person();

            var sut = new EnumerableTypeCreatorWrapper();

            Action action = () => sut.CreateItem(typeof(Person), null, person);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateDoesNotPopulateList()
        {
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);

            var sut = new IncrementingEnumerableTypeCreator();

            var result = (IList<int>) sut.Create(executeStrategy, typeof(IList<int>));

            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData(typeof(Dictionary<string, int>))]
        [InlineData(typeof(IEnumerable<string>))]
        [InlineData(typeof(ICollection<string>))]
        [InlineData(typeof(Collection<string>))]
        [InlineData(typeof(IList<string>))]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(LinkedList<string>))]
        [InlineData(typeof(InheritedGenericCollection))]
        public void CreateReturnsInstanceTest(Type type)
        {
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);

            var sut = new EnumerableTypeCreator();

            var actual = sut.Create(executeStrategy, type);

            actual.Should().NotBeNull();
        }

        [Theory]
        [InlineData(typeof(IEnumerable<int>))]
        [InlineData(typeof(ICollection<int>))]
        [InlineData(typeof(IList<int>))]
        public void CreateReturnsNewListOfSpecifiedTypeTest(Type targetType)
        {
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);

            var sut = new EnumerableTypeCreator();

            var actual = sut.Create(executeStrategy, targetType);

            actual.Should().BeOfType<List<int>>();
            actual.As<List<int>>().Should().BeEmpty();
        }

        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(ReadOnlyCollection<int>), false)]
        [InlineData(typeof(IDictionary<string, int>), false)]
        [InlineData(typeof(Tuple<string, bool>), false)]
        [InlineData(typeof(AbstractCollection<Person>), false)]
        [InlineData(typeof(IReadOnlyCollection<int>), false)]
        [InlineData(typeof(IReadOnlyList<int>), false)]
        [InlineData(typeof(ArraySegment<string>), false)]
        [InlineData(typeof(IPAddressCollection), false)]
        [InlineData(typeof(GatewayIPAddressInformationCollection), false)]
        [InlineData(typeof(IPAddressInformationCollection), false)]
        [InlineData(typeof(MulticastIPAddressInformationCollection), false)]
        [InlineData(typeof(UnicastIPAddressInformationCollection), false)]
        [InlineData(typeof(Dictionary<,>.KeyCollection), false)]
        [InlineData(typeof(Dictionary<,>.ValueCollection), false)]
        [InlineData(typeof(SortedDictionary<,>.KeyCollection), false)]
        [InlineData(typeof(SortedDictionary<,>.ValueCollection), false)]
        [InlineData(typeof(Dictionary<string, int>), true)]
        [InlineData(typeof(IEnumerable<string>), true)]
        [InlineData(typeof(ICollection<string>), true)]
        [InlineData(typeof(Collection<string>), true)]
        [InlineData(typeof(IList<string>), true)]
        [InlineData(typeof(List<string>), true)]
        [InlineData(typeof(LinkedList<string>), true)]
        [InlineData(typeof(HashSet<string>), true)]
        [InlineData(typeof(InheritedGenericCollection), true)]
        public void CreateValidatesWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);

            var sut = new EnumerableTypeCreator();

            Action action = () => sut.Create(executeStrategy, type);

            if (supported)
            {
                action.Should().NotThrow();
            }
            else
            {
                action.Should().Throw<NotSupportedException>();
            }
        }

        [Fact]
        public void PopulateAddsItemsToCollectionFromExecuteStrategy()
        {
            var expected = new Collection<Guid>();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);

            executeStrategy.Create(typeof(Guid)).Returns(Guid.NewGuid());

            var sut = new EnumerableTypeCreator
            {
                AutoPopulateCount = 15
            };

            var actual = sut.Populate(executeStrategy, expected);

            actual.Should().BeSameAs(expected);

            var set = (Collection<Guid>) actual;

            set.Should().HaveCount(sut.AutoPopulateCount);
            set.All(x => x != Guid.Empty).Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(Dictionary<string, int>))]
        [InlineData(typeof(IEnumerable<string>))]
        [InlineData(typeof(ICollection<string>))]
        [InlineData(typeof(Collection<string>))]
        [InlineData(typeof(IList<string>))]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(LinkedList<string>))]
        [InlineData(typeof(InheritedGenericCollection))]
        public void PopulateAddsItemsToInstancesTest(Type type)
        {
            var configuration = Model.UsingDefaultConfiguration();

            var sut = new EnumerableTypeCreator();

            var executeStrategy = new DefaultExecuteStrategy();

            executeStrategy.Initialize(configuration);

            var actual = sut.Create(executeStrategy, type);

            sut.Populate(executeStrategy, actual);

            var converted = (IEnumerable) actual;

            converted.Should().NotBeEmpty();
        }

        [Fact]
        public void PopulateAddsItemsToListFromExecuteStrategy()
        {
            var expected = new List<Guid>();

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var configuration = Substitute.For<IBuildConfiguration>();
            var typeResolver = Substitute.For<ITypeResolver>();

            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            configuration.TypeResolver.Returns(typeResolver);
            executeStrategy.Configuration.Returns(configuration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Create(typeof(Guid)).Returns(Guid.NewGuid());

            var sut = new EnumerableTypeCreator
            {
                AutoPopulateCount = 15
            };

            var actual = sut.Populate(executeStrategy, expected);

            actual.Should().BeSameAs(expected);

            var set = (List<Guid>) actual;

            set.Should().HaveCount(sut.AutoPopulateCount);
            set.All(x => x != Guid.Empty).Should().BeTrue();
        }

        [Fact]
        public void PopulateCanAddItemsBasedOnPreviousItem()
        {
            var actual = new List<int>();
            var executeStrategy = Model.UsingDefaultConfiguration()
                .UsingExecuteStrategy<DefaultExecuteStrategy<List<int>>>();

            var sut = new IncrementingEnumerableTypeCreator();

            var result = (List<int>) sut.Populate(executeStrategy, actual);

            var baseValue = result[0];
            var expected = new List<int>(sut.AutoPopulateCount);

            for (var index = 0; index < sut.AutoPopulateCount; index++)
            {
                expected.Add(baseValue + index);
            }

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void PopulateThrowsExceptionWithUnsupportedType()
        {
            var instance = new Lazy<bool>(() => true);

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var configuration = Substitute.For<IBuildConfiguration>();
            var typeResolver = Substitute.For<ITypeResolver>();

            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            configuration.TypeResolver.Returns(typeResolver);
            executeStrategy.Configuration.Returns(configuration);
            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new EnumerableTypeCreator();

            Action action = () => sut.Populate(executeStrategy, instance);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void PriorityReturnsHigherThanDefaultTypeCreator()
        {
            var sut = new EnumerableTypeCreator();
            var other = new DefaultTypeCreator();

            sut.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Fact]
        public void SettingAutoPopulateCountShouldNotChangeDefaultAutoPopulateCount()
        {
            var sut = new EnumerableTypeCreator
            {
                AutoPopulateCount = Environment.TickCount
            };

            EnumerableTypeCreator.DefaultAutoPopulateCount.Should().NotBe(sut.AutoPopulateCount);
        }

        [Fact]
        public void SettingDefaultAutoPopulateCountOnlyAffectsNewInstances()
        {
            var expected = EnumerableTypeCreator.DefaultAutoPopulateCount;

            try
            {
                var first = new EnumerableTypeCreator();

                EnumerableTypeCreator.DefaultAutoPopulateCount = 11;

                var second = new EnumerableTypeCreator();

                first.AutoPopulateCount.Should().Be(expected);
                second.AutoPopulateCount.Should().Be(11);
            }
            finally
            {
                EnumerableTypeCreator.DefaultAutoPopulateCount = expected;
            }
        }

        private class EnumerableTypeCreatorWrapper : EnumerableTypeCreator
        {
            public void CreateItem(Type type, IExecuteStrategy executeStrategy, object item)
            {
                CreateChildItem(type, executeStrategy, item);
            }
        }
    }
}
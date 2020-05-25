namespace ModelBuilder.UnitTests.Scenarios
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class EnumerableTests
    {
        [Theory]
        [InlineData(typeof(IEnumerable<string>))]
        [InlineData(typeof(ICollection<string>))]
        [InlineData(typeof(Collection<string>))]
        [InlineData(typeof(IList<string>))]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(LinkedList<string>))]
        [InlineData(typeof(InheritedGenericCollection))]
        [InlineData(typeof(HashSet<string>))]
        [InlineData(typeof(IList<KeyValuePair<string, Guid>>))]
        [InlineData(typeof(List<KeyValuePair<string, Guid>>))]
        [InlineData(typeof(List<MultipleGenericArguments<string, Guid>>))]
        [InlineData(typeof(IList<MultipleGenericArguments<string, Guid>>))]
        [InlineData(typeof(Collection<KeyValuePair<string, Guid>>))]
        [InlineData(typeof(ICollection<KeyValuePair<string, Guid>>))]
        [InlineData(typeof(IReadOnlyCollection<int>))]
        [InlineData(typeof(IReadOnlyList<int>))]
        [InlineData(typeof(IDictionary<string, Person>))]
        [InlineData(typeof(Dictionary<string, Person>))]
        public void CanCreateEnumerableTypes(Type type)
        {
            var actual = Model.Create(type);

            actual.As<IEnumerable>().Should().NotBeEmpty();
        }

        [Fact]
        public void CanCreateInstanceWithEnumerableProperty()
        {
            var actual = Model.Create<Company>()!;

            actual.Address.Should().NotBeNullOrWhiteSpace();
            actual.Name.Should().NotBeNullOrWhiteSpace();
            actual.Staff.Should().NotBeEmpty();
        }

        [Fact]
        public void CanCreateInstanceWithEnumerablePropertyTypes()
        {
            var actual = Model.Create<EnumerableParent>()!;

            actual.Collection.Should().NotBeEmpty();
            actual.Enumerable.Should().NotBeEmpty();
            actual.InterfaceCollection.Should().NotBeEmpty();
            actual.InterfaceList.Should().NotBeEmpty();
            actual.List.Should().NotBeEmpty();
        }

        [Fact]
        public void CanGenerateArrayOfCustomType()
        {
            var actual = Model.Create<Person[]>()!;

            actual.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void CanGenerateByteArray()
        {
            var actual = Model.Create<byte[]>()!;

            actual.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void CanUpdateBuildConfigurationDefaults()
        {
            var actual = Model.UsingDefaultConfiguration()
                .UpdateTypeCreator<EnumerableTypeCreator>(x => x.MinCount = x.MaxCount = 10)
                .Create<ICollection<Person>>()!;

            actual.Count.Should().BeGreaterOrEqualTo(10);
            actual.Count.Should().BeLessOrEqualTo(30);
            actual.All(x => x != null!).Should().BeTrue();
        }

        [Fact]
        public void CreateReadOnlyCollectionWithItemCountBetweenMinAndMax()
        {
            var actual = Model.Create<ReadOnlyCollection<int>>()!;

            actual.Count.Should().BeGreaterOrEqualTo(10);
            actual.Count.Should().BeLessOrEqualTo(30);
            actual.All(x => x == 0).Should().BeFalse();
        }

        [Fact]
        public void CreateReturnsCollectionWithItemCountBetweenMinAndMax()
        {
            var actual = Model.Create<ICollection<int>>()!;

            actual.Count.Should().BeGreaterOrEqualTo(10);
            actual.Count.Should().BeLessOrEqualTo(30);
            actual.All(x => x == 0).Should().BeFalse();
        }

        [Fact]
        public void CreateReturnsCompanyWithEnumerableTypeCreatorUsage()
        {
            var actual = Model.Create<Company>()!;

            actual.Should().NotBeNull();
            actual.Name.Should().NotBeNullOrWhiteSpace();
            actual.Staff.Should().NotBeEmpty();
        }

        [Fact]
        public void CreateReturnsEnumerableWithItemCountBetweenMinAndMax()
        {
            var actual = Model.Create<IEnumerable<int>>().ToList();

            actual.Count.Should().BeGreaterOrEqualTo(10);
            actual.Count.Should().BeLessOrEqualTo(30);
            actual.All(x => x == 0).Should().BeFalse();
        }

        [Fact]
        public void CreateReturnsListWithItemCountBetweenMinAndMax()
        {
            var actual = Model.Create<IList<int>>()!;

            actual.Count.Should().BeGreaterOrEqualTo(10);
            actual.Count.Should().BeLessOrEqualTo(30);
            actual.All(x => x == 0).Should().BeFalse();
        }
    }
}
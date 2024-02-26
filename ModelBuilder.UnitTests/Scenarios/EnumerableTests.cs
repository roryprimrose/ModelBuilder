namespace ModelBuilder.UnitTests.Scenarios
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using Xunit;
    using Xunit.Abstractions;

    public class EnumerableTests
    {
        private readonly ITestOutputHelper _output;

        public EnumerableTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(typeof(IEnumerable<string>))]
        [InlineData(typeof(ICollection<string>))]
        [InlineData(typeof(Collection<string>))]
        [InlineData(typeof(IList<string>))]
        [InlineData(typeof(List<string>))]
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
        [InlineData(typeof(SortedDictionary<string, Person>))]
        [InlineData(typeof(SortedList<string, Person>))]
        [InlineData(typeof(IDictionary))]
        [InlineData(typeof(Hashtable))]
        [InlineData(typeof(SortedList))]
        [InlineData(typeof(IOrderedDictionary))]
        [InlineData(typeof(OrderedDictionary))]
        [InlineData(typeof(ListDictionary))]
        public void CanCreateEnumerableTypes(Type type)
        {
            var actual = Model.WriteLog(_output.WriteLine).Create(type);

            var enumerable = actual.As<IEnumerable>();
            var entryFound = false;

            foreach (var value in enumerable)
            {
                entryFound = true;
                break;
            }

            entryFound.Should().BeTrue();
        }

        [Fact]
        public void CanCreateInstanceWithEnumerableProperty()
        {
            var actual = Model.WriteLog<Company>(_output.WriteLine).Create();

            actual.Address.Should().NotBeNullOrWhiteSpace();
            actual.Name.Should().NotBeNullOrWhiteSpace();
            actual.Staff.Should().NotBeEmpty();
        }

        [Fact]
        public void CanCreateInstanceWithEnumerablePropertyTypes()
        {
            var actual = Model.WriteLog<EnumerableParent>(_output.WriteLine).Create();

            actual.Collection.Should().NotBeEmpty();
            actual.Enumerable.Should().NotBeEmpty();
            actual.InterfaceCollection.Should().NotBeEmpty();
            actual.InterfaceList.Should().NotBeEmpty();
            actual.List.Should().NotBeEmpty();
        }

        [Fact]
        public void CanGenerateArrayOfCustomType()
        {
            var actual = Model.WriteLog<Person[]>(_output.WriteLine).Create();

            actual.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void CanGenerateByteArray()
        {
            var actual = Model.WriteLog<byte[]>(_output.WriteLine).Create();

            actual.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void CanUpdateBuildConfigurationDefaults()
        {
            var actual = Model.UsingDefaultConfiguration()
                .UpdateTypeCreator<EnumerableTypeCreator>(x => x.MinCount = x.MaxCount = 10)
                .WriteLog<ICollection<Person>>(_output.WriteLine)
                .Create();

            actual.Count.Should().BeGreaterOrEqualTo(10);
            actual.Count.Should().BeLessOrEqualTo(30);
            actual.All(x => x != null!).Should().BeTrue();
        }

        [Fact]
        public void CreateReadOnlyCollectionWithItemCountBetweenMinAndMax()
        {
            var actual = Model.WriteLog<ReadOnlyCollection<int>>(_output.WriteLine).Create();

            actual.Count.Should().BeGreaterOrEqualTo(10);
            actual.Count.Should().BeLessOrEqualTo(30);
            actual.All(x => x == 0).Should().BeFalse();
        }

        [Fact]
        public void CreateReturnsCollectionWithItemCountBetweenMinAndMax()
        {
            var actual = Model.WriteLog<ICollection<int>>(_output.WriteLine).Create();

            actual.Count.Should().BeGreaterOrEqualTo(10);
            actual.Count.Should().BeLessOrEqualTo(30);
            actual.All(x => x == 0).Should().BeFalse();
        }

        [Fact]
        public void CreateReturnsCompanyWithEnumerableTypeCreatorUsage()
        {
            var actual = Model.WriteLog<Company>(_output.WriteLine).Create();

            actual.Should().NotBeNull();
            actual.Name.Should().NotBeNullOrWhiteSpace();
            actual.Staff.Should().NotBeEmpty();
        }

        [Fact]
        public void CreateReturnsEnumerableWithItemCountBetweenMinAndMax()
        {
            var actual = Model.WriteLog<IEnumerable<int>>(_output.WriteLine).Create().ToList();

            actual.Count.Should().BeGreaterOrEqualTo(10);
            actual.Count.Should().BeLessOrEqualTo(30);
            actual.All(x => x == 0).Should().BeFalse();
        }

        [Fact]
        public void CreateReturnsListWithItemCountBetweenMinAndMax()
        {
            var actual = Model.WriteLog<IList<int>>(_output.WriteLine).Create();

            actual.Count.Should().BeGreaterOrEqualTo(10);
            actual.Count.Should().BeLessOrEqualTo(30);
            actual.All(x => x == 0).Should().BeFalse();
        }
    }
}
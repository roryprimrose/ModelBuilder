namespace ModelBuilder.IntegrationTests
{
    using System.Collections.Generic;
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end tests of the custom-collection-type support: a type inheriting a mutable BCL
    ///     collection base, a type inheriting a keyed BCL collection base, and a hand-written
    ///     <see cref="System.Collections.Generic.IList{T}" /> implementer.
    /// </summary>
    public class CustomCollectionTests
    {
        [Fact]
        public void CreateBuildsCustomTypeInheritingCollectionWithItems()
        {
            var actual = Model.SetOptions(x =>
                {
                    x.MinCount = 2;
                    x.MaxCount = 2;
                })
                .Create<WidgetBag>();

            actual.Should().HaveCount(2);
            actual.Should().OnlyContain(w => w.Name != string.Empty);
        }

        [Fact]
        public void CreateBuildsCustomTypeInheritingDictionaryWithItems()
        {
            var actual = Model.SetOptions(x =>
                {
                    x.MinCount = 2;
                    x.MaxCount = 2;
                })
                .Create<WidgetMap>();

            actual.Should().HaveCount(2);
            actual.Values.Should().OnlyContain(w => w.Name != string.Empty);
        }

        [Fact]
        public void CreateBuildsCustomIListImplementerWithItems()
        {
            var actual = Model.SetOptions(x =>
                {
                    x.MinCount = 2;
                    x.MaxCount = 2;
                })
                .Create<CustomWidgetList>();

            actual.Should().HaveCount(2);
            actual.Should().OnlyContain(w => w.Name != string.Empty);
        }

        [Fact]
        public void CreateBuildsListFromIListInterfaceRoot()
        {
            var actual = Model.SetOptions(x =>
                {
                    x.MinCount = 2;
                    x.MaxCount = 2;
                })
                .Create<IList<string>>();

            actual.Should().BeOfType<List<string>>();
            actual.Should().HaveCount(2);
            actual.Should().OnlyContain(s => !string.IsNullOrEmpty(s));
        }

        [Fact]
        public void CreateBuildsDictionaryFromIDictionaryInterfaceRoot()
        {
            var actual = Model.SetOptions(x =>
                {
                    x.MinCount = 2;
                    x.MaxCount = 2;
                })
                .Create<IDictionary<string, int>>();

            actual.Should().BeOfType<Dictionary<string, int>>();
            actual.Should().HaveCount(2);
        }

        [Fact]
        public void CreateBuildsHashSetFromISetInterfaceRoot()
        {
            var actual = Model.SetOptions(x =>
                {
                    x.MinCount = 2;
                    x.MaxCount = 2;
                })
                .Create<ISet<int>>();

            actual.Should().BeOfType<HashSet<int>>();
            actual.Should().HaveCountGreaterThanOrEqualTo(2);
        }

        [Fact]
        public void CreateBuildsListFromIReadOnlyListInterfaceRoot()
        {
            var actual = Model.SetOptions(x =>
                {
                    x.MinCount = 2;
                    x.MaxCount = 2;
                })
                .Create<IReadOnlyList<int>>();

            actual.Should().BeOfType<List<int>>();
            actual.Should().HaveCount(2);
        }
    }
}

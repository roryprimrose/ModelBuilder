namespace ModelBuilder.IntegrationTests
{
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end tests that every recognized BCL collection kind populates both its shape (Count)
    ///     and a fully built element/key/value graph, using <c>SetOptions</c> to force a known size.
    /// </summary>
    public class CollectionTests
    {
        [Fact]
        public void CreatePopulatesEveryCollectionKindWithFullyBuiltElements()
        {
            var actual = Model.SetOptions(x =>
                {
                    x.MinCount = 3;
                    x.MaxCount = 3;
                })
                .Create<CollectionsProfile>();

            actual.ArrayItems.Should().HaveCount(3);
            actual.ArrayItems.Should().OnlyContain(w => w.Name != string.Empty);

            actual.ListItems.Should().HaveCount(3);
            actual.SetItems.Should().HaveCount(3);
            actual.CollectionItems.Should().HaveCount(3);
            actual.QueueItems.Should().HaveCount(3);
            actual.StackItems.Should().HaveCount(3);
            actual.SortedSetItems.Should().HaveCount(3);
            actual.ObservableItems.Should().HaveCount(3);
            actual.ConcurrentBagItems.Should().HaveCount(3);
            actual.ConcurrentQueueItems.Should().HaveCount(3);
            actual.ConcurrentStackItems.Should().HaveCount(3);
            actual.DictionaryItems.Should().HaveCount(3);
            actual.SortedDictionaryItems.Should().HaveCount(3);
            actual.SortedListItems.Should().HaveCount(3);
            actual.ConcurrentDictionaryItems.Should().HaveCount(3);
            actual.ReadOnlyCollectionItems.Should().HaveCount(3);
            actual.ReadOnlyDictionaryItems.Should().HaveCount(3);

            // Spot-check that elements are fully built graphs (non-default Widget), not empty shells.
            actual.ListItems.Should().OnlyContain(w => w.Name != string.Empty && w.Price != 0);
            actual.DictionaryItems.Values.Should().OnlyContain(w => w.Name != string.Empty);
        }
    }
}

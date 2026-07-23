namespace ModelBuilder.IntegrationTests.Models
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     Exposes one settable member per collection kind the generator recognizes, using
    ///     <see cref="Widget" /> (a non-trivial element type) so both the collection shape and the item
    ///     graph population can be verified in one build.
    /// </summary>
    public class CollectionsProfile
    {
        public Widget[] ArrayItems { get; set; } = System.Array.Empty<Widget>();

        public List<Widget> ListItems { get; set; } = new();

        public HashSet<Widget> SetItems { get; set; } = new();

        public Collection<Widget> CollectionItems { get; set; } = new();

        public Queue<Widget> QueueItems { get; set; } = new();

        public Stack<Widget> StackItems { get; set; } = new();

        public SortedSet<int> SortedSetItems { get; set; } = new();

        public ObservableCollection<Widget> ObservableItems { get; set; } = new();

        public ConcurrentBag<Widget> ConcurrentBagItems { get; set; } = new();

        public ConcurrentQueue<Widget> ConcurrentQueueItems { get; set; } = new();

        public ConcurrentStack<Widget> ConcurrentStackItems { get; set; } = new();

        public Dictionary<string, Widget> DictionaryItems { get; set; } = new();

        public SortedDictionary<string, Widget> SortedDictionaryItems { get; set; } = new();

        public SortedList<string, Widget> SortedListItems { get; set; } = new();

        public ConcurrentDictionary<string, Widget> ConcurrentDictionaryItems { get; set; } = new();

        public ReadOnlyCollection<Widget> ReadOnlyCollectionItems { get; set; } = new(new List<Widget>());

        public ReadOnlyDictionary<string, Widget> ReadOnlyDictionaryItems { get; set; } =
            new(new Dictionary<string, Widget>());
    }
}

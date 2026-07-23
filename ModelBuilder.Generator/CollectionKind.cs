namespace ModelBuilder.Generator
{
    /// <summary>
    ///     The <see cref="CollectionKind" /> enum
    ///     identifies how a discovered collection target is materialized.
    /// </summary>
    internal enum CollectionKind
    {
        Array = 0,
        List,
        Set,
        Dictionary,

        /// <summary>System.Collections.ObjectModel.Collection&lt;T&gt;.</summary>
        Collection,

        /// <summary>System.Collections.Generic.Queue&lt;T&gt;.</summary>
        Queue,

        /// <summary>System.Collections.Generic.Stack&lt;T&gt;.</summary>
        Stack,

        /// <summary>System.Collections.Generic.SortedSet&lt;T&gt;.</summary>
        SortedSet,

        /// <summary>System.Collections.ObjectModel.ObservableCollection&lt;T&gt;.</summary>
        ObservableCollection,

        /// <summary>System.Collections.Generic.SortedDictionary&lt;TKey, TValue&gt;.</summary>
        SortedDictionary,

        /// <summary>System.Collections.Generic.SortedList&lt;TKey, TValue&gt;.</summary>
        SortedList,

        /// <summary>System.Collections.Concurrent.ConcurrentDictionary&lt;TKey, TValue&gt;.</summary>
        ConcurrentDictionary,

        /// <summary>System.Collections.Concurrent.ConcurrentBag&lt;T&gt;.</summary>
        ConcurrentBag,

        /// <summary>System.Collections.Concurrent.ConcurrentQueue&lt;T&gt;.</summary>
        ConcurrentQueue,

        /// <summary>System.Collections.Concurrent.ConcurrentStack&lt;T&gt;.</summary>
        ConcurrentStack,

        /// <summary>System.Collections.ObjectModel.ReadOnlyCollection&lt;T&gt;, backed by a built List&lt;T&gt;.</summary>
        ReadOnlyCollection,

        /// <summary>System.Collections.ObjectModel.ReadOnlyDictionary&lt;TKey, TValue&gt;, backed by a built Dictionary&lt;TKey, TValue&gt;.</summary>
        ReadOnlyDictionary,

        /// <summary>System.Collections.ObjectModel.ReadOnlyObservableCollection&lt;T&gt;, backed by a built ObservableCollection&lt;T&gt;.</summary>
        ReadOnlyObservableCollection,

        /// <summary>System.Collections.Immutable.ImmutableArray&lt;T&gt;.</summary>
        ImmutableArray,

        /// <summary>System.Collections.Immutable.ImmutableList&lt;T&gt;.</summary>
        ImmutableList,

        /// <summary>System.Collections.Immutable.ImmutableHashSet&lt;T&gt;.</summary>
        ImmutableHashSet,

        /// <summary>System.Collections.Immutable.ImmutableSortedSet&lt;T&gt;.</summary>
        ImmutableSortedSet,

        /// <summary>System.Collections.Immutable.ImmutableDictionary&lt;TKey, TValue&gt;.</summary>
        ImmutableDictionary,

        /// <summary>System.Collections.Immutable.ImmutableSortedDictionary&lt;TKey, TValue&gt;.</summary>
        ImmutableSortedDictionary,

        /// <summary>System.Collections.Immutable.ImmutableQueue&lt;T&gt;.</summary>
        ImmutableQueue,

        /// <summary>System.Collections.Immutable.ImmutableStack&lt;T&gt;.</summary>
        ImmutableStack
    }
}

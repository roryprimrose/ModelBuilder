namespace ModelBuilder.IntegrationTests.Models
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     A custom type that inherits a mutable BCL collection base, used to verify the generator
    ///     builds the requested type itself (not a substitute <see cref="Collection{T}" />) end to end.
    /// </summary>
    public class WidgetBag : Collection<Widget>
    {
        public string BagLabel { get; set; } = string.Empty;
    }

    /// <summary>
    ///     A custom type that inherits <see cref="Dictionary{TKey,TValue}" />, used to verify keyed
    ///     custom-collection construction end to end.
    /// </summary>
    public class WidgetMap : Dictionary<string, Widget>
    {
    }

    /// <summary>
    ///     A hand-written <see cref="IList{T}" /> implementer (does not inherit any BCL collection base)
    ///     used to verify the interface-implementation classification path end to end.
    /// </summary>
    public class CustomWidgetList : IList<Widget>
    {
        private readonly List<Widget> _items = new();

        public Widget this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public int Count => _items.Count;

        public bool IsReadOnly => false;

        public void Add(Widget item)
        {
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(Widget item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(Widget[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Widget> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public int IndexOf(Widget item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, Widget item)
        {
            _items.Insert(index, item);
        }

        public bool Remove(Widget item)
        {
            return _items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

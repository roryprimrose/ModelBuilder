namespace ModelBuilder.UnitTests.Models
{
    using System.Collections;
    using System.Collections.Generic;

    public class CustomCollection<T> : ICustomCollection<T> where T : class
    {
        private readonly List<T> _store = new List<T>();

        public IEnumerator<T> GetEnumerator()
        {
            return _store.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
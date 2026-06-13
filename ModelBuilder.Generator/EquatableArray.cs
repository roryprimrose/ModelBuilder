namespace ModelBuilder.Generator
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    /// <summary>
    ///     The <see cref="EquatableArray{T}" /> struct
    ///     wraps an <see cref="ImmutableArray{T}" /> with value-based sequence equality so it can be used
    ///     as incremental generator pipeline data without defeating caching.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    internal readonly struct EquatableArray<T> : IEquatable<EquatableArray<T>>, IReadOnlyList<T>
        where T : IEquatable<T>
    {
        private readonly ImmutableArray<T> _array;

        public EquatableArray(ImmutableArray<T> array)
        {
            _array = array;
        }

        public bool Equals(EquatableArray<T> other)
        {
            if (_array.IsDefault)
            {
                return other._array.IsDefault;
            }

            if (other._array.IsDefault)
            {
                return false;
            }

            return _array.SequenceEqual(other._array);
        }

        public override bool Equals(object? obj)
        {
            return obj is EquatableArray<T> other && Equals(other);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)(_array.IsDefault ? ImmutableArray<T>.Empty : _array)).GetEnumerator();
        }

        public override int GetHashCode()
        {
            if (_array.IsDefault)
            {
                return 0;
            }

            var hash = 17;

            foreach (var item in _array)
            {
                hash = hash * 31 + (item?.GetHashCode() ?? 0);
            }

            return hash;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _array.IsDefault ? 0 : _array.Length;

        public T this[int index] => _array[index];
    }
}

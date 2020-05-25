namespace ModelBuilder
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     The <see cref="BuildHistory" />
    ///     class is used to track a hierarchy of objects being created.
    /// </summary>
    [SuppressMessage(
        "Code.Quality",
        "CA1710",
        Justification = "The history is enumerable, but does not have the characteristics of a Collection.")]
    public class BuildHistory : IBuildHistory
    {
        private readonly Stack<object> _buildHistory = new Stack<object>();

        /// <inheritdoc />
        public IEnumerator GetEnumerator()
        {
            return _buildHistory.GetEnumerator();
        }

        /// <inheritdoc />
        public void Pop()
        {
            var instance = _buildHistory.Pop();

            if (First == instance)
            {
                First = null;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is <c>null</c>.</exception>
        public void Push(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            _buildHistory.Push(instance);

            if (First == null)
            {
                First = instance;
            }
        }

        /// <inheritdoc />
        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return _buildHistory.GetEnumerator();
        }

        /// <inheritdoc />
        public int Count => _buildHistory.Count;

        /// <inheritdoc />
        public object? First { get; private set; }

        /// <inheritdoc />
        public object? Last
        {
            get
            {
                if (_buildHistory.Count == 0)
                {
                    return null;
                }

                return _buildHistory.Peek();
            }
        }
    }
}
namespace ModelBuilder
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ModelBuilder.BuildActions;

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
        private readonly Stack<BuildHistoryItem> _buildHistory = new Stack<BuildHistoryItem>();

        /// <inheritdoc />
        public void AddCapability(Type type, IBuildCapability capability)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            capability = capability ?? throw new ArgumentNullException(nameof(capability));

            if (_buildHistory.Count == 0)
            {
                return;
            }

            var historyItem = _buildHistory.Peek();

            historyItem.Capabilities[type] = capability;
        }

        /// <inheritdoc />
        public IBuildCapability? GetCapability(Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            if (_buildHistory.Count == 0)
            {
                return null;
            }

            var historyItem = _buildHistory.Peek();

            if (historyItem.Capabilities.ContainsKey(type))
            {
                return historyItem.Capabilities[type];
            }

            return null;
        }

        /// <inheritdoc />
        public IEnumerator GetEnumerator()
        {
            return _buildHistory.Select(x => x.Value).GetEnumerator();
        }

        /// <inheritdoc />
        public void Pop()
        {
            var historyItem = _buildHistory.Pop();

            if (First == historyItem.Value)
            {
                First = null;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is <c>null</c>.</exception>
        public void Push(object instance)
        {
            instance = instance ?? throw new ArgumentNullException(nameof(instance));

            var historyItem = new BuildHistoryItem(instance);

            _buildHistory.Push(historyItem);

            if (First == null)
            {
                First = instance;
            }
        }

        /// <inheritdoc />
        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return _buildHistory.Select(x => x.Value).GetEnumerator();
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

                return _buildHistory.Peek().Value;
            }
        }
    }
}
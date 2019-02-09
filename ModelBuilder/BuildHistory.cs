namespace ModelBuilder
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class BuildHistory : IBuildHistory
    {
        private readonly Stack<object> _buildHistory = new Stack<object>();

        public IEnumerator GetEnumerator()
        {
            return _buildHistory.GetEnumerator();
        }

        public void Pop()
        {
            var instance = _buildHistory.Pop();

            if (First == instance)
            {
                First = null;
            }
        }

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

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return _buildHistory.GetEnumerator();
        }

        public int Count => _buildHistory.Count;

        public object First { get; private set; }

        public object Last
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
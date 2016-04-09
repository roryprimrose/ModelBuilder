using System.Collections;
using System.Collections.Generic;

namespace ModelBuilder.UnitTests
{
    public class NumericTypeDataSource : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new List<object[]>
        {
            new object[] {typeof (sbyte), true},
            new object[] {typeof (byte), true},
            new object[] {typeof (short), true},
            new object[] {typeof (ushort), true},
            new object[] {typeof (int), true},
            new object[] {typeof (uint), true},
            new object[] {typeof (long), true},
            new object[] {typeof (ulong), true},
            new object[] {typeof (double), true},
            new object[] {typeof (float), true},
            new object[] {typeof (string), false}
        };

        public IEnumerator<object[]> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
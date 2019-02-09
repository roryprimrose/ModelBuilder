namespace ModelBuilder.UnitTests
{
    using System.Collections;
    using System.Collections.Generic;

    public class NumericTypeDataSource : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new List<object[]>
        {
            new object[] {typeof(sbyte), true, sbyte.MinValue, sbyte.MaxValue},
            new object[] {typeof(byte), true, byte.MinValue, byte.MaxValue},
            new object[] {typeof(short), true, short.MinValue, short.MaxValue},
            new object[] {typeof(ushort), true, ushort.MinValue, ushort.MaxValue},
            new object[] {typeof(int), true, int.MinValue, int.MaxValue},
            new object[] {typeof(uint), true, uint.MinValue, uint.MaxValue},
            new object[] {typeof(long), true, long.MinValue, long.MaxValue},
            new object[] {typeof(ulong), true, ulong.MinValue, ulong.MaxValue},
            new object[] {typeof(double), true, double.MinValue, double.MaxValue},
            new object[] {typeof(float), true, float.MinValue, float.MaxValue},
            new object[] {typeof(sbyte?), true, sbyte.MinValue, sbyte.MaxValue},
            new object[] {typeof(byte?), true, byte.MinValue, byte.MaxValue},
            new object[] {typeof(short?), true, short.MinValue, short.MaxValue},
            new object[] {typeof(ushort?), true, ushort.MinValue, ushort.MaxValue},
            new object[] {typeof(int?), true, int.MinValue, int.MaxValue},
            new object[] {typeof(uint?), true, uint.MinValue, uint.MaxValue},
            new object[] {typeof(long?), true, long.MinValue, long.MaxValue},
            new object[] {typeof(ulong?), true, ulong.MinValue, ulong.MaxValue},
            new object[] {typeof(double?), true, double.MinValue, double.MaxValue},
            new object[] {typeof(float?), true, float.MinValue, float.MaxValue},
            new object[] {typeof(decimal), true, decimal.MinValue, decimal.MaxValue},
            new object[] {typeof(string), false, 0, 0}
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
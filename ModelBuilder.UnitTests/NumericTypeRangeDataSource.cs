﻿namespace ModelBuilder.UnitTests
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Code.Quality",
        "CA1710",
        Justification = "The data source not have the characteristics of a Collection.")]
    public class NumericTypeRangeDataSource : IEnumerable<object[]>
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

            // TODO: Wait until xUnit fixes the bug at https://github.com/xunit/xunit/issues/1771 before adding this back in again
            //new object[] {typeof(decimal), true, decimal.MinValue, decimal.MaxValue},

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
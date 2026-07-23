namespace ModelBuilder.IntegrationTests.Models
{
    /// <summary>
    ///     A class exposing every numeric primitive plus <see cref="char" /> and <see cref="bool" />, so
    ///     a single build can assert every numeric kind populates without overflow or type-conversion
    ///     failures.
    /// </summary>
    public class NumericBoundaries
    {
        public sbyte SByteValue { get; set; }

        public byte ByteValue { get; set; }

        public short ShortValue { get; set; }

        public ushort UShortValue { get; set; }

        public int IntValue { get; set; }

        public uint UIntValue { get; set; }

        public long LongValue { get; set; }

        public ulong ULongValue { get; set; }

        public float FloatValue { get; set; }

        public double DoubleValue { get; set; }

        public decimal DecimalValue { get; set; }

        public char CharValue { get; set; }

        public bool BoolValue { get; set; }
    }
}

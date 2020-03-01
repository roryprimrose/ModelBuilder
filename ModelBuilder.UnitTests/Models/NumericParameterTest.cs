namespace ModelBuilder.UnitTests.Models
{
    public class NumericParameterTest
    {
        public NumericParameterTest(
            sbyte param_sbyte,
            byte param_byte,
            short param_short,
            ushort param_ushort,
            int param_int,
            uint param_uint,
            long param_long,
            ulong param_ulong,
            double param_double,
            float param_float,
            decimal param_decimal,
            sbyte? paramNullable_sbyte,
            byte? paramNullable_byte,
            short? paramNullable_short,
            ushort? paramNullable_ushort,
            int? paramNullable_int,
            uint? paramNullable_uint,
            long? paramNullable_long,
            ulong? paramNullable_ulong,
            double? paramNullable_double,
            float? paramNullable_float,
            decimal? paramNullable_decimal
        )
        {
            Param_sbyte = param_sbyte;
            Param_byte = param_byte;
            Param_short = param_short;
            Param_ushort = param_ushort;
            Param_int = param_int;
            Param_uint = param_uint;
            Param_long = param_long;
            Param_ulong = param_ulong;
            Param_double = param_double;
            Param_float = param_float;
            Param_decimal = param_decimal;
            ParamNullable_sbyte = paramNullable_sbyte;
            ParamNullable_byte = paramNullable_byte;
            ParamNullable_short = paramNullable_short;
            ParamNullable_ushort = paramNullable_ushort;
            ParamNullable_int = paramNullable_int;
            ParamNullable_uint = paramNullable_uint;
            ParamNullable_long = paramNullable_long;
            ParamNullable_ulong = paramNullable_ulong;
            ParamNullable_double = paramNullable_double;
            ParamNullable_float = paramNullable_float;
            ParamNullable_decimal = paramNullable_decimal;
        }

        public byte Param_byte { get; }
        public decimal Param_decimal { get; }
        public double Param_double { get; }
        public float Param_float { get; }
        public int Param_int { get; }
        public long Param_long { get; }

        public sbyte Param_sbyte { get; }
        public short Param_short { get; }
        public uint Param_uint { get; }
        public ulong Param_ulong { get; }
        public ushort Param_ushort { get; }
        public byte? ParamNullable_byte { get; }
        public decimal? ParamNullable_decimal { get; }
        public double? ParamNullable_double { get; }
        public float? ParamNullable_float { get; }
        public int? ParamNullable_int { get; }
        public long? ParamNullable_long { get; }
        public sbyte? ParamNullable_sbyte { get; }
        public short? ParamNullable_short { get; }
        public uint? ParamNullable_uint { get; }
        public ulong? ParamNullable_ulong { get; }
        public ushort? ParamNullable_ushort { get; }
    }
}
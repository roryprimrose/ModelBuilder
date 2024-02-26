namespace ModelBuilder.UnitTests.Models
{
    public class NumericParameterModel
    {
        public NumericParameterModel(
            sbyte paramSbyte,
            byte paramByte,
            short paramShort,
            ushort paramUshort,
            int paramInt,
            uint paramUint,
            long paramLong,
            ulong paramUlong,
            double paramDouble,
            float paramFloat,
            decimal paramDecimal,
            sbyte? paramNullableSbyte,
            byte? paramNullableByte,
            short? paramNullableShort,
            ushort? paramNullableUshort,
            int? paramNullableInt,
            uint? paramNullableUint,
            long? paramNullableLong,
            ulong? paramNullableUlong,
            double? paramNullableDouble,
            float? paramNullableFloat,
            decimal? paramNullableDecimal
        )
        {
            ParamSbyte = paramSbyte;
            ParamByte = paramByte;
            ParamShort = paramShort;
            ParamUshort = paramUshort;
            ParamInt = paramInt;
            ParamUint = paramUint;
            ParamLong = paramLong;
            ParamUlong = paramUlong;
            ParamDouble = paramDouble;
            ParamFloat = paramFloat;
            ParamDecimal = paramDecimal;
            ParamNullableSbyte = paramNullableSbyte;
            ParamNullableByte = paramNullableByte;
            ParamNullableShort = paramNullableShort;
            ParamNullableUshort = paramNullableUshort;
            ParamNullableInt = paramNullableInt;
            ParamNullableUint = paramNullableUint;
            ParamNullableLong = paramNullableLong;
            ParamNullableUlong = paramNullableUlong;
            ParamNullableDouble = paramNullableDouble;
            ParamNullableFloat = paramNullableFloat;
            ParamNullableDecimal = paramNullableDecimal;
        }

        public byte ParamByte { get; }
        public decimal ParamDecimal { get; }
        public double ParamDouble { get; }
        public float ParamFloat { get; }
        public int ParamInt { get; }
        public long ParamLong { get; }
        public byte? ParamNullableByte { get; }
        public decimal? ParamNullableDecimal { get; }
        public double? ParamNullableDouble { get; }
        public float? ParamNullableFloat { get; }
        public int? ParamNullableInt { get; }
        public long? ParamNullableLong { get; }
        public sbyte? ParamNullableSbyte { get; }
        public short? ParamNullableShort { get; }
        public uint? ParamNullableUint { get; }
        public ulong? ParamNullableUlong { get; }
        public ushort? ParamNullableUshort { get; }

        public sbyte ParamSbyte { get; }
        public short ParamShort { get; }
        public uint ParamUint { get; }
        public ulong ParamUlong { get; }
        public ushort ParamUshort { get; }
    }
}
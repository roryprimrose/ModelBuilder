using System;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="BooleanValueGenerator"/>
    /// class is used to generate random numeric values.
    /// </summary>
    public class NumericValueGenerator : ValueGeneratorBase
    {
        /// <inheritdoc />
        public override object Generate(Type type, string referenceName, object context)
        {
            VerifyGenerateRequest(type, referenceName, context);

            if (type == typeof (sbyte))
            {
                return Convert.ToSByte(Generator.Next(sbyte.MinValue, sbyte.MaxValue));
            }

            if (type == typeof (byte))
            {
                return Convert.ToByte(Generator.Next(byte.MinValue, byte.MaxValue));
            }

            if (type == typeof (short))
            {
                return Convert.ToInt16(Generator.Next(short.MinValue, short.MaxValue));
            }

            if (type == typeof (ushort))
            {
                return Convert.ToUInt16(Generator.Next(ushort.MinValue, ushort.MaxValue));
            }

            if (type == typeof (uint))
            {
                return Generator.NextUInt32();
            }

            if (type == typeof (long))
            {
                return Generator.NextInt64();
            }

            if (type == typeof (ulong))
            {
                return Generator.NextUInt64();
            }

            if (type == typeof (double))
            {
                return Generator.NextDouble();
            }

            if (type == typeof (float))
            {
                return Generator.NextFloat();
            }

            // Return int by default
            return Generator.Next();
        }

        /// <inheritdoc />
        public override bool IsSupported(Type type, string referenceName, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsValueType == false)
            {
                return false;
            }

            if (type == typeof (sbyte))
            {
                return true;
            }

            if (type == typeof (byte))
            {
                return true;
            }

            if (type == typeof (short))
            {
                return true;
            }

            if (type == typeof (ushort))
            {
                return true;
            }

            if (type == typeof (int))
            {
                return true;
            }

            if (type == typeof (uint))
            {
                return true;
            }

            if (type == typeof (long))
            {
                return true;
            }

            if (type == typeof (ulong))
            {
                return true;
            }

            if (type == typeof (double))
            {
                return true;
            }

            if (type == typeof (float))
            {
                return true;
            }

            return false;
        }
    }
}
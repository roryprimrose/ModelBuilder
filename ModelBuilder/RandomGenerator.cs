using System;
using System.Globalization;
using ModelBuilder.Properties;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="RandomGenerator"/>
    /// class is used to build random numeric values and byte arrays.
    /// </summary>
    public class RandomGenerator : IRandomGenerator
    {
        private static readonly Random _random = new Random();

        /// <inheritdoc />
        public virtual bool IsSupported(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
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

            if (type == typeof (short))
            {
                return true;
            }

            if (type == typeof (ushort))
            {
                return true;
            }

            if (type == typeof (byte))
            {
                return true;
            }

            if (type == typeof (sbyte))
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
        
        /// <inheritdoc />
        public T Next<T>(T min, T max) where T : struct
        {
            return (T) Next(typeof(T), min, max);
        }
        
        /// <inheritdoc />
        public object Next(Type type, object min, object max)
        {
            ValidateRequestedType(type);

            if (min == null)
            {
                throw new ArgumentNullException(nameof(min));
            }

            if (max == null)
            {
                throw new ArgumentNullException(nameof(max));
            }
            
            var minimum = Convert.ToDouble(min);
            var maximum = Convert.ToDouble(max);
            var requiresRounding = RequiresRounding(type);

            var value = NextValue<double>(minimum, maximum, requiresRounding);

            return Convert.ChangeType(value, type);
        }

        /// <inheritdoc />
        public void Next(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            for (var index = 0; index < buffer.Length; index++)
            {
                var next = Next(byte.MinValue, byte.MaxValue);

                buffer[index] = next;
            }
        }

        /// <inheritdoc />
        public double GetMax(Type type)
        {
            ValidateRequestedType(type);

            if (type == typeof (int))
            {
                return int.MaxValue;
            }

            if (type == typeof (uint))
            {
                return uint.MaxValue;
            }

            if (type == typeof (long))
            {
                return long.MaxValue;
            }

            if (type == typeof (ulong))
            {
                return ulong.MaxValue;
            }

            if (type == typeof (short))
            {
                return short.MaxValue;
            }

            if (type == typeof (ushort))
            {
                return ushort.MaxValue;
            }

            if (type == typeof (byte))
            {
                return byte.MaxValue;
            }

            if (type == typeof (sbyte))
            {
                return sbyte.MaxValue;
            }

            if (type == typeof (float))
            {
                return float.MaxValue;
            }

            return double.MaxValue;
        }

        private bool RequiresRounding(Type type)
        {
            ValidateRequestedType(type);
            
            if (type == typeof(float))
            {
                return true;
            }

            if (type == typeof(double))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public double GetMin(Type type)
        {
            ValidateRequestedType(type);

            if (type == typeof (int))
            {
                return int.MinValue;
            }

            if (type == typeof (uint))
            {
                return uint.MinValue;
            }

            if (type == typeof (long))
            {
                return long.MinValue;
            }

            if (type == typeof (ulong))
            {
                return ulong.MinValue;
            }

            if (type == typeof (short))
            {
                return short.MinValue;
            }

            if (type == typeof (ushort))
            {
                return ushort.MinValue;
            }

            if (type == typeof (byte))
            {
                return byte.MinValue;
            }

            if (type == typeof (sbyte))
            {
                return sbyte.MinValue;
            }

            if (type == typeof (float))
            {
                return float.MinValue;
            }

            return double.MinValue;
        }

        private T NextValue<T>(double min, double max, bool roundValue)
        {
            if (min > max)
            {
                throw new ArgumentOutOfRangeException(nameof(min));
            }

            var range = max - min;
            var variance = _random.NextDouble();
            var pointInRange = variance*range;
            double value;

            if (roundValue)
            {
                value = Math.Round(pointInRange);
            }
            else
            {
                value = pointInRange;
            }

            var shiftedPoint = value + min;

            return (T) Convert.ChangeType(shiftedPoint, typeof (T));
        }

        private void ValidateRequestedType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (IsSupported(type) == false)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Resources.Error_TypeNotSupportedFormat,
                    GetType().FullName, type.FullName);

                throw new NotSupportedException(message);
            }
        }
    }
}
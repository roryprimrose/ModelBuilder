namespace ModelBuilder
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using ModelBuilder.Properties;

    /// <summary>
    ///     The <see cref="RandomGenerator" />
    ///     class is used to build random numeric values and byte arrays.
    /// </summary>
    public class RandomGenerator : IRandomGenerator
    {
        private static readonly Random _random = new Random();

        /// <inheritdoc />
        public object GetMax(Type type)
        {
            ValidateRequestedType(type);

            var checkType = type;

            if (type.IsNullable())
            {
                // The type is nullable so we need to validate whether we support the type argument
                checkType = type.GetGenericArguments()[0];
            }

            if (checkType == typeof(int))
            {
                return int.MaxValue;
            }

            if (checkType == typeof(uint))
            {
                return uint.MaxValue;
            }

            if (checkType == typeof(long))
            {
                return long.MaxValue;
            }

            if (checkType == typeof(ulong))
            {
                return ulong.MaxValue;
            }

            if (checkType == typeof(short))
            {
                return short.MaxValue;
            }

            if (checkType == typeof(ushort))
            {
                return ushort.MaxValue;
            }

            if (checkType == typeof(byte))
            {
                return byte.MaxValue;
            }

            if (checkType == typeof(sbyte))
            {
                return sbyte.MaxValue;
            }

            if (checkType == typeof(float))
            {
                return float.MaxValue;
            }

            if (checkType == typeof(decimal))
            {
                return decimal.MaxValue;
            }

            return double.MaxValue;
        }

        /// <inheritdoc />
        public object GetMin(Type type)
        {
            ValidateRequestedType(type);

            var checkType = type;

            if (type.IsNullable())
            {
                // The type is nullable so we need to validate whether we support the type argument
                checkType = type.GetGenericArguments()[0];
            }

            if (checkType == typeof(int))
            {
                return int.MinValue;
            }

            if (checkType == typeof(uint))
            {
                return uint.MinValue;
            }

            if (checkType == typeof(long))
            {
                return long.MinValue;
            }

            if (checkType == typeof(ulong))
            {
                return ulong.MinValue;
            }

            if (checkType == typeof(short))
            {
                return short.MinValue;
            }

            if (checkType == typeof(ushort))
            {
                return ushort.MinValue;
            }

            if (checkType == typeof(byte))
            {
                return byte.MinValue;
            }

            if (checkType == typeof(sbyte))
            {
                return sbyte.MinValue;
            }

            if (checkType == typeof(float))
            {
                return float.MinValue;
            }

            if (checkType == typeof(decimal))
            {
                return decimal.MinValue;
            }

            return double.MinValue;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is null.</exception>
        public virtual bool IsSupported(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var checkType = type;

            if (type.IsNullable())
            {
                // The type is nullable so we need to validate whether we support the type argument
                checkType = type.GetGenericArguments()[0];
            }

            if (checkType == typeof(int))
            {
                return true;
            }

            if (checkType == typeof(uint))
            {
                return true;
            }

            if (checkType == typeof(long))
            {
                return true;
            }

            if (checkType == typeof(ulong))
            {
                return true;
            }

            if (checkType == typeof(short))
            {
                return true;
            }

            if (checkType == typeof(ushort))
            {
                return true;
            }

            if (checkType == typeof(byte))
            {
                return true;
            }

            if (checkType == typeof(sbyte))
            {
                return true;
            }

            if (checkType == typeof(double))
            {
                return true;
            }

            if (checkType == typeof(float))
            {
                return true;
            }

            if (checkType == typeof(decimal))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="min" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="max" /> parameter is null.</exception>
        public object NextValue(Type type, object min, object max)
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

            var minimum = Convert.ToDouble(min, CultureInfo.InvariantCulture);
            var maximum = Convert.ToDouble(max, CultureInfo.InvariantCulture);

            var isNullable = type.IsNullable();
            var checkType = type;

            if (isNullable)
            {
                // The type is nullable so we need to validate whether we support the type argument
                checkType = type.GetGenericArguments()[0];
            }

            var requiresRounding = RequiresRounding(checkType);

            var value = NextValue<double>(minimum, maximum, requiresRounding);
            var convertedValue = Convert.ChangeType(value, checkType, CultureInfo.InvariantCulture);

            if (isNullable)
            {
                // Create a nullable with the converted value
                var instance = Activator.CreateInstance(type, convertedValue);

                return instance;
            }

            // Return the value converted to its target type
            return convertedValue;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buffer" /> parameter is null.</exception>
        public void NextValue(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            for (var index = 0; index < buffer.Length; index++)
            {
                var next = NextValue(typeof(byte), byte.MinValue, byte.MaxValue);

                buffer[index] = (byte)next;
            }
        }

        private static T NextValue<T>(double min, double max, bool roundValue)
        {
            if (min > max)
            {
                throw new ArgumentOutOfRangeException(nameof(min));
            }

            var range = max - min;
            var variance = _random.NextDouble();

            if (double.IsInfinity(range))
            {
                // We are going against the full range of double
                // Infinity causes calculation problems so we need to break up the range to avoid it
                // Dumb the range down to just half of the possible double values
                // Double has a negative and positive range so by taking the MaxValue, we restrict the range by half
                // By default we will take a random value from the negative range unless the variance is over 0.5
                range = double.MaxValue;

                if (variance >= 0.5D)
                {
                    // Get a random number from the positive side of double numbers
                    min = 0;
                }
            }

            var pointInRange = variance * range;

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

            return (T)Convert.ChangeType(shiftedPoint, typeof(T), CultureInfo.InvariantCulture);
        }

        private bool RequiresRounding(Type type)
        {
            ValidateRequestedType(type);

            if (type == typeof(float))
            {
                return false;
            }

            if (type == typeof(double))
            {
                return false;
            }

            return true;
        }

        private void ValidateRequestedType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (IsSupported(type) == false)
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Error_TypeNotSupportedFormat,
                    GetType().FullName,
                    type.FullName);

                throw new NotSupportedException(message);
            }
        }
    }
}
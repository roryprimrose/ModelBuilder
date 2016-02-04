using System;
using System.Globalization;
using ModelBuilder.Properties;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="BooleanValueGenerator"/>
    /// class is used to generate random date time values.
    /// </summary>
    public class DateTimeValueGenerator : IValueGenerator
    {
        private static readonly Random _random = new Random(Environment.TickCount);

        /// <inheritdoc />
        public object Generate(Type type)
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

            var shift = _random.Next();

            if (type == typeof (DateTime))
            {
                return DateTime.UtcNow.AddSeconds(shift);
            }

            if (type == typeof (TimeSpan))
            {
                return TimeSpan.FromSeconds(shift);
            }

            if (type == typeof (TimeZoneInfo))
            {
                var zones = TimeZoneInfo.GetSystemTimeZones();
                var zoneIndex = _random.Next(0, zones.Count - 1);

                return zones[zoneIndex];
            }

            return DateTimeOffset.UtcNow.AddSeconds(shift);
        }

        /// <inheritdoc />
        public bool IsSupported(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsValueType == false)
            {
                return false;
            }

            if (type == typeof (DateTime))
            {
                return true;
            }

            if (type == typeof (DateTimeOffset))
            {
                return true;
            }

            if (type == typeof (TimeSpan))
            {
                return true;
            }

            if (type == typeof (TimeZoneInfo))
            {
                return true;
            }

            return false;
        }
    }
}
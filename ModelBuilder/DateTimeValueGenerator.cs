using System;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="DateTimeValueGenerator"/>
    /// class is used to generate random date time values.
    /// </summary>
    public class DateTimeValueGenerator : ValueGeneratorBase
    {
        /// <inheritdoc />
        public override object Generate(Type type, string referenceName, object context)
        {
            VerifyGenerateRequest(type, referenceName, context);

            var shift = Generator.Next<int>();

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
                var zoneIndex = Generator.Next(0, zones.Count - 1);

                return zones[zoneIndex];
            }

            return DateTimeOffset.UtcNow.AddSeconds(shift);
        }

        /// <inheritdoc />
        public override bool IsSupported(Type type, string referenceName, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
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
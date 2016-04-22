using System;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="DateTimeValueGenerator"/>
    /// class is used to generate random date time values.
    /// </summary>
    public class DateTimeValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeValueGenerator"/> class.
        /// </summary>
        public DateTimeValueGenerator()
            : base(typeof(DateTime), typeof(DateTime?), typeof(DateTimeOffset), typeof(DateTimeOffset?), typeof(TimeSpan), typeof(TimeSpan?), typeof(TimeZoneInfo))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, object context)
        {
            var generateType = type;

            if (generateType.IsNullable())
            {
                // Allow for a 10% the chance that this might be null
                var range = Generator.NextValue(0, 100);

                if (range < 10)
                {
                    return null;
                }

                // Hijack the type to generator so we can continue with the normal code pointed at the correct type to generate
                generateType = type.GenericTypeArguments[0];
            }

            var shift = Generator.Next<int>();

            if (generateType == typeof(DateTime))
            {
                return DateTime.UtcNow.AddSeconds(shift);
            }

            if (generateType == typeof(TimeSpan))
            {
                return TimeSpan.FromSeconds(shift);
            }

            if (generateType == typeof(TimeZoneInfo))
            {
                var zones = TimeZoneInfo.GetSystemTimeZones();
                var zoneIndex = Generator.NextValue(0, zones.Count - 1);

                return zones[zoneIndex];
            }

            return DateTimeOffset.UtcNow.AddSeconds(shift);
        }
    }
}
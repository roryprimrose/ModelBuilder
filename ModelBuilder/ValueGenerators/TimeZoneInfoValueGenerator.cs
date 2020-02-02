﻿namespace ModelBuilder.ValueGenerators
{
    using System;

    /// <summary>
    ///     The <see cref="TimeZoneInfoValueGenerator" />
    ///     class is used to generate random <see cref="TimeZoneInfo" /> values.
    /// </summary>
    public class TimeZoneInfoValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TimeZoneInfoValueGenerator" /> class.
        /// </summary>
        public TimeZoneInfoValueGenerator() : base(typeof(TimeZoneInfo))
        {
        }

        /// <inheritdoc />
        public override object Generate(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            var zones = TimeZoneInfo.GetSystemTimeZones();
            var zoneIndex = Generator.NextValue(0, zones.Count - 1);

            return zones[zoneIndex];
        }
    }
}
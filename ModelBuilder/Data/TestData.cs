﻿namespace ModelBuilder.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="TestData" />
    ///     class is used to expose some pre-generated test data.
    /// </summary>
    public static class TestData
    {
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1810:InitializeReferenceTypeStaticFieldsInline",
            Justification =
                "This is done in the constructor for the better performance of splitting males an females in a single iteration.")]
        static TestData()
        {
            Companies = ReadResource("Companies");
            Locations = ParseLocations("Locations");
            FemaleNames = ReadResource("FemaleNames");
            MaleNames = ReadResource("MaleNames");
            LastNames = ReadResource("LastNames");
            TimeZones = ReadResource("TimeZones");
            Domains = ReadResource("Domains");
        }

        private static List<Location> ParseLocations(string name)
        {
            var lines = ReadResource(name);
            var parsedLines = new List<Location>(lines.Count);
            var index = 0;

            foreach (var line in lines)
            {
                index++;

#if DEBUG
                try
                {
#endif
                    var location = Location.Parse(line);

                    parsedLines.Add(location);
#if DEBUG
                }
                catch (IndexOutOfRangeException e)
                {
                    throw new InvalidOperationException("Failed to process line '" + line + "' at index" + index, e);
                }
#endif
            }

            return parsedLines;
        }

        private static IReadOnlyList<string> ReadResource(string name)
        {
            var assembly = typeof(TestData).GetTypeInfo().Assembly;
            var resourceName = "ModelBuilder.Resources." + name + ".txt";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);

            var values = new List<string>();

            while (reader.EndOfStream == false)
            {
                var line = reader.ReadLine();

                values.Add(line);
            }

            return values;
        }

        /// <summary>
        ///     Gets a test data set of companies.
        /// </summary>
        public static IReadOnlyList<string> Companies { get; }

        /// <summary>
        ///     Gets a test data set of domains.
        /// </summary>
        public static IReadOnlyList<string> Domains { get; }

        /// <summary>
        ///     Gets a test data set of female names.
        /// </summary>
        public static IReadOnlyList<string> FemaleNames { get; }

        /// <summary>
        ///     Gets a test data set of last names.
        /// </summary>
        public static IReadOnlyList<string> LastNames { get; }

        /// <summary>
        ///     Gets a test data set of locations.
        /// </summary>
        public static IReadOnlyList<Location> Locations { get; }

        /// <summary>
        ///     Gets a test data set of male names.
        /// </summary>
        public static IReadOnlyList<string> MaleNames { get; }

        /// <summary>
        ///     Gets a test data set of time zones.
        /// </summary>
        public static IReadOnlyList<string> TimeZones { get; }
    }
}
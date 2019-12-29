namespace ModelBuilder.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

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
            Companies = ParseValues(ResourceFile.Companies);
            Locations = ParseLocations(ResourceFile.Locations);
            FemaleNames = ParseValues(ResourceFile.FemaleNames);
            MaleNames = ParseValues(ResourceFile.MaleNames);
            LastNames = ParseValues(ResourceFile.LastNames);
            TimeZones = ParseValues(ResourceFile.TimeZones);
            Domains = ParseValues(ResourceFile.Domains);
        }

        private static List<Location> ParseLocations(string locations)
        {
            var lines = ParseValues(locations);
            var parsedLines = new List<Location>(lines.Count);
            var index = 0;

            foreach (var line in lines)
            {
                index++;

                try
                {
                    var location = Location.Parse(line);

                    parsedLines.Add(location);
                }
                catch (IndexOutOfRangeException e)
                {
                    throw new InvalidOperationException("Failed to process line '" + line + "' at index" + index, e);
                }
            }

            return parsedLines;
        }

        private static List<string> ParseValues(string values)
        {
            var items = values.Split(
                new[]
                {
                    Environment.NewLine
                },
                StringSplitOptions.RemoveEmptyEntries);

            return items.ToList();
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
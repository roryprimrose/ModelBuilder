namespace ModelBuilder.Data
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="TestData" />
    ///     class is used to expose some pre-generated test data.
    /// </summary>
    public static class TestData
    {
        private static readonly ConcurrentDictionary<string, IReadOnlyList<string>> _dataCache =
            new ConcurrentDictionary<string, IReadOnlyList<string>>();

        private static readonly ConcurrentDictionary<string, IReadOnlyList<Location>> _locationCache =
            new ConcurrentDictionary<string, IReadOnlyList<Location>>();

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

            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);

            var values = new List<string>();

            while (reader.EndOfStream == false)
            {
                var line = reader.ReadLine();

                if (string.IsNullOrWhiteSpace(line) == false)
                {
                    values.Add(line);
                }
            }

            return values;
        }

        /// <summary>
        ///     Gets a test data set of companies.
        /// </summary>
        public static IReadOnlyList<string> Companies
        {
            get { return _dataCache.GetOrAdd(nameof(Companies), x => ReadResource(nameof(Companies))); }
        }

        /// <summary>
        ///     Gets a test data set of domains.
        /// </summary>
        public static IReadOnlyList<string> Domains
        {
            get { return _dataCache.GetOrAdd(nameof(Domains), x => ReadResource(nameof(Domains))); }
        }

        /// <summary>
        ///     Gets a test data set of female names.
        /// </summary>
        public static IReadOnlyList<string> FemaleNames
        {
            get { return _dataCache.GetOrAdd(nameof(FemaleNames), x => ReadResource(nameof(FemaleNames))); }
        }

        /// <summary>
        ///     Gets a test data set of last names.
        /// </summary>
        public static IReadOnlyList<string> LastNames
        {
            get { return _dataCache.GetOrAdd(nameof(LastNames), x => ReadResource(nameof(LastNames))); }
        }

        /// <summary>
        ///     Gets a test data set of locations.
        /// </summary>
        public static IReadOnlyList<Location> Locations
        {
            get { return _locationCache.GetOrAdd(nameof(Locations), x => ParseLocations(nameof(Locations))); }
        }

        /// <summary>
        ///     Gets a test data set of male names.
        /// </summary>
        public static IReadOnlyList<string> MaleNames
        {
            get { return _dataCache.GetOrAdd(nameof(MaleNames), x => ReadResource(nameof(MaleNames))); }
        }

        /// <summary>
        ///     Gets a test data set of time zones.
        /// </summary>
        public static IReadOnlyList<string> TimeZones
        {
            get { return _dataCache.GetOrAdd(nameof(TimeZones), x => ReadResource(nameof(TimeZones))); }
        }
    }
}
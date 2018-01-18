namespace ModelBuilder.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using ModelBuilder.Properties;

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
            Locations = ParseLocations(Resources.Locations);
            FemaleNames = ParseValues(Resources.FemaleNames);
            MaleNames = ParseValues(Resources.MaleNames);
            LastNames = ParseValues(Resources.LastNames);
            Cultures = ParseValues(Resources.Cultures);

            People = ReadPeople();

            var males = new List<Person>();
            var females = new List<Person>();

            // More efficient to loop manually rather than use LINQ to avoid multiple iterations of the large collection
            foreach (var person in People)
            {
                if (person.Gender == "Male")
                {
                    males.Add(person);
                }
                else
                {
                    females.Add(person);
                }
            }

            Males = new ReadOnlyCollection<Person>(males);
            Females = new ReadOnlyCollection<Person>(females);
        }

        /// <summary>
        ///     Returns a random female from the test data set.
        /// </summary>
        /// <returns>A random female.</returns>
        public static Person NextFemale()
        {
            var generator = new RandomGenerator();

            var index = generator.NextValue(0, Females.Count - 1);

            return Females[index];
        }

        /// <summary>
        ///     Returns a random male from the test data set.
        /// </summary>
        /// <returns>A random male.</returns>
        public static Person NextMale()
        {
            var generator = new RandomGenerator();

            var index = generator.NextValue(0, Males.Count - 1);

            return Males[index];
        }

        /// <summary>
        ///     Returns a random person from the test data set.
        /// </summary>
        /// <returns>A random person.</returns>
        public static Person NextPerson()
        {
            var generator = new RandomGenerator();

            var index = generator.NextValue(0, People.Count - 1);

            return People[index];
        }

        private static List<Location> ParseLocations(string locations)
        {
            var lines = ParseValues(locations);
            var parsedLines = new List<Location>(lines.Count);

            foreach (var line in lines)
            {
                var location = Location.Parse(line);

                parsedLines.Add(location);
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

        private static ReadOnlyCollection<Person> ReadPeople()
        {
            var serializer = new XmlSerializer(typeof(People));

            using (var reader = new StringReader(Resources.People))
            {
                var data = (People)serializer.Deserialize(reader);

                return new ReadOnlyCollection<Person>(data.Items);
            }
        }

        /// <summary>
        ///     Gets a test data set of cultures.
        /// </summary>
        public static IReadOnlyList<string> Cultures { get; }

        /// <summary>
        ///     Gets a test data set of female names.
        /// </summary>
        public static IReadOnlyList<string> FemaleNames { get; }

        /// <summary>
        ///     Gets a test data set of females.
        /// </summary>
        public static IReadOnlyList<Person> Females { get; }

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
        ///     Gets a test data set of males.
        /// </summary>
        public static ReadOnlyCollection<Person> Males { get; }

        /// <summary>
        ///     Gets a test data set of people.
        /// </summary>
        public static ReadOnlyCollection<Person> People { get; }
    }
}
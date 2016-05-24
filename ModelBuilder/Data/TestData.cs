using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Serialization;
using ModelBuilder.Properties;

namespace ModelBuilder.Data
{
    /// <summary>
    /// The <see cref="TestData"/>
    /// class is used to expose some pre-generated test data.
    /// </summary>
    public static class TestData
    {
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline",
            Justification =
                "This is done in the constructor for the better performance of splitting males an females in a single iteration."
            )]
        static TestData()
        {
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
        /// Returns a random female from the test data set.
        /// </summary>
        /// <returns>A random female.</returns>
        public static Person NextFemale()
        {
            var generator = new RandomGenerator();

            var index = generator.NextValue(0, Females.Count - 1);

            return Females[index];
        }

        /// <summary>
        /// Returns a random male from the test data set.
        /// </summary>
        /// <returns>A random male.</returns>
        public static Person NextMale()
        {
            var generator = new RandomGenerator();

            var index = generator.NextValue(0, Males.Count - 1);

            return Males[index];
        }

        /// <summary>
        /// Returns a random person from the test data set.
        /// </summary>
        /// <returns>A random person.</returns>
        public static Person NextPerson()
        {
            var generator = new RandomGenerator();

            var index = generator.NextValue(0, People.Count - 1);

            return People[index];
        }

        private static ReadOnlyCollection<Person> ReadPeople()
        {
            var serializer = new XmlSerializer(typeof(People));

            using (var reader = new StringReader(Resources.People))
            {
                var data = (People) serializer.Deserialize(reader);

                return new ReadOnlyCollection<Person>(data.Items);
            }
        }

        /// <summary>
        /// Gets a test data set of females.
        /// </summary>
        public static ReadOnlyCollection<Person> Females { get; }

        /// <summary>
        /// Gets a test data set of males.
        /// </summary>
        public static ReadOnlyCollection<Person> Males { get; }

        /// <summary>
        /// Gets a test data set of people.
        /// </summary>
        public static ReadOnlyCollection<Person> People { get; }
    }
}
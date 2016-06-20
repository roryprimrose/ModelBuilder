namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using ModelBuilder.Data;

    /// <summary>
    /// The <see cref="TimeZoneValueGenerator"/>
    /// class is used to generate IANA time zone values.
    /// </summary>
    public class TimeZoneValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeZoneValueGenerator"/> class.
        /// </summary>
        public TimeZoneValueGenerator()
            : base(new Regex("TimeZone", RegexOptions.Compiled | RegexOptions.IgnoreCase), typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, LinkedList<object> buildChain)
        {
            string country = null;
            var context = buildChain?.Last.Value;

            if (context != null)
            {
                var expression = new Regex("Country");
                var property = context.FindProperties(expression).FirstOrDefault();

                if (property != null)
                {
                    country = (string)property.GetValue(context, null);
                }
            }

            if (country == null)
            {
                var person = TestData.NextPerson();

                return person.TimeZone;
            }

            var people =
                TestData.People.Where(x => x.TimeZone.IndexOf(country, StringComparison.OrdinalIgnoreCase) > -1)
                    .ToList();

            var filteredIndex = Generator.NextValue(0, people.Count - 1);
            var filteredPerson = people[filteredIndex];

            return filteredPerson.TimeZone;
        }

        /// <inheritdoc />
        public override int Priority
        {
            get;
        } = 1000;
    }
}
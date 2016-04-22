﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using ModelBuilder.Data;

namespace ModelBuilder
{
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
        protected override object GenerateValue(Type type, string referenceName, object context)
        {
            string country = null;

            if (context != null)
            {
                var expression = new Regex("Country");
                var property = context.FindProperties(expression).FirstOrDefault();

                if (property != null)
                {
                    country = (string) property.GetValue(context);
                }
            }

            if (country == null)
            {
                var index = Generator.NextValue(0, TestData.People.Count - 1);
                var person = TestData.People[index];

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
        public override int Priority { get; } = 1000;
    }
}
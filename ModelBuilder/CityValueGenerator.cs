namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using ModelBuilder.Data;

    /// <summary>
    ///     The <see cref="CityValueGenerator" />
    ///     class is used to generate random city values.
    /// </summary>
    public class CityValueGenerator : RelativeValueGenerator
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CityValueGenerator" /> class.
        /// </summary>
        public CityValueGenerator() : base(PropertyExpression.City, typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            var context = executeStrategy?.BuildChain?.Last;
            IEnumerable<Location> locations = TestData.Locations;

            locations = FilterLocations(
                locations,
                PropertyExpression.Country,
                (item, value) => item.Country.Equals(value, StringComparison.OrdinalIgnoreCase),
                context);

            locations = FilterLocations(
                locations,
                PropertyExpression.State,
                (item, value) => item.State.Equals(value, StringComparison.OrdinalIgnoreCase),
                context);

            var availableLocations = locations.ToList();

            if (availableLocations.Count > 0)
            {
                var matchingLocation = availableLocations.Next();

                return matchingLocation.City;
            }

            // There was either no country or no match on the country
            var location = TestData.Locations.Next();


            return location.City;
        }

        private IEnumerable<Location> FilterLocations(
            IEnumerable<Location> locations,
            Regex getExpression,
            Func<Location, string, bool> evaluator,
            object context)
        {
            var matchValue = GetValue<string>(getExpression, context);

            if (string.IsNullOrWhiteSpace(matchValue))
            {
                return locations;
            }

            return locations.Where(x => evaluator(x, matchValue));
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
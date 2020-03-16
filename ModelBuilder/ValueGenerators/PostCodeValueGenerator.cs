namespace ModelBuilder.ValueGenerators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using ModelBuilder.Data;

    /// <summary>
    ///     The <see cref="PostCodeValueGenerator" />
    ///     class is used to generate random post code values.
    /// </summary>
    public class PostCodeValueGenerator : RelativeValueGenerator
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PostCodeValueGenerator" /> class.
        /// </summary>
        public PostCodeValueGenerator() : base(PropertyExpression.PostCode, typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object Generate(IExecuteStrategy executeStrategy, Type type, string referenceName)
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

            locations = FilterLocations(
                locations,
                PropertyExpression.City,
                (item, value) => item.City.Equals(value, StringComparison.OrdinalIgnoreCase),
                context);

            var availableLocations = locations.ToList();

            if (availableLocations.Count > 0)
            {
                var matchingLocation = availableLocations.Next();

                return matchingLocation.PostCode;
            }

            // There was either no country or no match on the country
            var location = TestData.Locations.Next();

            return location.PostCode;
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
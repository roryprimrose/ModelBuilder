namespace ModelBuilder.ValueGenerators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ModelBuilder.Data;

    /// <summary>
    ///     The <see cref="StateValueGenerator" />
    ///     class is used to generate state addressing values.
    /// </summary>
    public class StateValueGenerator : RelativeValueGenerator
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StateValueGenerator" /> class.
        /// </summary>
        public StateValueGenerator() : base(NameExpression.State, typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName)
        {
            var context = executeStrategy?.BuildChain?.Last;
            IEnumerable<Location> locations = TestData.Locations;

            if (context != null)
            {
                var country = GetValue<string>(NameExpression.Country, context);

                if (string.IsNullOrWhiteSpace(country) == false)
                {
                    locations = locations
                        .Where(x => x.Country.Equals(country, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }

            var availableLocations = locations.ToList();

            if (availableLocations.Count > 0)
            {
                var matchingLocation = availableLocations.Next();

                return matchingLocation.State;
            }

            // There was either no country or no match on the country
            var location = TestData.Locations.Next();

            return location.State;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
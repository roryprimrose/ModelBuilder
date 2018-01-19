namespace ModelBuilder
{
    using System;
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
        public CityValueGenerator()
            : base(PropertyExpression.City, typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            var context = executeStrategy?.BuildChain?.Last?.Value;
            var state = GetValue<string>(PropertyExpression.State, context);
            Location location = null;

            if (string.IsNullOrWhiteSpace(state) == false)
            {
                var locationMatches = TestData.Locations.Where(x => x.State.Equals(state, StringComparison.OrdinalIgnoreCase)).ToList();

                location = locationMatches.Next();
            }

            if (location == null)
            {
                // There was either no country or no match on the country
                location = TestData.Locations.Next();
            }

            return location.City;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
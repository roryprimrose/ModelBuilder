namespace ModelBuilder.ValueGenerators
{
    using System;
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
        public StateValueGenerator() : base(PropertyExpression.State, typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object Generate(IExecuteStrategy executeStrategy, Type type, string referenceName)
        {
            var context = executeStrategy?.BuildChain?.Last;
            var country = GetValue<string>(PropertyExpression.Country, context);
            Location location = null;

            if (string.IsNullOrWhiteSpace(country) == false)
            {
                var locationMatches = TestData.Locations
                    .Where(x => x.Country.Equals(country, StringComparison.OrdinalIgnoreCase)).ToList();

                location = locationMatches.Next();
            }

            if (location == null)
            {
                // There was either no country or no match on the country
                location = TestData.Locations.Next();
            }

            return location.State;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
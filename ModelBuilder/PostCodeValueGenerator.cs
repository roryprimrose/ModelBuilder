namespace ModelBuilder
{
    using System;
    using System.Linq;
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
        protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            var context = executeStrategy?.BuildChain?.Last;
            var city = GetValue<string>(PropertyExpression.City, context);
            Location location = null;

            if (string.IsNullOrWhiteSpace(city) == false)
            {
                var locationMatches = TestData.Locations
                    .Where(x => x.City.Equals(city, StringComparison.OrdinalIgnoreCase)).ToList();

                location = locationMatches.Next();
            }

            if (location == null)
            {
                // There was either no country or no match on the country
                location = TestData.Locations.Next();
            }

            return location.PostCode;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
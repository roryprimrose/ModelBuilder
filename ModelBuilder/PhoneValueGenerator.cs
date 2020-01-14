namespace ModelBuilder
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Data;

    /// <summary>
    ///     The <see cref="AgeValueGenerator" />
    ///     class is used to generate phone numbers.
    /// </summary>
    public class PhoneValueGenerator : RelativeValueGenerator
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PhoneValueGenerator" /> class.
        /// </summary>
        public PhoneValueGenerator() : base(new Regex("Phone|Cell|Mobile|Fax", RegexOptions.IgnoreCase), typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
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

            return location.Phone;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
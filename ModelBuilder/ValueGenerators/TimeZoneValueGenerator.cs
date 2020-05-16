namespace ModelBuilder.ValueGenerators
{
    using System;
    using System.Linq;
    using ModelBuilder.Data;

    /// <summary>
    ///     The <see cref="TimeZoneValueGenerator" />
    ///     class is used to generate IANA time zone values.
    /// </summary>
    public class TimeZoneValueGenerator : RelativeValueGenerator
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TimeZoneValueGenerator" /> class.
        /// </summary>
        public TimeZoneValueGenerator() : base(NameExpression.TimeZone, typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName)
        {
            var context = executeStrategy?.BuildChain?.Last;

            var location = GetRelativeLocation(context);

            if (location == null)
            {
                // There was either no country or city or no match on the country or city
                location = TestData.Locations.Next();
            }

            string? timeZone = null;

            if (location != null)
            {
                var countryMatches = TestData.TimeZones
                    .Where(x => x.StartsWith(location.Country, StringComparison.OrdinalIgnoreCase)).ToList();

                // Attempt to find a timezone that contains the city
                var cityMatches = countryMatches
                    .Where(x => x.EndsWith(location.City, StringComparison.OrdinalIgnoreCase)).ToList();

                timeZone = cityMatches.Next();

                if (timeZone != null)
                {
                    return timeZone;
                }

                timeZone = countryMatches.Next();
            }

            if (timeZone == null)
            {
                return TestData.TimeZones.Next();
            }

            return timeZone;
        }

        private Location? GetRelativeLocation(object? context)
        {
            if (context == null)
            {
                return null;
            }

            var city = GetValue<string>(NameExpression.City, context) ?? "";
            var country = GetValue<string>(NameExpression.Country, context) ?? "";

            var allPossibleMatches = TestData.Locations.Where(
                x => x.City.Equals(city, StringComparison.OrdinalIgnoreCase) ||
                     x.Country.Equals(country, StringComparison.OrdinalIgnoreCase)).ToList();

            if (string.IsNullOrWhiteSpace(city) == false)
            {
                var cityMatches = allPossibleMatches.Where(x => x.City.Equals(city, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var cityMatch = cityMatches.Next();

                if (cityMatch != null)
                {
                    return cityMatch;
                }
            }

            if (string.IsNullOrWhiteSpace(country))
            {
                return null;
            }

            var countryMatches = allPossibleMatches
                .Where(x => x.Country.Equals(country, StringComparison.OrdinalIgnoreCase)).ToList();

            return countryMatches.Next();
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
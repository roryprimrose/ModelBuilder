namespace ModelBuilder.ValueGenerators
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using ModelBuilder.Data;

    /// <summary>
    ///     The <see cref="AddressValueGenerator" />
    ///     class is used to generate postal addressing values.
    /// </summary>
    public class AddressValueGenerator : RegexTypeNameValueGenerator
    {
        private static readonly Regex _matchNameExpression =
            new Regex("(?<!(email|internet).*)address", RegexOptions.IgnoreCase);

        private static readonly Regex _multipleAddressExpression = new Regex(
            "Address(Line)?(?<Number>\\d+)",
            RegexOptions.IgnoreCase);

        /// <summary>
        ///     Initializes a new instance of the <see cref="AddressValueGenerator" /> class.
        /// </summary>
        public AddressValueGenerator() : base(_matchNameExpression, typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName)
        {
            if (referenceName != null)
            {
                var multipleMatch = _multipleAddressExpression.Match(referenceName);

                if (multipleMatch.Success)
                {
                    // Get the number from the match
                    var number = int.Parse(multipleMatch.Groups["Number"].Value, CultureInfo.InvariantCulture);

                    if (number == 1)
                    {
                        var floor = Generator.NextValue(1, 15);
                        var unitIndex = Generator.NextValue(0, 15);
                        var unit = (char)(65 + unitIndex);

                        // Return a Unit Xy, Floor X style value
                        return "Unit " + floor + unit + ", Floor " + floor;
                    }

                    if (number > 2)
                    {
                        // This generator will only populate the first two address lines
                        return string.Empty;
                    }
                }
            }

            var addressNumber = Generator.NextValue(1, 1500);
            var location = TestData.Locations.Next();

            return addressNumber + " " + location.StreetName + " " + location.StreetSuffix;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 900;
    }
}
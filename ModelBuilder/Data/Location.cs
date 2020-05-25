namespace ModelBuilder.Data
{
    using System;
    using ModelBuilder.Properties;

    /// <summary>
    ///     The <see cref="Location" />
    ///     class defines location information.
    /// </summary>
    public class Location
    {
        /// <summary>
        ///     Parses a new location from the specified CSV data.
        /// </summary>
        /// <param name="csvData">The CSV data.</param>
        /// <returns>The location.</returns>
        public static Location Parse(string csvData)
        {
            if (string.IsNullOrWhiteSpace(csvData))
            {
                throw new ArgumentException(Resources.ArgumentException_NullOrWhiteSpace, nameof(csvData));
            }

            // This data is expected to be in the following CSV format 
            // Country,State,City,PostCode,StreetName,StreetSuffix,Phone
            var parts = csvData.Split(',');

            var location = new Location
            {
                Country = parts[0],
                State = parts[1],
                City = parts[2],
                PostCode = parts[3],
                StreetName = parts[4],
                StreetSuffix = parts[5],
                Phone = parts[6]
            };

            return location;
        }

        /// <summary>
        ///     Gets or sets the city.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the country.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the phone.
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the post code.
        /// </summary>
        public string PostCode { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the state.
        /// </summary>
        public string State { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the street name.
        /// </summary>
        public string StreetName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the street suffix.
        /// </summary>
        public string StreetSuffix { get; set; } = string.Empty;
    }
}
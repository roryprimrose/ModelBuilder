namespace ModelBuilder.Examples.Models
{
    /// <summary>
    ///     A sample address model. Its location fields are drawn from a single built-in location row
    ///     per instance, so they stay internally consistent.
    /// </summary>
    public class Address
    {
        public string AddressLine1 { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string PostCode { get; set; } = string.Empty;
    }
}

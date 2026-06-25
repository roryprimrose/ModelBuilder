namespace ModelBuilder.Examples.Models
{
    /// <summary>
    ///     A sample address model. Its location fields are drawn from a single built-in location row
    ///     per instance, so they stay internally consistent.
    /// </summary>
    public class Address
    {
        public string AddressLine1 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string PostCode { get; set; }
    }
}

namespace ModelBuilder.IntegrationTests.Models
{
    /// <summary>
    ///     A leaf class used as the element type for collection scenarios across the integration suite.
    /// </summary>
    public class Widget
    {
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }
}

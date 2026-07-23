namespace ModelBuilder.IntegrationTests.Models
{
    /// <summary>
    ///     A class whose <see cref="Sku" /> member is targeted by the custom-named-value-source tests.
    /// </summary>
    public class Product
    {
        public string Name { get; set; } = string.Empty;

        public string Sku { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }
}

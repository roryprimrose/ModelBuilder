namespace ModelBuilder.Examples.Models
{
    /// <summary>
    ///     A sample product with a <see cref="Sku" /> member targeted by a custom value source in the
    ///     examples.
    /// </summary>
    public class Product
    {
        public string Name { get; set; } = string.Empty;

        public string Sku { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }
}

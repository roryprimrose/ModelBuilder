namespace ModelBuilder.UnitTests.Models
{
    public class PropertyModel<T> where T : notnull
    {
        public T Value { get; set; } = default!;
    }
}
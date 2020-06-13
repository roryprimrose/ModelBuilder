namespace ModelBuilder.UnitTests.Models
{
    public class NullablePropertyModel<T> where T : class
    {
        public T Value { get; set; } = default!;
    }
}
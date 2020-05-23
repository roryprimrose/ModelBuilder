namespace ModelBuilder.UnitTests.Models
{
    public class GenericContainer<T> : IGenericContainer<T> where T : class, new()
    {
        public int Age { get; set; }

        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public T Value { get; set; } = new T();
    }
}
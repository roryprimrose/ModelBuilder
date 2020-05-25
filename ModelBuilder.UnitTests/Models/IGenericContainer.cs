namespace ModelBuilder.UnitTests.Models
{
    public interface IGenericContainer<T> where T : class, new()
    {
        int Age { get; set; }
        string Email { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        T Value { get; set; }
    }
}
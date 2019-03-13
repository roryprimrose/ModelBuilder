namespace ModelBuilder.UnitTests.Models
{
    public interface ITestItem
    {
        string FirstName { get; }
    }

    public class TestItem : ITestItem
    {
        public string FirstName { get; set; }
    }
}
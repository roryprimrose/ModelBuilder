namespace ModelBuilder.UnitTests.Models
{
    public interface ICantCreate
    {
        string FirstName { get; set; }
    }

    public interface IStillCantCreate : ICantCreate
    {
        string LastName { get; set; }
    }

    internal class CantCreate : ICantCreate
    {
        public string FirstName { get; set; }
    }

    public abstract class StillCantCreate : ICantCreate
    {
        public string FirstName { get; set; }
    }
}
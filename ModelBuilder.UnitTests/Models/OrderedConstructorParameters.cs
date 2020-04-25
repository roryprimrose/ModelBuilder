namespace ModelBuilder.UnitTests.Models
{
    public class OrderedConstructorParameters
    {
        public OrderedConstructorParameters(string email, string domain, string lastName, string firstName,
            Gender gender)
        {
            Email = email;
            Domain = domain;
            LastName = lastName;
            FirstName = firstName;
            Gender = gender;
        }

        public string Domain { get; }

        public string Email { get; }
        public string FirstName { get; }

        public Gender Gender { get; }

        public string LastName { get; }
    }
}
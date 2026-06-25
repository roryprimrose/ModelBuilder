namespace ModelBuilder.Examples.Models
{
    /// <summary>
    ///     A sample contact whose constructor parameters (<c>firstName</c>, <c>lastName</c>) are never
    ///     exposed as properties. The <see cref="Email" /> is still built consistently from them,
    ///     because constructor arguments become sibling values keyed by parameter name.
    /// </summary>
    public class Contact
    {
        public Contact(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; }

        public string LastName { get; }

        public string Email { get; set; } = string.Empty;
    }
}

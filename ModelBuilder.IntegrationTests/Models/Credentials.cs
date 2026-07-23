namespace ModelBuilder.IntegrationTests.Models
{
    /// <summary>
    ///     A class with members intended to be ignored in tests — a strongly typed member, a
    ///     type-and-name pair, and a member matched by an <c>IgnoringAny</c> naming predicate.
    /// </summary>
    public class Credentials
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ApiKeySecret { get; set; } = string.Empty;
    }
}

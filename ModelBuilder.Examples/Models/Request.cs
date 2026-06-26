namespace ModelBuilder.Examples.Models
{
    /// <summary>
    ///     A sample request type used to show ignore rules. <see cref="Data" /> is ignored by name and
    ///     <see cref="SecretInternal" /> is ignored by a predicate across all types.
    /// </summary>
    public class Request
    {
        public string Reference { get; set; } = string.Empty;

        public string Data { get; set; } = string.Empty;

        public string SecretInternal { get; set; } = string.Empty;
    }
}

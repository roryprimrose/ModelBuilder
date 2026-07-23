namespace ModelBuilder.IntegrationTests.Models
{
    /// <summary>
    ///     A plain, user-defined generic class with no BCL collection shape, used to verify closed
    ///     constructed generic types (for example <c>Box&lt;Widget&gt;</c>) build like any other class -
    ///     both as a root and as a nested member - independent of any mapping registration.
    /// </summary>
    public class Box<T>
    {
        public T? Value { get; set; }
    }
}

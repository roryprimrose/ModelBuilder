namespace ModelBuilder.UnitTests.Models
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Constructors are called indirectly.")]
    public class Copy
    {
        public Copy(Copy source)
        {
        }

        private Copy()
        {
        }

        public Copy Create()
        {
            return new Copy {Value = Value};
        }

        public string Value { get; set; }
    }
}
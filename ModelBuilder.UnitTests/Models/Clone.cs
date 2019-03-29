namespace ModelBuilder.UnitTests.Models
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Constructors are called indirectly.")]
    public class Clone
    {
        public Clone(Clone source)
        {
        }

        public Clone(Clone source, string value)
        {
        }

        private Clone()
        {
        }

        public static Clone Create()
        {
            return new Clone();
        }
    }
}
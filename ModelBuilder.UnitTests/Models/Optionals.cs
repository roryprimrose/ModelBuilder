namespace ModelBuilder.UnitTests.Models
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Constructors are called indirectly.")]
    public class Optionals
    {
        public Optionals(string first)
        {
        }

        public Optionals(string first, int second, string third, int fourth = 0, byte fifth = 4)
        {
        }
    }
}
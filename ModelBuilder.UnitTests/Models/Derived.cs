namespace ModelBuilder.UnitTests.Models
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Constructors are called indirectly.")]
    public class Derived
    {
        public Derived(string first, Company second)
        {
        }

        public Derived(string first, SpecificCompany second, string third)
        {
        }
    }
}
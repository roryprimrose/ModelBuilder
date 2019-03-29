#if NET452
namespace ModelBuilder.UnitTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public static class StringExtensions
    {
        [SuppressMessage("Microsoft.Performance", "CA1801", Justification = "Parameter is used here to satisfy the net452 compiler which does not provide this overload.")]
        public static bool Contains(this string value, string check, StringComparison comparison)
        {
            // Ignore the StringComparison as it is not used in NET452
            return value.Contains(check);
        }
        
        [SuppressMessage("Microsoft.Performance", "CA1801", Justification = "Parameter is used here to satisfy the net452 compiler which does not provide this overload.")]
        public static string Replace(this string value, string find, string use, StringComparison comparison)
        {
            // Ignore the StringComparison as it is not used in NET452
            return value.Replace(find, use);
        }
    }
}
#endif
#if NET472
namespace ModelBuilder.UnitTests.Models;

public static class StringPolyfills
{
    public static int GetHashCode(this string value, StringComparison comparison)
    {
        return value.GetHashCode();
    }
}
#endif
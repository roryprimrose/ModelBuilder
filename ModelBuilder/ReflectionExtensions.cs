namespace ModelBuilder
{
    using System.Diagnostics;
    using System.Reflection;

    internal static class ReflectionExtensions
    {
        public static bool CanPopulate(this PropertyInfo propertyInfo)
        {
            var setMethod = propertyInfo.GetSetMethod();

            if (setMethod != null)
            {
                if (setMethod.IsStatic)
                {
                    // Static properties are not supported
                    return false;
                }

                // This is a publically settable instance property
                // Against this we can assign both value and reference types
                return true;
            }

            // There is no set method. This is a read-only property
            var getMethod = propertyInfo.GetGetMethod();

            Debug.Assert(
                getMethod != null,
                "This is a private property which should not have been included in the set of properties to evaluate.");

            if (getMethod.IsStatic)
            {
                // Static methods are not supported
                return false;
            }

            if (getMethod.ReturnType.IsValueType)
            {
                // We can't populate a value type via a get method
                return false;
            }

            if (getMethod.ReturnType == typeof(string))
            {
                // We can't populate a string type via a get method because strings are immutable
                return false;
            }

            return true;
        }
    }
}
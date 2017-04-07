namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     The
    /// </summary>
    public static class PropertyResolverExtensions
    {
        /// <summary>
        ///     Gets the properties on the type that are supported for data generation.
        /// </summary>
        /// <param name="resolver">The property resolver.</param>
        /// <param name="type">The type to evaluate.</param>
        /// <returns>The supported properties on the type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="resolver" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public static IEnumerable<PropertyInfo> GetProperties(this IPropertyResolver resolver, Type type)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return from x in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                where resolver.CanPopulate(x)
                select x;
        }

        /// <summary>
        ///     Gets the properties on the type that have their name match the specified expression.
        /// </summary>
        /// <param name="resolver">The property resolver.</param>
        /// <param name="type">The type to evaluate.</param>
        /// <param name="expression">The property name match expression.</param>
        /// <returns>The matching properties.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="resolver" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        public static IEnumerable<PropertyInfo> GetProperties(
            this IPropertyResolver resolver,
            Type type,
            Regex expression)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return from x in resolver.GetProperties(type)
                where expression.IsMatch(x.Name)
                select x;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="ObjectExtensions"/>
    /// class provides extension methods for <see cref="object"/> instances.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Finds the properties on the instance that match the specified expression.
        /// </summary>
        /// <param name="source">The source instance.</param>
        /// <param name="expression">The evaluation expression.</param>
        /// <returns>The matching properties.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression"/> parameter is <c>null</c>.</exception>
        public static IEnumerable<PropertyInfo> FindProperties(this object source, Regex expression)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return from x in source.GetType().GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where expression.IsMatch(x.Name)
                select x;
        }
    }
}
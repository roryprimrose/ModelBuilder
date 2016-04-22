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

            return from x in source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where expression.IsMatch(x.Name)
                select x;
        }

        /// <summary>
        /// Finds the property on the instance that matches the specified name.
        /// </summary>
        /// <param name="source">The source instance.</param>
        /// <param name="name">The property name.</param>
        /// <returns>The matching property, or <c>null</c> if no property found.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> parameter is <c>null</c>.</exception>
        public static PropertyInfo FindProperty(this object source, string name)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var type = source.GetType();

            return type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        }

        /// <summary>
        /// Gets whether the instance has a property of the specified name.
        /// </summary>
        /// <param name="source">The source instance.</param>
        /// <param name="name">The property name.</param>
        /// <returns><c>true</c> if the instance has the property; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="name"/> parameter is <c>null</c>.</exception>
        public static bool HasProperty(this object source, string name)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var property = FindProperty(source, name);

            if (property == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets whether the instance has a property matching the specified expression.
        /// </summary>
        /// <param name="source">The source instance.</param>
        /// <param name="expression">The evaluation expression.</param>
        /// <returns><c>true</c> if the instance has the property; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="source"/> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression"/> parameter is <c>null</c>.</exception>
        public static bool HasProperty(this object source, Regex expression)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var properties = FindProperties(source, expression);

            if (properties.Any())
            {
                return true;
            }

            return false;
        }
    }
}
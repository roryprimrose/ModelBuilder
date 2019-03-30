namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    ///     The <see cref="Extensions" />
    ///     class provides common extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        ///     Gets whether the specified type is a nullable type.
        /// </summary>
        /// <param name="type">The type to validate.</param>
        /// <returns><c>true</c> if the type is nullable; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is null.</exception>
        public static bool IsNullable(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.TypeIsGenericType() == false)
            {
                return false;
            }

            if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns a random item from the data set.
        /// </summary>
        /// <param name="source">The source data set.</param>
        /// <returns>A new data item.</returns>
        public static T Next<T>(this IReadOnlyList<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source.Count == 0)
            {
                return default(T);
            }

            var generator = new RandomGenerator();

            var index = generator.NextValue(0, source.Count - 1);
           
            return source[index];
        }

        /// <summary>
        ///     Makes a change to the instance using the specified action.
        /// </summary>
        /// <typeparam name="T">The type of instance being changed.</typeparam>
        /// <param name="instance">The instance to update.</param>
        /// <param name="action">The action to run against the instance.</param>
        /// <returns>The updated instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is null.</exception>
        public static T Set<T>(this T instance, Action<T> action)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            action(instance);

            return instance;
        }

        /// <summary>
        ///     Sets values on each instance in a set.
        /// </summary>
        /// <typeparam name="T">The type of instance to configure.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The configuration action.</param>
        /// <returns>A list of the instances.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instances" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is null.</exception>
        [SuppressMessage("Microsoft.Design",
            "CA1002:DoNotExposeGenericLists",
            Justification =
                "For usability in the calling code, the return type reflects the actual type to avoid unnecessary conversion or casting.")]
        public static List<T> SetEach<T>(this IEnumerable<T> instances, Action<T> action) where T : class
        {
            if (instances == null)
            {
                throw new ArgumentNullException(nameof(instances));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var items = instances as List<T>;

            if (items == null)
            {
                // It is not already a list so we need to convert it first
                items = instances.ToList();
            }

            return SetEach(items, action);
        }

        /// <summary>
        ///     Sets values on each instance in a set.
        /// </summary>
        /// <typeparam name="T">The type of instance to configure.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The configuration action.</param>
        /// <returns>A list of the instances.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instances" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is null.</exception>
        [SuppressMessage("Microsoft.Design",
            "CA1002:DoNotExposeGenericLists",
            Justification =
                "For usability in the calling code, the return type reflects the actual type to avoid unnecessary conversion or casting.")]
        public static List<T> SetEach<T>(this List<T> instances, Action<T> action) where T : class
        {
            if (instances == null)
            {
                throw new ArgumentNullException(nameof(instances));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            instances.ForEach(action);

            return instances;
        }

        /// <summary>
        ///     Makes a change to each instance using the specified action.
        /// </summary>
        /// <typeparam name="T">The type of instance being changed.</typeparam>
        /// <param name="instances">The instances to update.</param>
        /// <param name="action">The action to run against the instance.</param>
        /// <returns>The updated instances.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instances" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is null.</exception>
        public static ICollection<T> SetEach<T>(this ICollection<T> instances, Action<T> action)
        {
            if (instances == null)
            {
                throw new ArgumentNullException(nameof(instances));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (var instance in instances)
            {
                action(instance);
            }

            return instances;
        }
    }
}
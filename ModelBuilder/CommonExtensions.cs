namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="CommonExtensions" />
    ///     class provides common extension methods.
    /// </summary>
    public static class CommonExtensions
    {
        /// <summary>
        ///     Gets whether the specified type is a nullable type.
        /// </summary>
        /// <param name="type">The type to validate.</param>
        /// <returns><c>true</c> if the type is nullable; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public static bool IsNullable(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsGenericType == false)
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
                return default!;
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
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
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
    }
}
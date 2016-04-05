using System;
using System.Collections.Generic;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="Extensions"/>
    /// class provides common extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Makes a change to the instance using the specified action.
        /// </summary>
        /// <typeparam name="T">The type of instance being changed.</typeparam>
        /// <param name="instance">The instance to update.</param>
        /// <param name="action">The action to run against the instance.</param>
        /// <returns>The updated instance.</returns>
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
        /// Makes a change to each instance using the specified action.
        /// </summary>
        /// <typeparam name="T">The type of instance being changed.</typeparam>
        /// <param name="instances">The instances to update.</param>
        /// <param name="action">The action to run against the instance.</param>
        /// <returns>The updated instances.</returns>
        public static T SetEach<T>(this T instances, Action<T> action) where T : ICollection<T>
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
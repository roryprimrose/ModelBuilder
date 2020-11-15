namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    ///     The <see cref="SetEachExtensions" />
    ///     class provides SetEach extension methods.
    /// </summary>
    public static class SetEachExtensions
    {
        /// <summary>
        ///     Sets values on each instance in a set.
        /// </summary>
        /// <typeparam name="T">The type of instance to configure.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The configuration action.</param>
        /// <returns>A list of the instances.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instances" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1002:DoNotExposeGenericLists",
            Justification =
                "For usability in the calling code, the return type reflects the actual type to avoid unnecessary conversion or casting.")]
        public static List<T> SetEach<T>(this IEnumerable<T> instances, Action<T> action) where T : class
        {
            instances = instances ?? throw new ArgumentNullException(nameof(instances));

            action = action ?? throw new ArgumentNullException(nameof(action));

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
        /// <exception cref="ArgumentNullException">The <paramref name="instances" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1002:DoNotExposeGenericLists",
            Justification =
                "For usability in the calling code, the return type reflects the actual type to avoid unnecessary conversion or casting.")]
        public static IList<T> SetEach<T>(this IList<T> instances, Action<T> action) where T : class
        {
            return SetEachExplicit(instances, action);
        }

        /// <summary>
        ///     Sets values on each instance in a set.
        /// </summary>
        /// <typeparam name="T">The type of instance to configure.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The configuration action.</param>
        /// <returns>A list of the instances.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instances" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1002:DoNotExposeGenericLists",
            Justification =
                "For usability in the calling code, the return type reflects the actual type to avoid unnecessary conversion or casting.")]
        public static IReadOnlyList<T> SetEach<T>(this IReadOnlyList<T> instances, Action<T> action) where T : class
        {
            return SetEachExplicit(instances, action);
        }

        /// <summary>
        ///     Sets values on each instance in a set.
        /// </summary>
        /// <typeparam name="T">The type of instance to configure.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The configuration action.</param>
        /// <returns>A list of the instances.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instances" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1002:DoNotExposeGenericLists",
            Justification =
                "For usability in the calling code, the return type reflects the actual type to avoid unnecessary conversion or casting.")]
        public static List<T> SetEach<T>(this List<T> instances, Action<T> action) where T : class
        {
            return SetEachExplicit(instances, action);
        }

        /// <summary>
        ///     Sets values on each instance in a set.
        /// </summary>
        /// <typeparam name="T">The type of instance to configure.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The configuration action.</param>
        /// <returns>A list of the instances.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instances" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1002:DoNotExposeGenericLists",
            Justification =
                "For usability in the calling code, the return type reflects the actual type to avoid unnecessary conversion or casting.")]
        public static Collection<T> SetEach<T>(this Collection<T> instances, Action<T> action) where T : class
        {
            return SetEachExplicit(instances, action);
        }

        /// <summary>
        ///     Sets values on each instance in a set.
        /// </summary>
        /// <typeparam name="T">The type of instance to configure.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The configuration action.</param>
        /// <returns>A list of the instances.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instances" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1002:DoNotExposeGenericLists",
            Justification =
                "For usability in the calling code, the return type reflects the actual type to avoid unnecessary conversion or casting.")]
        public static ICollection<T> SetEach<T>(this ICollection<T> instances, Action<T> action) where T : class
        {
            return SetEachExplicit(instances, action);
        }

        /// <summary>
        ///     Sets values on each instance in a set.
        /// </summary>
        /// <typeparam name="T">The type of instance to configure.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The configuration action.</param>
        /// <returns>A list of the instances.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instances" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1002:DoNotExposeGenericLists",
            Justification =
                "For usability in the calling code, the return type reflects the actual type to avoid unnecessary conversion or casting.")]
        public static IReadOnlyCollection<T> SetEach<T>(this IReadOnlyCollection<T> instances, Action<T> action)
            where T : class
        {
            return SetEachExplicit(instances, action);
        }

        /// <summary>
        ///     Sets values on each instance in a set.
        /// </summary>
        /// <typeparam name="T">The type of instance to configure.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The configuration action.</param>
        /// <returns>A list of the instances.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instances" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1002:DoNotExposeGenericLists",
            Justification =
                "For usability in the calling code, the return type reflects the actual type to avoid unnecessary conversion or casting.")]
        public static ReadOnlyCollection<T> SetEach<T>(this ReadOnlyCollection<T> instances, Action<T> action)
            where T : class
        {
            return SetEachExplicit(instances, action);
        }

        /// <summary>
        ///     Sets values on each instance in a set.
        /// </summary>
        /// <typeparam name="TKey">The type of key used in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of instance to configure.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The configuration action.</param>
        /// <returns>A list of the instances.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instances" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1002:DoNotExposeGenericLists",
            Justification =
                "For usability in the calling code, the return type reflects the actual type to avoid unnecessary conversion or casting.")]
        public static IDictionary<TKey, TValue> SetEach<TKey, TValue>(this IDictionary<TKey, TValue> instances,
            Action<KeyValuePair<TKey, TValue>> action) where TValue : class
        {
            return SetEachExplicit(instances, action);
        }

        /// <summary>
        ///     Sets values on each instance in a set.
        /// </summary>
        /// <typeparam name="TKey">The type of key used in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of instance to configure.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The configuration action.</param>
        /// <returns>A list of the instances.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instances" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1002:DoNotExposeGenericLists",
            Justification =
                "For usability in the calling code, the return type reflects the actual type to avoid unnecessary conversion or casting.")]
        public static Dictionary<TKey, TValue> SetEach<TKey, TValue>(this Dictionary<TKey, TValue> instances,
            Action<KeyValuePair<TKey, TValue>> action) where TKey : notnull where TValue : class
        {
            return SetEachExplicit(instances, action);
        }

        /// <summary>
        ///     Sets values on each instance in a set.
        /// </summary>
        /// <typeparam name="TKey">The type of key used in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of instance to configure.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The configuration action.</param>
        /// <returns>A list of the instances.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instances" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1002:DoNotExposeGenericLists",
            Justification =
                "For usability in the calling code, the return type reflects the actual type to avoid unnecessary conversion or casting.")]
        public static IReadOnlyDictionary<TKey, TValue> SetEach<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> instances, Action<KeyValuePair<TKey, TValue>> action)
            where TValue : class
        {
            return SetEachExplicit(instances, action);
        }

        /// <summary>
        ///     Sets values on each instance in a set.
        /// </summary>
        /// <typeparam name="TKey">The type of key used in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of instance to configure.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The configuration action.</param>
        /// <returns>A list of the instances.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instances" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1002:DoNotExposeGenericLists",
            Justification =
                "For usability in the calling code, the return type reflects the actual type to avoid unnecessary conversion or casting.")]
        public static ReadOnlyDictionary<TKey, TValue> SetEach<TKey, TValue>(
            this ReadOnlyDictionary<TKey, TValue> instances, Action<KeyValuePair<TKey, TValue>> action)
            where TKey : notnull
            where TValue : class
        {
            return SetEachExplicit(instances, action);
        }

        /// <summary>
        ///     Sets values on each instance in a set.
        /// </summary>
        /// <typeparam name="TList">The type of list to configure.</typeparam>
        /// <typeparam name="TEntry">The type of instance to configure.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The configuration action.</param>
        /// <returns>A list of the instances.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instances" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1002:DoNotExposeGenericLists",
            Justification =
                "For usability in the calling code, the return type reflects the actual type to avoid unnecessary conversion or casting.")]
        public static TList SetEachExplicit<TList, TEntry>(this TList instances, Action<TEntry> action)
            where TList : IEnumerable<TEntry> where TEntry : class
        {
            instances = instances ?? throw new ArgumentNullException(nameof(instances));

            action = action ?? throw new ArgumentNullException(nameof(action));

            foreach (var instance in instances)
            {
                action(instance);
            }

            return instances;
        }

        /// <summary>
        ///     Sets values on each instance in a set.
        /// </summary>
        /// <typeparam name="TDictionary">The type of dictionary to configure.</typeparam>
        /// <typeparam name="TKey">The type of key to configure.</typeparam>
        /// <typeparam name="TValue">The type of instance to configure.</typeparam>
        /// <param name="instances">The instances.</param>
        /// <param name="action">The configuration action.</param>
        /// <returns>A list of the instances.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instances" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1002:DoNotExposeGenericLists",
            Justification =
                "For usability in the calling code, the return type reflects the actual type to avoid unnecessary conversion or casting.")]
        public static TDictionary SetEachExplicit<TDictionary, TKey, TValue>(this TDictionary instances,
            Action<KeyValuePair<TKey, TValue>> action) where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
            where TValue : class
        {
            instances = instances ?? throw new ArgumentNullException(nameof(instances));

            action = action ?? throw new ArgumentNullException(nameof(action));

            foreach (var instance in instances)
            {
                action(instance);
            }

            return instances;
        }
    }
}
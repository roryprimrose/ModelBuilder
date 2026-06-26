namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="SetExtensions" />
    ///     class provides the post-build <c>Set</c> helper for making changes to a created instance.
    /// </summary>
    public static class SetExtensions
    {
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
            instance = instance ?? throw new ArgumentNullException(nameof(instance));
            action = action ?? throw new ArgumentNullException(nameof(action));

            action(instance);

            return instance;
        }
    }
}

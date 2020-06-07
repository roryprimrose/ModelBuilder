namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="BuildConfigurationExtensions" />
    ///     class provides extension methods for the <see cref="IBuildConfiguration" /> interface.
    /// </summary>
    public static partial class BuildConfigurationExtensions
    {
        /// <summary>
        ///     Writes the log entry using the specified action after the execute strategy is invoked.
        /// </summary>
        /// <typeparam name="T">The type of instance to create and populate.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="action">The logging action to call.</param>
        /// <returns>The execute strategy to invoke.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        public static IExecuteStrategy<T> WriteLog<T>(this IBuildConfiguration configuration, Action<string> action)
            where T : notnull
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return configuration.UsingExecuteStrategy<DefaultExecuteStrategy<T>>().WriteLog(action);
        }

        /// <summary>
        ///     Writes the log entry using the specified action after the execute strategy is invoked.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="action">The logging action to call.</param>
        /// <returns>The execute strategy to invoke.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        public static IExecuteStrategy WriteLog(this IBuildConfiguration configuration, Action<string> action)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return configuration.UsingExecuteStrategy<DefaultExecuteStrategy>().WriteLog(action);
        }
    }
}
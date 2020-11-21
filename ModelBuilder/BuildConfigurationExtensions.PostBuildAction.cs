namespace ModelBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    ///     The <see cref="BuildConfigurationExtensions" />
    ///     class provides extension methods for the <see cref="IBuildConfiguration" /> interface.
    /// </summary>
    public static partial class BuildConfigurationExtensions
    {
        /// <summary>
        ///     Adds a new post-build action to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="postBuildAction">The post-build action.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="postBuildAction" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, IPostBuildAction postBuildAction)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            postBuildAction = postBuildAction ?? throw new ArgumentNullException(nameof(postBuildAction));

            configuration.PostBuildActions.Add(postBuildAction);

            return configuration;
        }

        /// <summary>
        ///     Adds a new post-build action to the configuration.
        /// </summary>
        /// <typeparam name="T">The type of post-build action to add.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddPostBuildAction<T>(this IBuildConfiguration configuration)
            where T : IPostBuildAction, new()
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var postBuildAction = new T();

            configuration.PostBuildActions.Add(postBuildAction);

            return configuration;
        }

        /// <summary>
        ///     Removes post-build actions from the configuration that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of post-build action to remove.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration RemovePostBuildAction<T>(this IBuildConfiguration configuration)
            where T : IPostBuildAction
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var itemsToRemove = configuration.PostBuildActions.Where(x => x.GetType().IsAssignableFrom(typeof(T)))
                .ToList();

            foreach (var rule in itemsToRemove)
            {
                configuration.PostBuildActions.Remove(rule);
            }

            return configuration;
        }

        /// <summary>
        ///     Updates a post-build action.
        /// </summary>
        /// <typeparam name="T">The type of post-build action being updated.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="action">The action to run against the post-build action.</param>
        /// <returns>The build configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">
        ///     The <typeparamref name="T" /> post-build action was not found in the build
        ///     configuration.
        /// </exception>
        public static IBuildConfiguration UpdatePostBuildAction<T>(this IBuildConfiguration configuration,
            Action<T> action)
            where T : IPostBuildAction
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            action = action ?? throw new ArgumentNullException(nameof(action));

            var targetType = typeof(T);
            var postBuildAction = configuration.PostBuildActions.OfType<T>()
                .FirstOrDefault(x => x.GetType() == targetType);

            if (postBuildAction == null)
            {
                throw new InvalidOperationException(
                    $"PostBuildAction {targetType.FullName} does not exist in the BuildConfiguration");
            }

            action(postBuildAction);

            return configuration;
        }
    }
}
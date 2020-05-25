namespace ModelBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     The <see cref="BuildConfigurationExtensions" />
    ///     class provides extension methods for the <see cref="IBuildConfiguration" /> interface.
    /// </summary>
    public static partial class BuildConfigurationExtensions
    {
        /// <summary>
        ///     Adds configuration provided by the specified configuration module.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="module">The module.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="module" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, IConfigurationModule module)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            module.Configure(configuration);

            return configuration;
        }

        /// <summary>
        ///     Creates an instance of <typeparamref name="T" /> using the specified build configuration and any provided
        ///     constructor arguments.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <param name="buildConfiguration">The build configuration to create the instance with.</param>
        /// <param name="args">The constructor arguments to create the type with.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <remarks>This method uses <see cref="DefaultExecuteStrategy{T}" /> to create the instance.</remarks>
#if NETSTANDARD2_1
        [return: MaybeNull]
#endif
        public static T Create<T>(this IBuildConfiguration buildConfiguration, params object[] args)
        {
            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            return buildConfiguration.UsingExecuteStrategy<DefaultExecuteStrategy<T>>().Create(args);
        }

        /// <summary>
        ///     Creates an instance of a type using the specified build configuration and any provided constructor arguments.
        /// </summary>
        /// <param name="buildConfiguration">The build configuration to create the instance with.</param>
        /// <param name="instanceType">The type of instance to create.</param>
        /// <param name="args">The constructor arguments to create the type with.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceType" /> parameter is <c>null</c>.</exception>
        /// <remarks>This method uses <see cref="DefaultExecuteStrategy" /> to create the instance.</remarks>
        public static object? Create(this IBuildConfiguration buildConfiguration, Type instanceType,
            params object[] args)
        {
            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            if (instanceType == null)
            {
                throw new ArgumentNullException(nameof(instanceType));
            }

            return buildConfiguration.UsingExecuteStrategy<DefaultExecuteStrategy>().Create(instanceType, args);
        }

        /// <summary>
        ///     Populates the specified instance using the provided build configuration.
        /// </summary>
        /// <typeparam name="T">The type of instance to populate.</typeparam>
        /// <param name="buildConfiguration">The build configuration to use.</param>
        /// <param name="instance">The instance to populate.</param>
        /// <returns>The updated instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        public static T Populate<T>(this IBuildConfiguration buildConfiguration, T instance)
        {
            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return buildConfiguration.UsingExecuteStrategy<DefaultExecuteStrategy<T>>().Populate(instance);
        }

        /// <summary>
        ///     Returns a new <see cref="IExecuteStrategy{T}" /> for the specified build configuration.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IExecuteStrategy{T}" /> to return.</typeparam>
        /// <param name="buildConfiguration">The build configuration.</param>
        /// <returns>A new execute strategy.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        public static T UsingExecuteStrategy<T>(this IBuildConfiguration buildConfiguration)
            where T : IExecuteStrategy, new()
        {
            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            var executeStrategy = new T();

            executeStrategy.Initialize(buildConfiguration);

            return executeStrategy;
        }

        /// <summary>
        ///     Adds configuration using the specified module type.
        /// </summary>
        /// <typeparam name="T">The type of configuration module to add.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration UsingModule<T>(this IBuildConfiguration configuration)
            where T : IConfigurationModule, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var module = new T();

            module.Configure(configuration);

            return configuration;
        }
    }
}
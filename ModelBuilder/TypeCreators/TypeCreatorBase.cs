namespace ModelBuilder.TypeCreators
{
    using System;
    using System.Globalization;
    using ModelBuilder.Properties;

    /// <summary>
    ///     The <see cref="TypeCreatorBase" />
    ///     class is used to provide the common implementation of a type creator.
    /// </summary>
    public abstract class TypeCreatorBase : ITypeCreator
    {
        private static readonly IRandomGenerator _random = new RandomGenerator();

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public virtual bool CanCreate(Type type, string referenceName, IBuildConfiguration configuration,
            IBuildChain buildChain)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var buildType = ResolveBuildType(type, configuration);

            if (buildType.IsInterface)
            {
                return false;
            }

            if (buildType.IsAbstract)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public virtual bool CanPopulate(Type type, string referenceName, IBuildConfiguration buildConfiguration,
            IBuildChain buildChain)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return true;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        public virtual object Create(
            Type type,
            string referenceName,
            IExecuteStrategy executeStrategy,
            params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (executeStrategy.BuildChain == null)
            {
                throw new InvalidOperationException(Resources.ExecuteStrategy_NoBuildChain);
            }

            var buildType = ResolveBuildType(type, executeStrategy.Configuration);

            if (CanCreate(type, referenceName, executeStrategy.Configuration, executeStrategy.BuildChain) == false)
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Error_GenerationNotSupportedFormat,
                    GetType().FullName,
                    type.FullName,
                    referenceName ?? "<null>");

                throw new NotSupportedException(message);
            }

            return CreateInstance(buildType, referenceName, executeStrategy, args);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        public virtual object Populate(object instance, IExecuteStrategy executeStrategy)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (executeStrategy.BuildChain == null)
            {
                throw new InvalidOperationException(Resources.ExecuteStrategy_NoBuildChain);
            }
            
            if (CanPopulate(instance.GetType(), null, executeStrategy.Configuration, executeStrategy.BuildChain) == false)
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Error_GenerationNotSupportedFormat,
                    GetType().FullName,
                    instance.GetType(),
                    "<null>");

                throw new NotSupportedException(message);
            }
            // The default will be to not do any additional population of the instance
            return PopulateInstance(instance, executeStrategy);
        }

        /// <summary>
        ///     Creates an instance of the type with the specified arguments.
        /// </summary>
        /// <param name="type">The type of instance to create.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name the instance is intended for.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <param name="args">The constructor parameters to create the instance with.</param>
        /// <returns>A new instance.</returns>
        protected abstract object CreateInstance(
            Type type,
            string referenceName,
            IExecuteStrategy executeStrategy,
            params object[] args);

        /// <summary>
        ///     Populates the specified instance using an execution strategy.
        /// </summary>
        /// <param name="instance">The instance to populate.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <returns>The populated instance.</returns>
        protected abstract object PopulateInstance(object instance, IExecuteStrategy executeStrategy);

        /// <summary>
        ///     Resolves the type to build based on the specified type.
        /// </summary>
        /// <param name="requestedType">The requested type.</param>
        /// <param name="buildConfiguration">The build configuration.</param>
        /// <returns>The type to build.</returns>
        protected virtual Type ResolveBuildType(Type requestedType, IBuildConfiguration buildConfiguration)
        {
            if (requestedType == null)
            {
                throw new ArgumentNullException(nameof(requestedType));
            }

            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            return buildConfiguration.TypeResolver.GetBuildType(buildConfiguration, requestedType);
        }

        /// <inheritdoc />
        public virtual bool AutoDetectConstructor => true;

        /// <inheritdoc />
        public virtual bool AutoPopulate => true;

        /// <inheritdoc />
        public virtual int Priority { get; } = 0;

        /// <summary>
        ///     Gets the random generator for this instance.
        /// </summary>
        protected virtual IRandomGenerator Generator { get; } = _random;
    }
}
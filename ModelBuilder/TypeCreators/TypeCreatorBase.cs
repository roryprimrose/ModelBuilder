namespace ModelBuilder.TypeCreators
{
    using System;
    using System.Globalization;
    using System.Reflection;
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
        public bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, Type type)
        {
            return CanCreate(configuration, buildChain, type, null);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, ParameterInfo parameterInfo)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            return CanCreate(configuration, buildChain, parameterInfo.ParameterType, parameterInfo.Name);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return CanCreate(configuration, buildChain, propertyInfo.PropertyType, propertyInfo.Name);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public bool CanPopulate(IBuildConfiguration configuration, IBuildChain buildChain, Type type)
        {
            return CanPopulate(configuration, buildChain, type, null);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public bool CanPopulate(IBuildConfiguration configuration, IBuildChain buildChain, ParameterInfo parameterInfo)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            return CanPopulate(configuration, buildChain, parameterInfo.ParameterType, parameterInfo.Name);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public bool CanPopulate(IBuildConfiguration configuration, IBuildChain buildChain, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return CanPopulate(configuration, buildChain, propertyInfo.PropertyType, propertyInfo.Name);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        public object? Create(IExecuteStrategy executeStrategy, Type type, params object?[]? args)
        {
            return Create(executeStrategy, type, null, args);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        public object? Create(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo, params object?[]? args)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            return Create(executeStrategy, parameterInfo.ParameterType, parameterInfo.Name, args);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        public object? Create(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo, params object?[]? args)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return Create(executeStrategy, propertyInfo.PropertyType, propertyInfo.Name, args);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        public virtual object Populate(IExecuteStrategy executeStrategy, object instance)
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

            if (CanPopulate(executeStrategy.Configuration, executeStrategy.BuildChain, instance.GetType(), null)
                == false)
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
            return PopulateInstance(executeStrategy, instance);
        }

        /// <summary>
        ///     Returns whether this type creator can create the specified type.
        /// </summary>
        /// <param name="configuration">The build configuration.</param>
        /// <param name="buildChain">The chain of instances built up to this point.</param>
        /// <param name="type">The type to evaluate.</param>
        /// <param name="referenceName">The property or parameter name to evaluate.</param>
        /// <returns><c>true</c> if this creator can create the type; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        protected virtual bool CanCreate(IBuildConfiguration configuration,
            IBuildChain buildChain, Type type, string? referenceName)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var buildType = ResolveBuildType(configuration, type);

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

        /// <summary>
        ///     Returns whether this type creator can populate the specified type.
        /// </summary>
        /// <param name="configuration">The build configuration.</param>
        /// <param name="buildChain">The chain of instances built up to this point.</param>
        /// <param name="type">The type to evaluate.</param>
        /// <param name="referenceName">The property or parameter name to evaluate.</param>
        /// <returns><c>true</c> if this creator can populate the type; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        protected virtual bool CanPopulate(IBuildConfiguration configuration,
            IBuildChain buildChain, Type type, string? referenceName)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return true;
        }

        /// <summary>
        ///     Creates an instance of the type with the specified arguments.
        /// </summary>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <param name="type">The type to evaluate.</param>
        /// <param name="referenceName">The property or parameter name to evaluate.</param>
        /// <param name="args">The constructor parameters to create the instance with.</param>
        /// <returns>A new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        protected virtual object? Create(IExecuteStrategy executeStrategy,
            Type type,
            string? referenceName,
            params object?[]? args)
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

            var buildType = ResolveBuildType(executeStrategy.Configuration, type);

            if (CanCreate(executeStrategy.Configuration, executeStrategy.BuildChain, buildType, referenceName) == false)
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Error_GenerationNotSupportedFormat,
                    GetType().FullName,
                    type.FullName,
                    referenceName ?? "<null>");

                throw new NotSupportedException(message);
            }

            if (buildType != type)
            {
                executeStrategy.Log.MappedType(type, buildType);
            }

            return CreateInstance(executeStrategy, buildType, referenceName, args);
        }

        /// <summary>
        ///     Creates an instance of the type with the specified arguments.
        /// </summary>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <param name="type">The type of instance to create.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name the instance is intended for.</param>
        /// <param name="args">The constructor parameters to create the instance with.</param>
        /// <returns>A new instance.</returns>
        protected abstract object? CreateInstance(IExecuteStrategy executeStrategy,
            Type type,
            string? referenceName,
            params object?[]? args);

        /// <summary>
        ///     Populates the specified instance using an execution strategy.
        /// </summary>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <param name="instance">The instance to populate.</param>
        /// <returns>The populated instance.</returns>
        protected abstract object PopulateInstance(IExecuteStrategy executeStrategy, object instance);

        /// <summary>
        ///     Resolves the type to build based on the specified type.
        /// </summary>
        /// <param name="buildConfiguration">The build configuration.</param>
        /// <param name="requestedType">The requested type.</param>
        /// <returns>The type to build.</returns>
        protected virtual Type ResolveBuildType(IBuildConfiguration buildConfiguration, Type requestedType)
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
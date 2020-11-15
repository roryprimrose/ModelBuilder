namespace ModelBuilder.BuildActions
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using ModelBuilder.TypeCreators;

    /// <summary>
    ///     The <see cref="TypeCreatorBuildAction" />
    ///     class is used to provide a build action that uses a matching <see cref="ITypeCreator" /> to create and/or populate
    ///     values.
    /// </summary>
    public class TypeCreatorBuildAction : IBuildAction
    {
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public object? Build(IExecuteStrategy executeStrategy, Type type, params object?[]? arguments)
        {
            executeStrategy = executeStrategy ?? throw new ArgumentNullException(nameof(executeStrategy));

            type = type ?? throw new ArgumentNullException(nameof(type));

            var typeCreator =
                GetMatchingTypeCreator(
                    x => x.CanCreate(executeStrategy.Configuration, executeStrategy.BuildChain, type),
                    executeStrategy.Configuration);

            return Build(typeCreator, type, null, executeStrategy.BuildChain,
                () => typeCreator?.Create(executeStrategy, type, arguments),
                executeStrategy.Log);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public object? Build(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo, params object?[]? arguments)
        {
            executeStrategy = executeStrategy ?? throw new ArgumentNullException(nameof(executeStrategy));

            parameterInfo = parameterInfo ?? throw new ArgumentNullException(nameof(parameterInfo));

            var typeCreator = GetMatchingTypeCreator(
                x => x.CanCreate(executeStrategy.Configuration, executeStrategy.BuildChain, parameterInfo),
                executeStrategy.Configuration);

            return Build(typeCreator, parameterInfo.ParameterType, parameterInfo.Name, executeStrategy.BuildChain,
                () => typeCreator?.Create(executeStrategy, parameterInfo, arguments),
                executeStrategy.Log);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public object? Build(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo, params object?[]? arguments)
        {
            executeStrategy = executeStrategy ?? throw new ArgumentNullException(nameof(executeStrategy));

            propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));

            var typeCreator = GetMatchingTypeCreator(
                x => x.CanCreate(executeStrategy.Configuration, executeStrategy.BuildChain, propertyInfo),
                executeStrategy.Configuration);

            return Build(typeCreator, propertyInfo.PropertyType, propertyInfo.Name, executeStrategy.BuildChain,
                () => typeCreator?.Create(executeStrategy, propertyInfo, arguments),
                executeStrategy.Log);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            Type type)
        {
            buildConfiguration = buildConfiguration ?? throw new ArgumentNullException(nameof(buildConfiguration));

            buildChain = buildChain ?? throw new ArgumentNullException(nameof(buildChain));

            type = type ?? throw new ArgumentNullException(nameof(type));

            return GetBuildCapability(
                x => x.CanCreate(buildConfiguration, buildChain, type),
                x => x.CanPopulate(buildConfiguration, buildChain, type), buildConfiguration);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            ParameterInfo parameterInfo)
        {
            buildConfiguration = buildConfiguration ?? throw new ArgumentNullException(nameof(buildConfiguration));

            buildChain = buildChain ?? throw new ArgumentNullException(nameof(buildChain));

            parameterInfo = parameterInfo ?? throw new ArgumentNullException(nameof(parameterInfo));

            return GetBuildCapability(
                x => x.CanCreate(buildConfiguration, buildChain, parameterInfo),
                x => x.CanPopulate(buildConfiguration, buildChain, parameterInfo), buildConfiguration);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            PropertyInfo propertyInfo)
        {
            buildConfiguration = buildConfiguration ?? throw new ArgumentNullException(nameof(buildConfiguration));

            buildChain = buildChain ?? throw new ArgumentNullException(nameof(buildChain));

            propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));

            return GetBuildCapability(
                x => x.CanCreate(buildConfiguration, buildChain, propertyInfo),
                x => x.CanPopulate(buildConfiguration, buildChain, propertyInfo), buildConfiguration);
        }

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Populate is not supported by this build action.</exception>
        public object Populate(IExecuteStrategy executeStrategy, object instance)
        {
            executeStrategy = executeStrategy ?? throw new ArgumentNullException(nameof(executeStrategy));

            instance = instance ?? throw new ArgumentNullException(nameof(instance));

            var typeCreator = GetMatchingTypeCreator(
                x => x.CanCreate(executeStrategy.Configuration, executeStrategy.BuildChain, instance.GetType()),
                executeStrategy.Configuration);

            if (typeCreator == null)
            {
                return instance;
            }

            return typeCreator.Populate(executeStrategy, instance);
        }

        private static object? Build(ITypeCreator? typeCreator, Type typeToBuild, string? referenceName,
            IBuildChain buildChain,
            Func<object?> createAction, IBuildLog buildLog)
        {
            if (typeCreator == null)
            {
                return null;
            }

            var context = buildChain.Last;
            var creatorType = typeCreator.GetType();

            try
            {
                return createAction();
            }
            catch (BuildException)
            {
                throw;
            }
            catch (Exception ex)
            {
                buildLog.BuildFailure(ex);

                const string messageFormat =
                    "Failed to create value for type {0} using type creator {1}, {2}: {3}{4}{4}At the time of the failure, the build log was:{4}{4}{5}";
                var output = buildLog.Output;
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    messageFormat,
                    typeToBuild.FullName,
                    creatorType.FullName,
                    ex.GetType().Name,
                    ex.Message,
                    Environment.NewLine,
                    output);

                throw new BuildException(message, typeToBuild, referenceName, context, output, ex);
            }
        }

        private static IBuildCapability? GetBuildCapability(Func<ITypeCreator, bool> canCreate,
            Func<ITypeCreator, bool> canPopulate, IBuildConfiguration buildConfiguration)
        {
            var typeCreator = GetMatchingTypeCreator(canCreate, buildConfiguration);

            if (typeCreator == null)
            {
                return null;
            }

            var supportsCreate = canCreate(typeCreator);
            var supportsPopulate = canPopulate(typeCreator);

            return new BuildCapability(typeCreator, supportsCreate, supportsPopulate);
        }

        private static ITypeCreator? GetMatchingTypeCreator(Func<ITypeCreator, bool> canCreate,
            IBuildConfiguration buildConfiguration)
        {
            return buildConfiguration.TypeCreators?.Where(canCreate)
                .OrderByDescending(x => x.Priority).FirstOrDefault();
        }

        /// <inheritdoc />
        public int Priority => 1000;
    }
}
namespace ModelBuilder.BuildActions
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using ModelBuilder.TypeCreators;

    /// <summary>
    /// The <see cref="TypeCreatorBuildAction"/>
    /// class is used to provide a build action that uses a matching <see cref="ITypeCreator"/> to create and/or populate values.
    /// </summary>
    public class TypeCreatorBuildAction : IBuildAction
    {
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public object Build(IExecuteStrategy executeStrategy, Type type, params object[] arguments)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var typeCreator =
                GetMatchingTypeCreator(type, null, executeStrategy.Configuration, executeStrategy.BuildChain);

            return Build(typeCreator, type, null, executeStrategy.BuildChain,
                () => typeCreator?.Create(type, null, executeStrategy, arguments),
                executeStrategy.Log);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public object Build(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo, params object[] arguments)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            var typeCreator = GetMatchingTypeCreator(parameterInfo.ParameterType, parameterInfo.Name,
                executeStrategy.Configuration,
                executeStrategy.BuildChain);

            return Build(typeCreator, parameterInfo.ParameterType, parameterInfo.Name, executeStrategy.BuildChain,
                () => typeCreator?.Create(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy, arguments),
                executeStrategy.Log);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public object Build(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo, params object[] arguments)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            var typeCreator = GetMatchingTypeCreator(propertyInfo.PropertyType, propertyInfo.Name,
                executeStrategy.Configuration,
                executeStrategy.BuildChain);

            return Build(typeCreator, propertyInfo.PropertyType, propertyInfo.Name, executeStrategy.BuildChain,
                () => typeCreator?.Create(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy, arguments),
                executeStrategy.Log);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public BuildCapability GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain, Type type)
        {
            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var typeCreator = GetMatchingTypeCreator(type, null, buildConfiguration, buildChain);

            return GetMatchResult(typeCreator, type, null, buildConfiguration, buildChain);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public BuildCapability GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            ParameterInfo parameterInfo)
        {
            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            var typeCreator = GetMatchingTypeCreator(parameterInfo.ParameterType, parameterInfo.Name,
                buildConfiguration,
                buildChain);

            return GetMatchResult(typeCreator, parameterInfo.ParameterType, parameterInfo.Name, buildConfiguration, buildChain);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public BuildCapability GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            PropertyInfo propertyInfo)
        {
            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            var typeCreator = GetMatchingTypeCreator(propertyInfo.PropertyType, propertyInfo.Name, buildConfiguration,
                buildChain);

            return GetMatchResult(typeCreator, propertyInfo.PropertyType, propertyInfo.Name, buildConfiguration, buildChain);
        }

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Populate is not supported by this build action.</exception>
        public object Populate(IExecuteStrategy executeStrategy, object instance)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var typeCreator = GetMatchingTypeCreator(instance.GetType(), null,
                executeStrategy.Configuration,
                executeStrategy.BuildChain);

            if (typeCreator == null)
            {
                return instance;
            }

            return typeCreator.Populate(instance, executeStrategy);
        }

        private static object Build(ITypeCreator typeCreator, Type typeToBuild, string referenceName,
            IBuildChain buildChain,
            Func<object> createAction, IBuildLog buildLog)
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

        private static ITypeCreator GetMatchingTypeCreator(Type type, string referenceName,
            IBuildConfiguration buildConfiguration,
            IBuildChain buildChain)
        {
            return buildConfiguration.TypeCreators?.Where(x => x.CanCreate(type, referenceName, buildConfiguration, buildChain))
                .OrderByDescending(x => x.Priority).FirstOrDefault();
        }

        private static BuildCapability GetMatchResult(ITypeCreator typeCreator, Type type, string referenceName,
            IBuildConfiguration configuration,
            IBuildChain buildChain)
        {
            if (typeCreator == null)
            {
                return null;
            }

            var canCreate = typeCreator.CanCreate(type, referenceName, configuration, buildChain);
            var canPopulate = typeCreator.CanPopulate(type, referenceName, buildChain);

            return new BuildCapability
            {
                SupportsCreate = canCreate,
                SupportsPopulate = canPopulate,
                AutoPopulate = typeCreator.AutoPopulate,
                AutoDetectConstructor = typeCreator.AutoDetectConstructor,
                ImplementedByType = typeCreator.GetType()
            };
        }

        /// <inheritdoc />
        public int Priority => 1000;
    }
}
namespace ModelBuilder.BuildActions
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using ModelBuilder.ValueGenerators;

    /// <summary>
    ///     The <see cref="ValueGeneratorBuildAction" />
    ///     class is used to build a value from a matching <see cref="IValueGenerator" />.
    /// </summary>
    public class ValueGeneratorBuildAction : IBuildAction
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

            var generator = GetMatchingGenerator(executeStrategy.Configuration, x => x.IsMatch(type, executeStrategy.BuildChain));

            return Build(generator, type, null, executeStrategy.BuildChain,
                () => generator?.Generate(type, executeStrategy),
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

            var generator = GetMatchingGenerator(executeStrategy.Configuration, x => x.IsMatch(parameterInfo, executeStrategy.BuildChain));

            return Build(generator, parameterInfo.ParameterType, parameterInfo.Name, executeStrategy.BuildChain,
                () => generator?.Generate(parameterInfo, executeStrategy),
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

            var generator = GetMatchingGenerator(executeStrategy.Configuration, x => x.IsMatch(propertyInfo, executeStrategy.BuildChain));

            return Build(generator, propertyInfo.PropertyType, propertyInfo.Name, executeStrategy.BuildChain,
                () => generator?.Generate(propertyInfo, executeStrategy),
                executeStrategy.Log);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public BuildCapability GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            Type type)
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

            return GetBuildCapability(buildConfiguration, x => x.IsMatch(type, buildChain));
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

            return GetBuildCapability(buildConfiguration, x => x.IsMatch(parameterInfo, buildChain));
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

            return GetBuildCapability(buildConfiguration, x => x.IsMatch(propertyInfo, buildChain));
        }

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Populate is not supported by this build action.</exception>
        public object Populate(IExecuteStrategy executeStrategy, object instance)
        {
            throw new NotSupportedException();
        }

        private static object Build(IValueGenerator generator, Type typeToBuild, string referenceName,
            IBuildChain buildChain,
            Func<object> createAction, IBuildLog buildLog)
        {
            if (generator == null)
            {
                return null;
            }

            var context = buildChain.Last;
            var generatorType = generator.GetType();

            buildLog.CreatingValue(typeToBuild, generatorType, context);

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
                    "Failed to create value for type {0} using value generator {1}, {2}: {3}{4}{4}At the time of the failure, the build log was:{4}{4}{5}";
                var output = buildLog.Output;
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    messageFormat,
                    typeToBuild.FullName,
                    generatorType.FullName,
                    ex.GetType().Name,
                    ex.Message,
                    Environment.NewLine,
                    output);

                throw new BuildException(message, typeToBuild, referenceName, context, output, ex);
            }
        }

        private static BuildCapability GetBuildCapability(IBuildConfiguration buildConfiguration,
            Func<IValueGenerator, bool> isMatch)
        {
            var generator = GetMatchingGenerator(buildConfiguration, isMatch);

            if (generator == null)
            {
                return null;
            }

            return new BuildCapability
            {
                SupportsCreate = true,
                ImplementedByType = generator.GetType()
            };
        }

        private static IValueGenerator GetMatchingGenerator(IBuildConfiguration buildConfiguration,
            Func<IValueGenerator, bool> isMatch)
        {
            return buildConfiguration.ValueGenerators?.Where(isMatch)
                .OrderByDescending(x => x.Priority).FirstOrDefault();
        }

        /// <inheritdoc />
        public int Priority => 3000;
    }
}
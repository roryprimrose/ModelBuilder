﻿namespace ModelBuilder.BuildActions
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

            var generator = GetMatchingGenerator(type, null, executeStrategy.Configuration, executeStrategy.BuildChain);

            return Build(generator, type, null, executeStrategy.BuildChain,
                () => generator?.Generate(type, null, executeStrategy),
                executeStrategy.Log);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public object Build(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            var generator = GetMatchingGenerator(parameterInfo.ParameterType, parameterInfo.Name,
                executeStrategy.Configuration,
                executeStrategy.BuildChain);

            return Build(generator, parameterInfo.ParameterType, parameterInfo.Name, executeStrategy.BuildChain,
                () => generator?.Generate(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy),
                executeStrategy.Log);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public object Build(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            var generator = GetMatchingGenerator(propertyInfo.PropertyType, propertyInfo.Name,
                executeStrategy.Configuration,
                executeStrategy.BuildChain);

            return Build(generator, propertyInfo.PropertyType, propertyInfo.Name, executeStrategy.BuildChain,
                () => generator?.Generate(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy),
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

            return GetBuildCapability(type, null, buildConfiguration, buildChain);
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

            return GetBuildCapability(parameterInfo.ParameterType, parameterInfo.Name, buildConfiguration, buildChain);
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

            return GetBuildCapability(propertyInfo.PropertyType, propertyInfo.Name, buildConfiguration, buildChain);
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

        private static BuildCapability GetBuildCapability(Type type, string referenceName,
            IBuildConfiguration buildConfiguration, IBuildChain buildChain)
        {
            var generator = GetMatchingGenerator(type, referenceName, buildConfiguration, buildChain);

            if (generator == null)
            {
                return null;
            }

            var isMatch = generator.IsMatch(type, referenceName, buildChain);

            if (isMatch == false)
            {
                return null;
            }

            return new BuildCapability
            {
                SupportsCreate = true,
                ImplementedByType = generator.GetType()
            };
        }

        private static IValueGenerator GetMatchingGenerator(Type type, string referenceName,
            IBuildConfiguration buildConfiguration,
            IBuildChain buildChain)
        {
            return buildConfiguration.ValueGenerators?.Where(x => x.IsMatch(type, referenceName, buildChain))
                .OrderByDescending(x => x.Priority).FirstOrDefault();
        }

        /// <inheritdoc />
        public int Priority => 3000;
    }
}
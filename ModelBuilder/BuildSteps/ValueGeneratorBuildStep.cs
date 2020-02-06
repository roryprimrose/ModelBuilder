namespace ModelBuilder.BuildSteps
{
    using System;
    using System.Linq;
    using System.Reflection;
    using ModelBuilder.ValueGenerators;

    /// <summary>
    ///     The <see cref="ValueGeneratorBuildStep" />
    ///     class is used to build a value from a matching <see cref="IValueGenerator" />.
    /// </summary>
    public class ValueGeneratorBuildStep : IBuildStep
    {
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        public object Build(Type type, IExecuteStrategy executeStrategy)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            var generator = GetMatchingGenerator(type, null, executeStrategy.Configuration, executeStrategy.BuildChain);

            return generator?.Generate(type, null, executeStrategy);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        public object Build(ParameterInfo parameterInfo, IExecuteStrategy executeStrategy)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            var generator = GetMatchingGenerator(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy.Configuration,
                executeStrategy.BuildChain);

            return generator?.Generate(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        public object Build(PropertyInfo propertyInfo, IExecuteStrategy executeStrategy)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            var generator = GetMatchingGenerator(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy.Configuration,
                executeStrategy.BuildChain);

            return generator?.Generate(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(Type type, IBuildConfiguration buildConfiguration, IBuildChain buildChain)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            var generator = GetMatchingGenerator(type, null, buildConfiguration, buildChain);

            if (generator == null)
            {
                return false;
            }

            return generator.IsSupported(type, null, buildChain);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(ParameterInfo parameterInfo, IBuildConfiguration buildConfiguration, IBuildChain buildChain)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            var generator = GetMatchingGenerator(parameterInfo.ParameterType, parameterInfo.Name, buildConfiguration, buildChain);

            if (generator == null)
            {
                return false;
            }

            return generator.IsSupported(parameterInfo.ParameterType, parameterInfo.Name, buildChain);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(PropertyInfo propertyInfo, IBuildConfiguration buildConfiguration, IBuildChain buildChain)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            var generator = GetMatchingGenerator(propertyInfo.PropertyType, propertyInfo.Name, buildConfiguration, buildChain);

            if (generator == null)
            {
                return false;
            }

            return generator.IsSupported(propertyInfo.PropertyType, propertyInfo.Name, buildChain);
        }

        private IValueGenerator GetMatchingGenerator(Type type, string referenceName, IBuildConfiguration buildConfiguration,
            IBuildChain buildChain)
        {
            return buildConfiguration.ValueGenerators?.Where(x => x.IsSupported(type, referenceName, buildChain))
                .OrderByDescending(x => x.Priority).FirstOrDefault();
        }

        /// <inheritdoc />
        public int Priority => 3000;
    }
}
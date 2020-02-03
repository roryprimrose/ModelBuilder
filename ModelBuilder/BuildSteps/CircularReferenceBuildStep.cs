namespace ModelBuilder.BuildSteps
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="CircularReferenceBuildStep" />
    ///     class is used to return a value from the <see cref="IBuildChain" /> that has previously been created.
    /// </summary>
    public class CircularReferenceBuildStep : IBuildStep
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

            return GetCircularReference(executeStrategy.BuildChain, type);
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

            return GetCircularReference(executeStrategy.BuildChain, parameterInfo.ParameterType);
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

            return GetCircularReference(executeStrategy.BuildChain, propertyInfo.PropertyType);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(Type type, IBuildChain buildChain)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            return HasCircularReference(buildChain, type);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(ParameterInfo parameterInfo, IBuildChain buildChain)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            return HasCircularReference(buildChain, parameterInfo.ParameterType);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(PropertyInfo propertyInfo, IBuildChain buildChain)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            return HasCircularReference(buildChain, propertyInfo.PropertyType);
        }

        private static object GetCircularReference(IBuildChain buildChain, Type type)
        {
            return buildChain.FirstOrDefault(x => x.GetType() == type);
        }

        private static bool HasCircularReference(IBuildChain buildChain, Type type)
        {
            var circularReference = GetCircularReference(buildChain, type);

            if (circularReference == null)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public int Priority => int.MaxValue;
    }
}
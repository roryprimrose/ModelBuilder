namespace ModelBuilder.BuildActions
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="CircularReferenceBuildAction" />
    ///     class is used to return a value from the <see cref="IBuildChain" /> that has previously been created.
    /// </summary>
    public class CircularReferenceBuildAction : IBuildAction
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

            return GetCircularReference(executeStrategy, type);
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

            return GetCircularReference(executeStrategy, parameterInfo.ParameterType);
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

            return GetCircularReference(executeStrategy, propertyInfo.PropertyType);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public MatchResult IsMatch(IBuildConfiguration buildConfiguration, IBuildChain buildChain, Type type)
        {
            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return GetMatchResult(buildChain, type);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public MatchResult IsMatch(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            ParameterInfo parameterInfo)
        {
            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            return GetMatchResult(buildChain, parameterInfo.ParameterType);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public MatchResult IsMatch(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            PropertyInfo propertyInfo)
        {
            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return GetMatchResult(buildChain, propertyInfo.PropertyType);
        }

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Populate is not supported by this build action.</exception>
        public object Populate(IExecuteStrategy executeStrategy, object instance)
        {
            throw new NotSupportedException();
        }

        private static object FindItemByType(IBuildChain buildChain, Type type)
        {
            return buildChain.FirstOrDefault(x => x.GetType() == type);
        }

        private static object GetCircularReference(IExecuteStrategy executeStrategy, Type type)
        {
            var item = FindItemByType(executeStrategy.BuildChain, type);

            if (item == null)
            {
                return null;
            }

            executeStrategy.Log.CircularReferenceDetected(type);

            return item;
        }

        private static MatchResult GetMatchResult(IBuildChain buildChain, Type type)
        {
            var circularReference = FindItemByType(buildChain, type);

            if (circularReference == null)
            {
                return MatchResult.NoMatch;
            }

            return new MatchResult {SupportsCreate = true};
        }

        /// <inheritdoc />
        public int Priority => int.MaxValue;
    }
}
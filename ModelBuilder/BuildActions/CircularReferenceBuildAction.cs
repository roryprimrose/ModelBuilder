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
        public object? Build(IExecuteStrategy executeStrategy, Type type, params object?[]? arguments)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var capability = new CircularReferenceCapability();

            return capability.CreateType(executeStrategy, type, arguments);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public object? Build(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo, params object?[]? arguments)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            var capability = new CircularReferenceCapability();

            return capability.CreateParameter(executeStrategy, parameterInfo, arguments);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public object? Build(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo, params object?[]? arguments)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            var capability = new CircularReferenceCapability();

            return capability.CreateProperty(executeStrategy, propertyInfo, arguments);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            Type type)
        {
            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return GetCapability(buildChain, type);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
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

            return GetCapability(buildChain, parameterInfo.ParameterType);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public IBuildCapability? GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
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

            return GetCapability(buildChain, propertyInfo.PropertyType);
        }

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">Populate is not supported by this build action.</exception>
        public object Populate(IExecuteStrategy executeStrategy, object instance)
        {
            throw new NotSupportedException();
        }

        private static IBuildCapability? GetCapability(IBuildChain buildChain, Type type)
        {
            var circularReference = CircularReferenceCapability.FindItemByType(buildChain, type);

            if (circularReference == null)
            {
                return null;
            }

            return new CircularReferenceCapability();
        }

        /// <inheritdoc />
        public int Priority => int.MaxValue;

        private class CircularReferenceCapability : IBuildCapability
        {
            public CircularReferenceCapability()
            {
                AutoPopulate = false;
                ImplementedByType = GetType();
                SupportsCreate = true;
                SupportsPopulate = false;
            }

            public static object FindItemByType(IBuildChain buildChain, Type type)
            {
                return buildChain.FirstOrDefault(x => x.GetType() == type);
            }

            public object? CreateParameter(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo,
                object?[]? args)
            {
                return CreateType(executeStrategy, parameterInfo.ParameterType, args);
            }

            public object? CreateProperty(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo, object?[]? args)
            {
                return CreateType(executeStrategy, propertyInfo.PropertyType, args);
            }

            public object? CreateType(IExecuteStrategy executeStrategy, Type targetType, object?[]? args)
            {
                return GetCircularReference(executeStrategy, targetType);
            }

            public object Populate(IExecuteStrategy executeStrategy, object instance)
            {
                throw new NotSupportedException();
            }

            private static object? GetCircularReference(IExecuteStrategy executeStrategy, Type type)
            {
                var item = FindItemByType(executeStrategy.BuildChain, type);

                if (item == null)
                {
                    return null;
                }

                executeStrategy.Log.CircularReferenceDetected(type);

                return item;
            }

            public bool AutoPopulate { get; }
            public Type ImplementedByType { get; }
            public bool SupportsCreate { get; }
            public bool SupportsPopulate { get; }
        }
    }
}
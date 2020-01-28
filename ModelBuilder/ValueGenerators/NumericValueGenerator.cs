namespace ModelBuilder.ValueGenerators
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="NumericValueGenerator" />
    ///     class is used to generate random numeric values.
    /// </summary>
    public class NumericValueGenerator : IValueGenerator
    {
        private static readonly IRandomGenerator _random = new RandomGenerator();

        /// <inheritdoc />
        public virtual object Generate(PropertyInfo propertyInfo, IExecuteStrategy executeStrategy)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            var type = propertyInfo.PropertyType;
            var name = propertyInfo.Name;

            return Generate(type, name, executeStrategy);
        }

        /// <inheritdoc />
        public virtual object Generate(ParameterInfo parameterInfo, IExecuteStrategy executeStrategy)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            var type = parameterInfo.ParameterType;
            var name = parameterInfo.Name;

            return Generate(type, name, executeStrategy);
        }

        /// <inheritdoc />
        public virtual object Generate(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            var generateType = type;

            if (generateType.IsNullable())
            {
                // Allow for a 10% the chance that this might be null
                var range = Generator.NextValue(0, 100000);

                if (range < 10000)
                {
                    return null;
                }

                // Hijack the type to generator so we can continue with the normal code pointed at the correct type to generate
                generateType = type.GetGenericArguments()[0];
            }

            var context = executeStrategy?.BuildChain?.Last;
            var min = GetMinimum(generateType, referenceName, context);
            var max = GetMaximum(generateType, referenceName, context);

            return Generator.NextValue(generateType, min, max);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public virtual bool IsSupported(PropertyInfo propertyInfo, IBuildChain buildChain)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return IsSupported(propertyInfo.PropertyType, propertyInfo.Name, buildChain);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public virtual bool IsSupported(ParameterInfo parameterInfo, IBuildChain buildChain)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            return IsSupported(parameterInfo.ParameterType, parameterInfo.Name, buildChain);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        public virtual bool IsSupported(Type type, string referenceName, IBuildChain buildChain)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            if (type.IsNullable())
            {
                // Get the internal type
                var internalType = type.GetGenericArguments()[0];

                return Generator.IsSupported(internalType);
            }

            return Generator.IsSupported(type);
        }

        /// <summary>
        ///     Returns the maximum value for the specified generation target.
        /// </summary>
        /// <param name="type">The type of value to generate.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name the value is intended for.</param>
        /// <param name="context">The possible context object the value is being created for.</param>
        /// <returns>The maximum value allowed to be generated.</returns>
        protected virtual object GetMaximum(Type type, string referenceName, object context)
        {
            return Generator.GetMax(type);
        }

        /// <summary>
        ///     Returns the minimum value for the specified generation target.
        /// </summary>
        /// <param name="type">The type of value to generate.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name the value is intended for.</param>
        /// <param name="context">The possible context object the value is being created for.</param>
        /// <returns>The minimum value allowed to be generated.</returns>
        protected virtual object GetMinimum(Type type, string referenceName, object context)
        {
            return Generator.GetMin(type);
        }

        /// <inheritdoc />
        public virtual int Priority { get; } = int.MinValue;

        /// <summary>
        ///     Gets the random generator for this instance.
        /// </summary>
        protected virtual IRandomGenerator Generator { get; } = _random;
    }
}
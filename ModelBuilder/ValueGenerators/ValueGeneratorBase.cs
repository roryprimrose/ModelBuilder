namespace ModelBuilder.ValueGenerators
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="ValueGeneratorBase" />
    ///     class provides the base implementation for generating values.
    /// </summary>
    public abstract class ValueGeneratorBase : IValueGenerator
    {
        private static readonly IRandomGenerator _random = new RandomGenerator();

        /// <inheritdoc />
        public virtual object Generate(Type type, IExecuteStrategy executeStrategy)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            return Generate(type, null, executeStrategy);
        }

        /// <inheritdoc />
        public virtual object Generate(PropertyInfo propertyInfo, IExecuteStrategy executeStrategy)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
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

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            var type = parameterInfo.ParameterType;
            var name = parameterInfo.Name;

            return Generate(type, name, executeStrategy);
        }

        /// <inheritdoc />
        public virtual bool IsSupported(Type type, IBuildChain buildChain)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            return IsSupported(type, null, buildChain);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public virtual bool IsSupported(PropertyInfo propertyInfo, IBuildChain buildChain)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
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

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            return IsSupported(parameterInfo.ParameterType, parameterInfo.Name, buildChain);
        }

        /// <summary>
        ///     Generates a new value of the specified type.
        /// </summary>
        /// <param name="type">The type of value to generate.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name the value is intended for.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <returns>A new value of the type.</returns>
        protected abstract object Generate(Type type, string referenceName, IExecuteStrategy executeStrategy);

        /// <summary>
        ///     Returns whether the specified type is supported by this generator.
        /// </summary>
        /// <param name="type">The type to evaluate.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name the value is intended for.</param>
        /// <param name="buildChain">The chain of instances built up to this point.</param>
        /// <returns><c>true</c> if the type is supported; otherwise <c>false</c>.</returns>
        protected abstract bool IsSupported(Type type, string referenceName, IBuildChain buildChain);

        /// <inheritdoc />
        public virtual int Priority { get; } = int.MinValue;

        /// <summary>
        ///     Gets the random generator for this instance.
        /// </summary>
        protected virtual IRandomGenerator Generator { get; } = _random;
    }
}
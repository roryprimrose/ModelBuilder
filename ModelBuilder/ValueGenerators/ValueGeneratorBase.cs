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
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        public object Generate(Type type, IExecuteStrategy executeStrategy)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return Generate(type, null, executeStrategy);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public virtual bool IsMatch(PropertyInfo propertyInfo, IBuildChain buildChain)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            return IsMatch(propertyInfo.PropertyType, propertyInfo.Name, buildChain);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public virtual bool IsMatch(ParameterInfo parameterInfo, IBuildChain buildChain)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            return IsMatch(parameterInfo.ParameterType, parameterInfo.Name, buildChain);
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

            return IsMatch(type, null, buildChain);
        }

        /// <summary>
        ///     Generates a new value of the specified type.
        /// </summary>
        /// <param name="type">The type of value to generate.</param>
        /// <param name="referenceName">The name of the item to generate.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <returns>A new value of the type.</returns>
        protected abstract object Generate(Type type, string referenceName, IExecuteStrategy executeStrategy);

        /// <summary>
        ///     Returns whether the specified type and name matches this generator.
        /// </summary>
        /// <param name="type">The type of value to generate.</param>
        /// <param name="referenceName">The name of the item to generate.</param>
        /// <param name="buildChain">The build chain.</param>
        /// <returns><c>true</c> if the type matches this generator; otherwise <c>false</c>.</returns>
        protected abstract bool IsMatch(Type type, string referenceName, IBuildChain buildChain);

        /// <inheritdoc />
        public virtual int Priority { get; } = int.MinValue;

        /// <summary>
        ///     Gets the random generator for this instance.
        /// </summary>
        protected virtual IRandomGenerator Generator { get; } = _random;
    }
}
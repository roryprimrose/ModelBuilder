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
        public object Generate(IExecuteStrategy executeStrategy, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return Generate(executeStrategy, type, null);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        public virtual object Generate(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo)
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

            return Generate(executeStrategy, type, name);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        public virtual object Generate(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo)
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

            return Generate(executeStrategy, type, name);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public virtual bool IsMatch(IBuildChain buildChain, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            return IsMatch(buildChain, propertyInfo.PropertyType, propertyInfo.Name);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public virtual bool IsMatch(IBuildChain buildChain, ParameterInfo parameterInfo)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            return IsMatch(buildChain, parameterInfo.ParameterType, parameterInfo.Name);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="buildChain" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(IBuildChain buildChain, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (buildChain == null)
            {
                throw new ArgumentNullException(nameof(buildChain));
            }

            return IsMatch(buildChain, type, null);
        }

        /// <summary>
        ///     Generates a new value of the specified type.
        /// </summary>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <param name="type">The type of value to generate.</param>
        /// <param name="referenceName">The name of the item to generate.</param>
        /// <returns>A new value of the type.</returns>
        protected abstract object Generate(IExecuteStrategy executeStrategy, Type type, string referenceName);

        /// <summary>
        ///     Returns whether the specified type and name matches this generator.
        /// </summary>
        /// <param name="buildChain">The build chain.</param>
        /// <param name="type">The type of value to generate.</param>
        /// <param name="referenceName">The name of the item to generate.</param>
        /// <returns><c>true</c> if the type matches this generator; otherwise <c>false</c>.</returns>
        protected abstract bool IsMatch(IBuildChain buildChain, Type type, string referenceName);

        /// <inheritdoc />
        public virtual int Priority { get; } = int.MinValue;

        /// <summary>
        ///     Gets the random generator for this instance.
        /// </summary>
        protected virtual IRandomGenerator Generator { get; } = _random;
    }
}
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
        public abstract object Generate(Type type, string referenceName, IExecuteStrategy executeStrategy);

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
        public abstract bool IsMatch(Type type, string referenceName, IBuildChain buildChain);

        /// <inheritdoc />
        public virtual int Priority { get; } = int.MinValue;

        /// <summary>
        ///     Gets the random generator for this instance.
        /// </summary>
        protected virtual IRandomGenerator Generator { get; } = _random;
    }
}
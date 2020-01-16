namespace ModelBuilder.ValueGenerators
{
    using System;
    using System.Globalization;
    using ModelBuilder.Properties;

    /// <summary>
    ///     The <see cref="ValueGeneratorBase" />
    ///     class provides the base implementation for generating values.
    /// </summary>
    public abstract class ValueGeneratorBase : IValueGenerator
    {
        private static readonly IRandomGenerator _random = new RandomGenerator();

        /// <inheritdoc />
        public virtual object Generate(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (executeStrategy.BuildChain == null)
            {
                throw new InvalidOperationException(Resources.ExecuteStrategy_NoBuildChain);
            }

            VerifyGenerateRequest(type, referenceName, executeStrategy);

            return GenerateValue(type, referenceName, executeStrategy);
        }

        /// <inheritdoc />
        public abstract bool IsSupported(Type type, string referenceName, IBuildChain buildChain);

        /// <summary>
        ///     Generates a new value with the provided context.
        /// </summary>
        /// <param name="type">The type of value to generate.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name the value is intended for.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <returns>A new value of the type.</returns>
        protected abstract object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy);

        /// <summary>
        ///     Verifies that the minimum required information has been provided in order to generate a value.
        /// </summary>
        /// <param name="type">The type of value to generate.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name this value is intended for.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is null.</exception>
        /// <exception cref="NotSupportedException">This generator does not support creating the requested value.</exception>
        protected virtual void VerifyGenerateRequest(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            // Calculate the build chain just once
            var buildChain = executeStrategy.BuildChain;

            if (buildChain == null)
            {
                throw new InvalidOperationException(Resources.ExecuteStrategy_NoBuildChain);
            }

            if (IsSupported(type, referenceName, buildChain) == false)
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Error_GenerationNotSupportedFormat,
                    GetType().FullName,
                    type.FullName,
                    referenceName ?? "<null>");

                throw new NotSupportedException(message);
            }
        }

        /// <inheritdoc />
        public virtual int Priority { get; } = int.MinValue;

        /// <summary>
        ///     Gets the random generator for this instance.
        /// </summary>
        protected virtual IRandomGenerator Generator { get; } = _random;
    }
}
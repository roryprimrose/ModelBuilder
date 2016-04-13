using System;
using System.Globalization;
using ModelBuilder.Properties;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="ValueGeneratorBase"/>
    /// class provides the base implementation for generating values.
    /// </summary>
    public abstract class ValueGeneratorBase : IValueGenerator
    {
        private static readonly IRandomGenerator _random;

        static ValueGeneratorBase()
        {
            // Define a single random generator for the app domain to avoid collisions within a short space of time
            _random = new RandomGenerator();
        }

        /// <inheritdoc />
        public abstract object Generate(Type type, string referenceName, object context);

        /// <inheritdoc />
        public abstract bool IsSupported(Type type, string referenceName, object context);

        /// <summary>
        /// Verifies that the minimum required information has been provided in order to generate a value.
        /// </summary>
        /// <param name="type">The type of value to generate.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name this value is intended for.</param>
        /// <param name="context">The possible context object this value is being created for.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is null.</exception>
        /// <exception cref="NotSupportedException">This generator does not support creating the requested value.</exception>
        protected virtual void VerifyGenerateRequest(Type type, string referenceName, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (IsSupported(type, referenceName, context) == false)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Resources.Error_TypeNotSupportedFormat,
                    GetType().FullName, type.FullName);

                throw new NotSupportedException(message);
            }
        }

        /// <inheritdoc />
        public virtual int Priority { get; } = int.MinValue;

        /// <summary>
        /// Gets the random generator for this instance.
        /// </summary>
        protected virtual IRandomGenerator Generator { get; } = _random;
    }
}
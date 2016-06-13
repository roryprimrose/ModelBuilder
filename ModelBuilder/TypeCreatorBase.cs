using System;
using System.Globalization;
using ModelBuilder.Properties;

namespace ModelBuilder
{
    using System.Collections.Generic;

    /// <summary>
    /// The <see cref="TypeCreatorBase"/>
    /// class is used to provide the common implementation of a type creator.
    /// </summary>
    public abstract class TypeCreatorBase : ITypeCreator
    {
        private static readonly IRandomGenerator _random = new RandomGenerator();

        /// <inheritdoc />
        public abstract object Create(Type type, string referenceName, LinkedList<object> buildChain, params object[] args);

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is null.</exception>
        public virtual bool IsSupported(Type type, string referenceName, LinkedList<object> buildChain)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsInterface)
            {
                return false;
            }

            if (type.IsAbstract)
            {
                return false;
            }

            if (type.IsValueType)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public virtual object Populate(object instance, IExecuteStrategy executeStrategy)
        {
            // The default will be to not do any additional population of the instance
            return instance;
        }

        /// <summary>
        /// Verifies that the minimum required information has been provided in order to create an instance.
        /// </summary>
        /// <param name="type">The type of value to generate.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name this value is intended for.</param>
        /// <param name="buildChain">The chain of instances built up to this point.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is null.</exception>
        /// <exception cref="NotSupportedException">This generator does not support creating the requested value.</exception>
        protected virtual void VerifyCreateRequest(Type type, string referenceName, LinkedList<object> buildChain)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (IsSupported(type, referenceName, buildChain) == false)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Resources.Error_TypeNotSupportedFormat,
                    GetType().FullName, type.FullName);

                throw new NotSupportedException(message);
            }
        }

        /// <inheritdoc />
        public virtual bool AutoDetectConstructor => true;

        /// <inheritdoc />
        public virtual bool AutoPopulate => true;

        /// <inheritdoc />
        public virtual int Priority { get; } = 0;


        /// <summary>
        /// Gets the random generator for this instance.
        /// </summary>
        protected virtual IRandomGenerator Generator { get; } = _random;
    }
}
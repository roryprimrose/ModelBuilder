using System;
using System.Globalization;
using ModelBuilder.Properties;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="TypeCreatorBase"/>
    /// class is used to provide the common implementation of a type creator.
    /// </summary>
    public abstract class TypeCreatorBase : ITypeCreator
    {
        /// <inheritdoc />
        public abstract object Create(Type type, string referenceName, object context, params object[] args);

        /// <inheritdoc />
        public virtual bool IsSupported(Type type, string referenceName, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsAbstract)
            {
                return false;
            }

            if (type.IsInterface)
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
        /// <param name="context">The possible context object this value is being created for.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is null.</exception>
        /// <exception cref="NotSupportedException">This generator does not support creating the requested value.</exception>
        protected virtual void VerifyCreateRequest(Type type, string referenceName, object context)
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
        public virtual bool AutoDetectConstructor => true;

        /// <inheritdoc />
        public virtual bool AutoPopulate => true;

        /// <inheritdoc />
        public virtual int Priority { get; } = 0;
    }
}
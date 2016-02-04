using System;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="BaseTypeCreator"/>
    /// class is used to provide the common implementation of a type creator.
    /// </summary>
    public abstract class BaseTypeCreator : ITypeCreator
    {
        /// <inheritdoc />
        public abstract object Create(Type type, params object[] args);

        /// <inheritdoc />
        public virtual bool IsSupported(Type type)
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
        public virtual bool AutoDetectConstructor => true;
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ModelBuilder.Properties;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="EnumerableTypeCreator"/>
    /// class is used to create an instance from an <see cref="IEnumerable{T}"/> type.
    /// </summary>
    public class EnumerableTypeCreator : ITypeCreator
    {
        /// <inheritdoc />
        public object Create(Type type, params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (IsSupported(type) == false)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Resources.Error_TypeNotSupportedFormat,
                    GetType().FullName, type.FullName);

                throw new NotSupportedException(message);
            }

            var listGenericType = typeof (List<string>).GetGenericTypeDefinition();
            var typeArgument = type.GenericTypeArguments.Single();
            var genericType = listGenericType.MakeGenericType(typeArgument);

            return Activator.CreateInstance(genericType);
        }

        /// <inheritdoc />
        public bool IsSupported(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsGenericType == false)
            {
                return false;
            }

            var expectedGenericType = typeof (IEnumerable<string>).GetGenericTypeDefinition();

            if (type.GetGenericTypeDefinition() == expectedGenericType)
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public bool AutoDetectConstructor => false;
    }
}
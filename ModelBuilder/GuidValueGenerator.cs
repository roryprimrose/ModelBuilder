using System;
using System.Globalization;
using ModelBuilder.Properties;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="BooleanValueGenerator"/>
    /// class is used to generate <see cref="Guid"/> values.
    /// </summary>
    public class GuidValueGenerator : IValueGenerator
    {
        /// <inheritdoc />
        public object Generate(Type type)
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

            return Guid.NewGuid();
        }

        /// <inheritdoc />
        public bool IsSupported(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsValueType == false)
            {
                return false;
            }

            if (type == typeof (Guid))
            {
                return true;
            }

            return false;
        }
    }
}
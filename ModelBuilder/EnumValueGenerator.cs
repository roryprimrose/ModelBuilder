using System;
using System.Globalization;
using ModelBuilder.Properties;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="BooleanValueGenerator"/>
    /// class is used to generate random enum values.
    /// </summary>
    public class EnumValueGenerator : IValueGenerator
    {
        private static readonly Random _random = new Random(Environment.TickCount);

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

            var values = Enum.GetValues(type);
            var valueIndex = _random.Next(0, values.Length - 1);

            return values.GetValue(valueIndex);
        }

        /// <inheritdoc />
        public bool IsSupported(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsEnum)
            {
                return true;
            }

            return false;
        }
    }
}
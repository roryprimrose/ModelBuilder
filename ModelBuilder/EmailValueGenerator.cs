using System;
using System.Globalization;
using ModelBuilder.Properties;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="EmailValueGenerator"/>
    /// class is used to generate strings that should represent an email.
    /// </summary>
    public class EmailValueGenerator : StringValueGenerator
    {
        /// <inheritdoc />
        public override object Generate(Type type, string referenceName, object context)
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

            var firstPart = Guid.NewGuid().ToString("N").Substring(0, 5);
            var secondPart = Guid.NewGuid().ToString("N").Substring(0, 8);
            var domainPart = Guid.NewGuid().ToString("N").Substring(0, 10);

            return firstPart + "." + secondPart + "@" + domainPart + ".com";
        }

        /// <inheritdoc />
        public override bool IsSupported(Type type, string referenceName, object context)
        {
            var baseSupported = base.IsSupported(type, referenceName, context);

            if (baseSupported == false)
            {
                return false;
            }

            if (string.IsNullOrEmpty(referenceName))
            {
                return false;
            }

            if (referenceName.IndexOf("email", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return true;
            }

            if (referenceName.IndexOf("emailaddress", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
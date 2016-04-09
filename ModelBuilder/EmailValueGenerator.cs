using System;

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
            VerifyGenerateRequest(type, referenceName, context);

            var firstPartLength = Generator.Next(2, 5);
            var firstPart = Guid.NewGuid().ToString("N").Substring(0, firstPartLength);
            var secondPartLength = Generator.Next(3, 8);
            var secondPart = Guid.NewGuid().ToString("N").Substring(0, secondPartLength);
            var thirdPartLength = Generator.Next(3, 10);
            var domainPart = Guid.NewGuid().ToString("N").Substring(0, thirdPartLength);

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

            return false;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
using System;
using System.Net;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="IPAddressValueGenerator"/>
    /// class is used to generate IP Address values.
    /// </summary>
    public class IPAddressValueGenerator : ValueGeneratorBase
    {
        /// <inheritdoc />
        public override object Generate(Type type, string referenceName, object context)
        {
            VerifyGenerateRequest(type, referenceName, context);

            var firstPart = Generator.NextByte();
            var secondPart = Generator.NextByte();
            var thirdPart = Generator.NextByte();
            var fourthPart = Generator.NextByte();

            if (type == typeof (IPAddress))
            {
                var parts = new[] {firstPart, secondPart, thirdPart, fourthPart};

                return new IPAddress(parts);
            }

            return firstPart + "." + secondPart + "." + thirdPart + "." + fourthPart;
        }

        /// <inheritdoc />
        public override bool IsSupported(Type type, string referenceName, object context)
        {
            if (type == typeof (IPAddress))
            {
                return true;
            }

            if (string.IsNullOrEmpty(referenceName))
            {
                return false;
            }

            if (referenceName.IndexOf("ipaddress", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
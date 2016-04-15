﻿using System;
using System.Globalization;
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

            var buffer = new byte[4];

            Generator.Next(buffer);

            if (type == typeof (IPAddress))
            {
                return new IPAddress(buffer);
            }

            const string addressFormat = "{0}.{1}.{2}.{3}";

            var address = string.Format(CultureInfo.InvariantCulture, addressFormat, buffer[0], buffer[1], buffer[2],
                buffer[3]);

            return address;
        }

        /// <inheritdoc />
        public override bool IsSupported(Type type, string referenceName, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type == typeof (IPAddress))
            {
                return true;
            }

            if (type != typeof (string))
            {
                return false;
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
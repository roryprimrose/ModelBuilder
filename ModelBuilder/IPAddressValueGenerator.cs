﻿namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;

    /// <summary>
    ///     The <see cref="IPAddressValueGenerator" />
    ///     class is used to generate IP Address values.
    /// </summary>
    public class IPAddressValueGenerator : ValueGeneratorBase
    {
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is null.</exception>
        public override bool IsSupported(Type type, string referenceName, LinkedList<object> buildChain)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type == typeof(IPAddress))
            {
                return true;
            }

            if (type != typeof(string))
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
        protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            var buffer = new byte[4];

            Generator.NextValue(buffer);

            if (type == typeof(IPAddress))
            {
                return new IPAddress(buffer);
            }

            const string addressFormat = "{0}.{1}.{2}.{3}";

            var address = string.Format(
                CultureInfo.InvariantCulture,
                addressFormat,
                buffer[0],
                buffer[1],
                buffer[2],
                buffer[3]);

            return address;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
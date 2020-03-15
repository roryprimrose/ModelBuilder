﻿namespace ModelBuilder.ValueGenerators
{
    using System;
    using ModelBuilder.Data;

    /// <summary>
    ///     The <see cref="UriValueGenerator" />
    ///     class is used to generate random uri values.
    /// </summary>
    public class UriValueGenerator : ValueGeneratorBase
    {
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        protected override bool IsMatch(IBuildChain buildChain, Type type, string referenceName)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type == typeof(Uri))
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

            if (referenceName.IndexOf("url", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return true;
            }

            if (referenceName.IndexOf("uri", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected override object Generate(IExecuteStrategy executeStrategy, Type type, string referenceName)
        {
            var domain = TestData.Domains.Next();
            var value = "https://www." + domain;

            if (type == typeof(Uri))
            {
                return new Uri(value);
            }

            return value;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
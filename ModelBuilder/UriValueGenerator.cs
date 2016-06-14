﻿namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using ModelBuilder.Data;

    /// <summary>
    /// The <see cref="UriValueGenerator"/>
    /// class is used to generate random uri values.
    /// </summary>
    public class UriValueGenerator : ValueGeneratorBase
    {
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is null.</exception>
        public override bool IsSupported(Type type, string referenceName, LinkedList<object> buildChain)
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
        protected override object GenerateValue(Type type, string referenceName, LinkedList<object> buildChain)
        {
            var person = TestData.NextPerson();
            var value = "https://www." + person.Domain;

            if (type == typeof(Uri))
            {
                return new Uri(value);
            }

            return value;
        }

        /// <inheritdoc />
        public override int Priority
        {
            get;
        } = 1000;
    }
}
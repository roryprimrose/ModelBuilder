namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The <see cref="StringValueGenerator"/>
    /// class is used to generate random <see cref="string"/> values.
    /// </summary>
    public class StringValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringValueGenerator"/> class.
        /// </summary>
        public StringValueGenerator() : base(typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, LinkedList<object> buildChain)
        {
            return Guid.NewGuid().ToString();
        }
    }
}
namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using ModelBuilder.Data;

    /// <summary>
    /// The <see cref="SuburbValueGenerator"/>
    /// class is used to generate random suburb values.
    /// </summary>
    public class SuburbValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuburbValueGenerator"/> class.
        /// </summary>
        public SuburbValueGenerator()
            : base(new Regex("Suburb", RegexOptions.Compiled | RegexOptions.IgnoreCase), typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, LinkedList<object> buildChain)
        {
            var person = TestData.NextPerson();

            // Many suburbs are named after people so we will just take a random last name
            return person.LastName;
        }

        /// <inheritdoc />
        public override int Priority
        {
            get;
        } = 1000;
    }
}
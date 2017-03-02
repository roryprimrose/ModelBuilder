namespace ModelBuilder
{
    using System;
    using System.Text.RegularExpressions;
    using ModelBuilder.Data;

    /// <summary>
    ///     The <see cref="SuburbValueGenerator" />
    ///     class is used to generate random suburb values.
    /// </summary>
    public class SuburbValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SuburbValueGenerator" /> class.
        /// </summary>
        public SuburbValueGenerator()
            : base(new Regex("Suburb", RegexOptions.IgnoreCase), typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            var person = TestData.NextPerson();

            // Many suburbs are named after people so we will just take a random last name
            return person.LastName;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
using System;
using System.Text.RegularExpressions;
using ModelBuilder.Data;

namespace ModelBuilder
{
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
        protected override object GenerateValue(Type type, string referenceName, object context)
        {
            var index = Generator.NextValue(0, TestData.People.Count - 1);
            var person = TestData.People[index];

            // Many suburbs are named after people so we will just take a random last name
            return person.LastName;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
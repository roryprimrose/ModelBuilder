using System;
using System.Text.RegularExpressions;
using ModelBuilder.Data;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="CountryValueGenerator"/>
    /// class is used to generate random country values.
    /// </summary>
    public class CountryValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CountryValueGenerator"/> class.
        /// </summary>
        public CountryValueGenerator()
            : base(new Regex("Country", RegexOptions.Compiled | RegexOptions.IgnoreCase), typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, object context)
        {
            var person = TestData.NextPerson();

            return person.Country;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
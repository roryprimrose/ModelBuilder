using System;
using System.Text.RegularExpressions;
using ModelBuilder.Data;

namespace ModelBuilder
{
    public class CountryValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CountryValueGenerator"/> class.
        /// </summary>
        public CountryValueGenerator()
            : base(new Regex("Country", RegexOptions.Compiled | RegexOptions.IgnoreCase), typeof(string))
        {
        }

        protected override object GenerateValue(Type type, string referenceName, object context)
        {
            var index = Generator.Next(0, TestData.People.Count - 1);
            var person = TestData.People[index];

            return person.Country;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
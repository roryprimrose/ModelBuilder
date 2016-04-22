using System;
using System.Text.RegularExpressions;
using ModelBuilder.Data;

namespace ModelBuilder
{
    public class CityValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CityValueGenerator"/> class.
        /// </summary>
        public CityValueGenerator()
            : base(new Regex("City", RegexOptions.Compiled | RegexOptions.IgnoreCase), typeof(string))
        {
        }

        protected override object GenerateValue(Type type, string referenceName, object context)
        {
            var index = Generator.Next(0, TestData.People.Count - 1);
            var person = TestData.People[index];

            return person.City;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
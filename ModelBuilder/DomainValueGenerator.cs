using System;
using System.Text.RegularExpressions;
using ModelBuilder.Data;

namespace ModelBuilder
{
    public class DomainValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainValueGenerator"/> class.
        /// </summary>
        public DomainValueGenerator()
            : base(new Regex("Domain", RegexOptions.Compiled | RegexOptions.IgnoreCase), typeof(string))
        {
        }

        protected override object GenerateValue(Type type, string referenceName, object context)
        {
            var index = Generator.Next(0, TestData.People.Count - 1);
            var person = TestData.People[index];

            return person.Domain;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
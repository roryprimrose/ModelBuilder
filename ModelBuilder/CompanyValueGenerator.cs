namespace ModelBuilder
{
    using System;
    using System.Text.RegularExpressions;
    using ModelBuilder.Data;

    /// <summary>
    ///     The <see cref="CompanyValueGenerator" />
    ///     class is used to generate random company name values.
    /// </summary>
    public class CompanyValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyValueGenerator" /> class.
        /// </summary>
        public CompanyValueGenerator()
            : base(new Regex("Company", RegexOptions.Compiled | RegexOptions.IgnoreCase), typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            var person = TestData.NextPerson();

            return person.Company;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
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
        private static readonly Regex _matchNameExpression = new Regex("Company", RegexOptions.IgnoreCase);

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyValueGenerator" /> class.
        /// </summary>
        public CompanyValueGenerator() : base(_matchNameExpression, typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            return TestData.Companies.Next();
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
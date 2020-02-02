namespace ModelBuilder.ValueGenerators
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
        private static readonly Regex _matchNameExpression = new Regex("Suburb", RegexOptions.IgnoreCase);

        /// <summary>
        ///     Initializes a new instance of the <see cref="SuburbValueGenerator" /> class.
        /// </summary>
        public SuburbValueGenerator() : base(_matchNameExpression, typeof(string))
        {
        }

        /// <inheritdoc />
        public override object Generate(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            // Some suburbs are named after people so we will just take a random last name
            return TestData.LastNames.Next();
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
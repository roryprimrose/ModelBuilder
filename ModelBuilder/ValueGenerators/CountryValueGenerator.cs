namespace ModelBuilder.ValueGenerators
{
    using System;
    using ModelBuilder.Data;

    /// <summary>
    ///     The <see cref="CountryValueGenerator" />
    ///     class is used to generate random country values.
    /// </summary>
    public class CountryValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CountryValueGenerator" /> class.
        /// </summary>
        public CountryValueGenerator() : base(NameExpression.Country, typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object Generate(IExecuteStrategy executeStrategy, Type type, string referenceName)
        {
            var location = TestData.Locations.Next();

            return location.Country;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
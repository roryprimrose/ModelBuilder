namespace ModelBuilder.ValueGenerators
{
    using System;
    using ModelBuilder.Data;

    /// <summary>
    ///     The <see cref="LastNameValueGenerator" />
    ///     class is used to generate random last name values.
    /// </summary>
    public class LastNameValueGenerator : RelativeValueGenerator
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LastNameValueGenerator" />.
        /// </summary>
        public LastNameValueGenerator() : base(NameExpression.LastName, typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName)
        {
            return TestData.LastNames.Next();
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
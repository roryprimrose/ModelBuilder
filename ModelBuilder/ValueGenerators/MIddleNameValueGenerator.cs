namespace ModelBuilder.ValueGenerators
{
    using System;
    using ModelBuilder.Data;

    /// <summary>
    ///     The <see cref="MiddleNameValueGenerator" />
    ///     class is used to generate random middle name values.
    /// </summary>
    public class MiddleNameValueGenerator : RelativeValueGenerator
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MiddleNameValueGenerator" />.
        /// </summary>
        public MiddleNameValueGenerator() : base(NameExpression.MiddleName, typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName)
        {
            if (IsMale(executeStrategy))
            {
                // Use a male first name
                return TestData.MaleNames.Next();
            }

            // Use a female name
            return TestData.FemaleNames.Next();
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
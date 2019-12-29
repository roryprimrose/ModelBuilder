namespace ModelBuilder
{
    using System;
    using ModelBuilder.Data;

    /// <summary>
    ///     The <see cref="FirstNameValueGenerator" />
    ///     class is used to generate random first name values.
    /// </summary>
    public class FirstNameValueGenerator : RelativeValueGenerator
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FirstNameValueGenerator" />.
        /// </summary>
        public FirstNameValueGenerator() : base(PropertyExpression.FirstName, typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
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
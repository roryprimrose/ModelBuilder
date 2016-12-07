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
            var context = executeStrategy.BuildChain?.Last?.Value;
            var gender = GetValue<string>(PropertyExpression.Gender, context);

            if (string.Equals(gender, "male", StringComparison.OrdinalIgnoreCase))
            {
                // Use a male first name
                var male = TestData.NextMale();

                return male.FirstName;
            }

            // Use a female name
            var female = TestData.NextFemale();

            return female.FirstName;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
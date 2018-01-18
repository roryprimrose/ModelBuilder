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
            var context = executeStrategy?.BuildChain?.Last?.Value;
            var gender = GetValue<string>(PropertyExpression.Gender, context);
            bool isMale;

            if (string.IsNullOrWhiteSpace(gender))
            {
                // Randomly assign a gender so that we can pick from a gender data set rather than limiting to a specific one
                var nextValue = Generator.NextValue(0, 1);

                if (nextValue == 0)
                {
                    isMale = false;
                }
                else
                {
                    isMale = true;
                }
            }
            else
            {
                isMale = string.Equals(gender, "male", StringComparison.OrdinalIgnoreCase);
            }

            if (isMale)
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
namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using ModelBuilder.Data;

    /// <summary>
    /// The <see cref="FirstNameValueGenerator"/>
    /// class is used to generate random first name values.
    /// </summary>
    public class FirstNameValueGenerator : RelativeValueGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValueGenerator"/>.
        /// </summary>
        public FirstNameValueGenerator() : base(PropertyExpression.FirstName, PropertyExpression.Gender, typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, LinkedList<object> buildChain)
        {
            var context = buildChain.Last.Value;
            var gender = GetSourceValue<string>(context);

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
        public override int Priority
        {
            get;
        } = 1000;
    }
}
namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using ModelBuilder.Data;

    /// <summary>
    /// The <see cref="LastNameValueGenerator"/>
    /// class is used to generate random last name values.
    /// </summary>
    public class LastNameValueGenerator : RelativeValueGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValueGenerator"/>.
        /// </summary>
        public LastNameValueGenerator() : base(PropertyExpression.LastName, PropertyExpression.Gender, typeof(string))
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

                return male.LastName;
            }

            // Use a female name
            var female = TestData.NextFemale();

            return female.LastName;
        }

        /// <inheritdoc />
        public override int Priority
        {
            get;
        } = 1000;
    }
}
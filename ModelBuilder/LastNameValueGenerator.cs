using System;
using ModelBuilder.Data;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="LastNameValueGenerator"/>
    /// class is used to generate random last name values.
    /// </summary>
    public class LastNameValueGenerator : RelativeValueGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValueGenerator"/>.
        /// </summary>
        public LastNameValueGenerator() : base(PropertyExpression.LastName, PropertyExpression.Gender)
        {
        }

        /// <inheritdoc />
        public override object Generate(Type type, string referenceName, object context)
        {
            var gender = GetSourceValue(context);

            if (string.Equals(gender, "male", StringComparison.OrdinalIgnoreCase))
            {
                // Use a male first name
                var maleIndex = Generator.NextValue(0, TestData.Males.Count - 1);
                var male = TestData.Males[maleIndex];

                return male.LastName;
            }

            // Use a female name
            var femaleIndex = Generator.NextValue(0, TestData.Females.Count - 1);
            var female = TestData.Females[femaleIndex];

            return female.LastName;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
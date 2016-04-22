using System;
using ModelBuilder.Data;

namespace ModelBuilder
{
    public class LastNameValueGenerator : RelativeValueGenerator
    {
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
                var maleIndex = Generator.Next(0, TestData.Males.Count - 1);
                var male = TestData.Males[maleIndex];

                return male.LastName;
            }

            // Use a female name
            var femaleIndex = Generator.Next(0, TestData.Females.Count - 1);
            var female = TestData.Females[femaleIndex];

            return female.LastName;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
using System;
using System.Text.RegularExpressions;

namespace ModelBuilder
{
    public class DateOfBirthValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeValueGenerator"/> class.
        /// </summary>
        public DateOfBirthValueGenerator()
            : base(new Regex("dob|dateofbirth|born", RegexOptions.IgnoreCase), typeof(DateTime), typeof(DateTime?), typeof(DateTimeOffset), typeof(DateTimeOffset?))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, object context)
        {
            var generateType = type;

            if (generateType.IsNullable())
            {
                // Allow for a 10% the chance that this might be null
                var range = Generator.Next(0, 100);

                if (range < 10)
                {
                    return null;
                }

                // Hijack the type to generator so we can continue with the normal code pointed at the correct type to generate
                generateType = type.GenericTypeArguments[0];
            }

            var years = Generator.Next(0, 98);
            var months = Generator.Next(0, 12);
            var days = Generator.Next(0, 31);
            var hours = Generator.Next(0, 24);
            var minutes = Generator.Next(0, 60);

            if (generateType == typeof(DateTime))
            {
                var point = DateTime.UtcNow;

                point = point.AddYears(-years).AddMonths(-months).AddDays(-days).AddHours(-hours).AddMinutes(-minutes);

                return point;
            }

            var offsetPoint = DateTimeOffset.UtcNow;

            offsetPoint =
                offsetPoint.AddYears(-years).AddMonths(-months).AddDays(-days).AddHours(-hours).AddMinutes(-minutes);

            return offsetPoint;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
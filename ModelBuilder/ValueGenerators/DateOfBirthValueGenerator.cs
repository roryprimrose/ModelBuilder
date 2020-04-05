namespace ModelBuilder.ValueGenerators
{
    using System;

    /// <summary>
    ///     The <see cref="DateOfBirthValueGenerator" />
    ///     class is used to generate random date of birth values.
    /// </summary>
    public class DateOfBirthValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DateTimeValueGenerator" /> class.
        /// </summary>
        public DateOfBirthValueGenerator() : base(
            PropertyExpression.DateOfBirth,
            typeof(DateTime),
            typeof(DateTime?),
            typeof(DateTimeOffset),
            typeof(DateTimeOffset?))
        {
        }

        /// <inheritdoc />
        protected override object Generate(IExecuteStrategy executeStrategy, Type type, string referenceName)
        {
            var generateType = type;

            if (generateType.IsNullable())
            {
                // Allow for a 10% the chance that this might be null
                var range = Generator.NextValue(0, 100);

                if (range < 10)
                {
                    return null;
                }

                // Hijack the type to generator so we can continue with the normal code pointed at the correct type to generate
                generateType = type.GetGenericArguments()[0];
            }

            var years = Generator.NextValue(0, 98);
            var months = Generator.NextValue(0, 12);
            var days = Generator.NextValue(0, 31);
            var hours = Generator.NextValue(0, 24);
            var minutes = Generator.NextValue(0, 60);

            if (generateType == typeof(DateTime))
            {
                var point = DateTime.UtcNow;

                point = point.AddYears(-years).AddMonths(-months).AddDays(-days).AddHours(-hours).AddMinutes(-minutes);

                return point;
            }

            var offsetPoint = DateTimeOffset.UtcNow;

            offsetPoint = offsetPoint.AddYears(-years).AddMonths(-months).AddDays(-days).AddHours(-hours)
                .AddMinutes(-minutes);

            return offsetPoint;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
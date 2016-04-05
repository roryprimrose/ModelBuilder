using System;
using System.Globalization;
using ModelBuilder.Properties;

namespace ModelBuilder
{
    public class DateOfBirthValueGenerator : DateTimeValueGenerator
    {
        /// <inheritdoc />
        public override object Generate(Type type, string referenceName, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (IsSupported(type, referenceName, context) == false)
            {
                var message = string.Format(CultureInfo.CurrentCulture, Resources.Error_TypeNotSupportedFormat,
                    GetType().FullName, type.FullName);

                throw new NotSupportedException(message);
            }

            var years = Generator.Next(0, 98);
            var months = Generator.Next(0, 12);
            var days = Generator.Next(0, 31);
            var hours = Generator.Next(0, 24);
            var minutes = Generator.Next(0, 60);

            if (type == typeof(DateTime))
            {
                var point = DateTime.UtcNow;

                point = point.AddYears(-years).AddMonths(-months).AddDays(-days).AddHours(-hours).AddMinutes(-minutes);

                return point;
            }

            var offsetPoint = DateTimeOffset.UtcNow;

            offsetPoint = offsetPoint.AddYears(-years).AddMonths(-months).AddDays(-days).AddHours(-hours).AddMinutes(-minutes);

            return offsetPoint;
        }

        /// <inheritdoc />
        public override bool IsSupported(Type type, string referenceName, object context)
        {
            var baseSupported = base.IsSupported(type, referenceName, context);

            if (baseSupported == false)
            {
                return false;
            }

            if (type != typeof(DateTime)
                && type != typeof(DateTimeOffset))
            {
                return false;
            }

            if (string.IsNullOrEmpty(referenceName))
            {
                return false;
            }

            if (referenceName.IndexOf("dob", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return true;
            }

            if (referenceName.IndexOf("dateofbirth", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return true;
            }

            if (referenceName.IndexOf("born", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;

        /// <summary>
        /// Gets the random generator.
        /// </summary>
        private Random Generator { get; } = new Random(Environment.TickCount);
    }
}
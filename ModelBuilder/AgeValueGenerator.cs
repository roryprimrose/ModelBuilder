using System;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="AgeValueGenerator"/>
    /// class is used to generate numbers that should represent a persons age.
    /// </summary>
    public class AgeValueGenerator : NumericValueGenerator
    {
        /// <inheritdoc />
        public override object Generate(Type type, string referenceName, object context)
        {
            VerifyGenerateRequest(type, referenceName, context);

            var value = Generator.Next(0, MaxAge);

            return Convert.ChangeType(value, type);
        }

        /// <inheritdoc />
        public override bool IsSupported(Type type, string referenceName, object context)
        {
            var baseSupported = base.IsSupported(type, referenceName, context);

            if (baseSupported == false)
            {
                return false;
            }

            if (string.IsNullOrEmpty(referenceName))
            {
                return false;
            }

            if (referenceName.IndexOf("age", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;

        public int MaxAge { get; set; } = DefaultMaxAge;

        public static int DefaultMaxAge { get; set; } = 100;
    }
}
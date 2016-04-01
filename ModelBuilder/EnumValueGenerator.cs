using System;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="BooleanValueGenerator"/>
    /// class is used to generate random enum values.
    /// </summary>
    public class EnumValueGenerator : ValueGeneratorBase
    {
        private static readonly Random _random = new Random(Environment.TickCount);

        /// <inheritdoc />
        public override object Generate(Type type, string referenceName, object context)
        {
            VerifyGenerateRequest(type, referenceName, context);

            var values = Enum.GetValues(type);
            var valueIndex = _random.Next(0, values.Length - 1);

            return values.GetValue(valueIndex);
        }

        /// <inheritdoc />
        public override bool IsSupported(Type type, string referenceName, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsEnum)
            {
                return true;
            }

            return false;
        }
    }
}
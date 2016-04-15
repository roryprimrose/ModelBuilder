using System;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="NumericValueGenerator"/>
    /// class is used to generate random numeric values.
    /// </summary>
    public class NumericValueGenerator : ValueGeneratorBase
    {
        /// <inheritdoc />
        public override object Generate(Type type, string referenceName, object context)
        {
            VerifyGenerateRequest(type, referenceName, context);

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

            var min = GetMinimum(generateType, referenceName, context);
            var max = GetMaximum(generateType, referenceName, context);

            return Generator.Next(generateType, min, max);
        }

        protected virtual object GetMaximum(Type type, string referenceName, object context)
        {
            return Generator.GetMax(type);
        }

        protected virtual object GetMinimum(Type type, string referenceName, object context)
        {
            return Generator.GetMin(type);
        }

        /// <inheritdoc />
        public override bool IsSupported(Type type, string referenceName, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsNullable())
            {
                // Get the internal type
                var internalType = type.GenericTypeArguments[0];

                return Generator.IsSupported(internalType);
            }

            return Generator.IsSupported(type);
        }
    }
}
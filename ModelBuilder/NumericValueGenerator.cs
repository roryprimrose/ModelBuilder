using System;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="BooleanValueGenerator"/>
    /// class is used to generate random numeric values.
    /// </summary>
    public class NumericValueGenerator : ValueGeneratorBase
    {
        /// <inheritdoc />
        public override object Generate(Type type, string referenceName, object context)
        {
            VerifyGenerateRequest(type, referenceName, context);

            return Generator.Next(type);
        }

        /// <inheritdoc />
        public override bool IsSupported(Type type, string referenceName, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return Generator.IsSupported(type);
        }
    }
}
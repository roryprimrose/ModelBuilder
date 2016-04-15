using System;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="StringValueGenerator"/>
    /// class is used to generate random <see cref="string"/> values.
    /// </summary>
    public class StringValueGenerator : ValueGeneratorBase
    {
        /// <inheritdoc />
        public override object Generate(Type type, string referenceName, object context)
        {
            VerifyGenerateRequest(type, referenceName, context);

            return Guid.NewGuid().ToString();
        }

        /// <inheritdoc />
        public override bool IsSupported(Type type, string referenceName, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type == typeof (string);
        }
    }
}
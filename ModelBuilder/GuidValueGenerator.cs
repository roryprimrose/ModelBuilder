using System;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="GuidValueGenerator"/>
    /// class is used to generate <see cref="Guid"/> values.
    /// </summary>
    public class GuidValueGenerator : ValueGeneratorBase
    {
        /// <inheritdoc />
        public override object Generate(Type type, string referenceName, object context)
        {
            VerifyGenerateRequest(type, referenceName, context);

            if (type == typeof (Guid))
            {
                return Guid.NewGuid();
            }

            // Weight the random distribution so that it is roughly 5 times more likely to get a new guid than a null
            var source = Generator.Next<double>(0, 5);

            Guid? value;

            if (source < 1)
            {
                value = null;
            }
            else
            {
                value = Guid.NewGuid();
            }

            return value;
        }

        /// <inheritdoc />
        public override bool IsSupported(Type type, string referenceName, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type == typeof(Guid))
            {
                return true;
            }

            if (type == typeof(Guid?))
            {
                return true;
            }

            return false;
        }
    }
}
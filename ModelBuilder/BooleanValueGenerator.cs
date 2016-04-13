using System;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="BooleanValueGenerator"/>
    /// class is used to generate random <see cref="bool"/> values.
    /// </summary>
    public class BooleanValueGenerator : ValueGeneratorBase
    {
        /// <inheritdoc />
        public override object Generate(Type type, string referenceName, object context)
        {
            VerifyGenerateRequest(type, referenceName, context);

            if (type == typeof (bool?))
            {
                var source = Generator.Next(0, 2);
                bool? value = null;

                if (source == 0)
                {
                    value = false;
                }

                if (source == 1)
                {
                    value = true;
                }

                return value;
            }

            var nextValue = Generator.Next(0, 1);

            if (nextValue == 0)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public override bool IsSupported(Type type, string referenceName, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            
            if (type == typeof (bool))
            {
                return true;
            }

            if (type == typeof (bool?))
            {
                return true;
            }

            return false;
        }
    }
}
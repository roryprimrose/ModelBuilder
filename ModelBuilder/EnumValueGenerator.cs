using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="EnumValueGenerator"/>
    /// class is used to generate random enum values.
    /// </summary>
    public class EnumValueGenerator : ValueGeneratorBase
    {
        /// <inheritdoc />
        public override object Generate(Type type, string referenceName, object context)
        {
            VerifyGenerateRequest(type, referenceName, context);

            var isFlags = type.GetCustomAttributes(typeof(FlagsAttribute), true).Any();

            var values = Enum.GetValues(type);

            if (values.Length == 0)
            {
                // Return the default value of the enum
                return Activator.CreateInstance(type);
            }

            if (values.Length == 1)
            {
                return values.GetValue(0);
            }

            if (isFlags)
            {
                // Build a bitwise value
                var flagCount = Generator.Next(1, values.Length);
                var parts = new List<string>();
                
                for (var index = 0; index < flagCount; index++)
                {
                    var nextIndex = Generator.Next(0, values.Length - 1);
                    var nextValue = values.GetValue(nextIndex);
                    var valueText = nextValue.ToString();
                    
                    parts.Add(valueText);
                }

                var text = parts.Aggregate((x, y) => x + ", " + y);

                return Enum.Parse(type, text, true);
            }

            // This is not a flags enum so we will return a single value
            var valueIndex = Generator.Next(0, values.Length - 1);

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
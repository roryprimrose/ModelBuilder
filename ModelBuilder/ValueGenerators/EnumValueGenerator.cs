namespace ModelBuilder.ValueGenerators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     The <see cref="EnumValueGenerator" />
    ///     class is used to generate random enum values.
    /// </summary>
    public class EnumValueGenerator : ValueGeneratorBase
    {
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        protected override bool IsMatch(IBuildChain buildChain, Type type, string referenceName)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var generateType = type;

            if (generateType.IsNullable())
            {
                generateType = type.GetGenericArguments()[0];
            }

            if (generateType.IsEnum)
            {
                return true;
            }

            return false;
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

            var isFlags = generateType.GetCustomAttributes(typeof(FlagsAttribute), true).Any();

            var values = Enum.GetValues(generateType);

            if (values.Length == 0)
            {
                // Return the default value of the enum
                return Activator.CreateInstance(generateType);
            }

            if (values.Length == 1)
            {
                return values.GetValue(0);
            }

            if (isFlags)
            {
                // Build a bitwise value
                var flagCount = Generator.NextValue(1, values.Length);
                var parts = new List<string>();

                for (var index = 0; index < flagCount; index++)
                {
                    var nextIndex = Generator.NextValue(0, values.Length - 1);
                    var nextValue = values.GetValue(nextIndex);
                    var valueText = nextValue.ToString();

                    parts.Add(valueText);
                }

                var text = parts.Aggregate((x, y) => x + ", " + y);

                return Enum.Parse(generateType, text, true);
            }

            // This is not a flags enum so we will return a single value
            var valueIndex = Generator.NextValue(0, values.Length - 1);

            return values.GetValue(valueIndex);
        }
    }
}
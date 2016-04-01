using System;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="Extensions"/>
    /// class provides extension methods for the <see cref="IValueGenerator"/> interface.
    /// </summary>
    public static class ValueGeneratorExtensions
    {
        /// <summary>
        /// Generates a new value of the specified type.
        /// </summary>
        /// <param name="generator">The generator that will create a value.</param>
        /// <param name="type">The type of value to generate.</param>
        /// <returns>A new value of the type.</returns>
        public static object Generate(this IValueGenerator generator, Type type)
        {
            if (generator == null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            return generator.Generate(type, null, null);
        }

        /// <summary>
        /// Returns whether the specified type is supported by this generator.
        /// </summary>
        /// <param name="generator">The generator that will create a value.</param>
        /// <param name="type">The type to evaulate.</param>
        /// <returns><c>true</c> if the type is supported; otherwise <c>false</c>.</returns>
        public static bool IsSupported(this IValueGenerator generator, Type type)
        {
            if (generator == null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            return generator.IsSupported(type, null, null);
        }
    }
}
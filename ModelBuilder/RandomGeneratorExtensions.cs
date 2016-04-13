using System;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="RandomGeneratorExtensions"/>
    /// class provides extension methods for the <see cref="IRandomGenerator"/> interface.
    /// </summary>
    public static class RandomGeneratorExtensions
    {
        /// <summary>
        /// Generates a new random value constrained to the specified maximum boundary.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <returns>A new random value.</returns>
        public static T Next<T>(this IRandomGenerator generator) where T : struct
        {
            if (generator == null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            var type = typeof (T);
            var max = generator.GetMax(type);
            var min = generator.GetMin(type);

            return (T) generator.Next(type, min, max);
        }

        /// <summary>
        /// Generates a new random value constrained to the specified maximum boundary.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>A new random value.</returns>
        public static T Next<T>(this IRandomGenerator generator, T max)
        {
            if (generator == null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            var type = typeof (T);
            var min = generator.GetMin(type);

            return (T) generator.Next(type, min, max);
        }

        /// <summary>
        /// Generates a new random value constrained to the specified maximum boundary.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="type">The type of number to generate.</param>
        /// <returns>A new random value.</returns>
        public static object Next(this IRandomGenerator generator, Type type)
        {
            if (generator == null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var max = generator.GetMax(type);
            var min = generator.GetMin(type);

            return generator.Next(type, min, max);
        }

        /// <summary>
        /// Generates a new random value constrained to the specified maximum boundary.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="type">The type of number to generate.</param>
        /// <returns>A new random value.</returns>
        public static object Next(this IRandomGenerator generator, Type type, object max)
        {
            if (generator == null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var min = generator.GetMin(type);

            return generator.Next(type, min, max);
        }
    }
}
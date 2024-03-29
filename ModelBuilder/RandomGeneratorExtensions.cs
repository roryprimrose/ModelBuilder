namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="RandomGeneratorExtensions" />
    ///     class provides extension methods for the <see cref="IRandomGenerator" /> interface.
    /// </summary>
    public static class RandomGeneratorExtensions
    {
        /// <summary>
        ///     Generates a new random value constrained to the specified maximum boundary.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <returns>A new random value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="generator" /> parameter is <c>null</c>.</exception>
        public static T NextValue<T>(this IRandomGenerator generator) where T : struct
        {
            generator = generator ?? throw new ArgumentNullException(nameof(generator));

            var type = typeof(T);
            var max = generator.GetMax(type);
            var min = generator.GetMin(type);
            var value = generator.NextValue(type, min, max);

            return (T)value;
        }

        /// <summary>
        ///     Generates a new random value constrained to the specified maximum boundary.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>A new random value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="generator" /> parameter is <c>null</c>.</exception>
        public static T NextValue<T>(this IRandomGenerator generator, T max) where T : struct
        {
            generator = generator ?? throw new ArgumentNullException(nameof(generator));

            var type = typeof(T);
            var min = generator.GetMin(type);

            return (T)generator.NextValue(type, min, max);
        }

        /// <summary>
        ///     Generates a new random value constrained to the specified minimum and maximum boundaries.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>A new random value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="generator" /> parameter is <c>null</c>.</exception>
        public static T NextValue<T>(this IRandomGenerator generator, T min, T max) where T : struct
        {
            generator = generator ?? throw new ArgumentNullException(nameof(generator));

            var type = typeof(T);

            return (T)generator.NextValue(type, min, max);
        }

        /// <summary>
        ///     Generates a new random value constrained to the specified maximum boundary.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="type">The type of number to generate.</param>
        /// <returns>A new random value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="generator" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public static object NextValue(this IRandomGenerator generator, Type type)
        {
            generator = generator ?? throw new ArgumentNullException(nameof(generator));

            type = type ?? throw new ArgumentNullException(nameof(type));

            var max = generator.GetMax(type);
            var min = generator.GetMin(type);

            return generator.NextValue(type, min, max);
        }

        /// <summary>
        ///     Generates a new random value constrained to the specified maximum boundary.
        /// </summary>
        /// <param name="generator">The generator.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="type">The type of number to generate.</param>
        /// <returns>A new random value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="generator" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public static object NextValue(this IRandomGenerator generator, Type type, object max)
        {
            generator = generator ?? throw new ArgumentNullException(nameof(generator));

            type = type ?? throw new ArgumentNullException(nameof(type));

            var min = generator.GetMin(type);

            return generator.NextValue(type, min, max);
        }
    }
}
using System;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="IRandomGenerator"/>
    /// interface defines the members for generating random numeric values.
    /// </summary>
    public interface IRandomGenerator
    {
        /// <summary>
        /// Gets the maxmum value for the specified type.
        /// </summary>
        /// <param name="type">The type to evaluate.</param>
        /// <returns>The maximum value for the type.</returns>
        double GetMax(Type type);

        /// <summary>
        /// Gets the minimum value for the specified type.
        /// </summary>
        /// <param name="type">The type to evaluate.</param>
        /// <returns>The minimum value for the type.</returns>
        double GetMin(Type type);

        /// <summary>
        /// Gets whether the specified type is supported by the generator.
        /// </summary>
        /// <param name="type">The type to evaluate.</param>
        /// <returns><c>true</c> if the type is supported; otherwise <c>false</c>.</returns>
        bool IsSupported(Type type);

        /// <summary>
        /// Generates a new random value constrained to the specified minimum and maximum boundaries.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <typeparam name="T">The type of number to generate.</typeparam>
        /// <returns>A new random value.</returns>
        T NextValue<T>(T min, T max) where T : struct;

        /// <summary>
        /// Generates a new random value constrained to the specified minimum and maximum boundaries.
        /// </summary>
        /// <param name="type">The type of number to generate.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <returns>A new random value.</returns>
        object NextValue(Type type, object min, object max);

        /// <summary>
        /// Populates the specified buffer with random bytes.
        /// </summary>
        void NextValue(byte[] buffer);
    }
}
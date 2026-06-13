namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="IRandomSource" />
    ///     interface defines a thread-safe, seedable source of strongly-typed random values.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This is the vNext replacement for <see cref="IRandomGenerator" />. Unlike the v8
    ///         generator it exposes typed methods that return the value type directly (no boxing and
    ///         no reflection on the value path), generates wide-integer and <see cref="decimal" />
    ///         ranges with their own arithmetic (no precision loss through <see cref="double" />),
    ///         fills byte buffers in a single call, validates range boundaries at the public edge,
    ///         and is safe for concurrent use across threads.
    ///     </para>
    ///     <para>
    ///         All numeric range methods are inclusive of both <c>min</c> and <c>max</c>.
    ///     </para>
    /// </remarks>
    public interface IRandomSource
    {
        /// <summary>
        ///     Returns a random <see cref="bool" /> value.
        /// </summary>
        /// <returns><c>true</c> or <c>false</c> with equal probability.</returns>
        bool NextBool();

        /// <summary>
        ///     Returns a random <see cref="byte" /> within the inclusive range.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        /// <returns>A random value in the range <paramref name="min" /> to <paramref name="max" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="min" /> is greater than <paramref name="max" />.
        /// </exception>
        byte NextByte(byte min = byte.MinValue, byte max = byte.MaxValue);

        /// <summary>
        ///     Fills the specified buffer with random bytes.
        /// </summary>
        /// <param name="buffer">The buffer to fill.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="buffer" /> parameter is <c>null</c>.</exception>
        void NextBytes(byte[] buffer);

        /// <summary>
        ///     Returns a random <see cref="decimal" /> across the full range of the type.
        /// </summary>
        /// <returns>A random value between <see cref="decimal.MinValue" /> and <see cref="decimal.MaxValue" />.</returns>
        decimal NextDecimal();

        /// <summary>
        ///     Returns a random <see cref="decimal" /> within the inclusive range.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        /// <returns>A random value in the range <paramref name="min" /> to <paramref name="max" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="min" /> is greater than <paramref name="max" />.
        /// </exception>
        decimal NextDecimal(decimal min, decimal max);

        /// <summary>
        ///     Returns a random <see cref="double" /> within the inclusive range.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        /// <returns>A random value in the range <paramref name="min" /> to <paramref name="max" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="min" /> is greater than <paramref name="max" />.
        /// </exception>
        double NextDouble(double min = double.MinValue, double max = double.MaxValue);

        /// <summary>
        ///     Returns a random <see cref="short" /> within the inclusive range.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        /// <returns>A random value in the range <paramref name="min" /> to <paramref name="max" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="min" /> is greater than <paramref name="max" />.
        /// </exception>
        short NextInt16(short min = short.MinValue, short max = short.MaxValue);

        /// <summary>
        ///     Returns a random <see cref="int" /> within the inclusive range.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        /// <returns>A random value in the range <paramref name="min" /> to <paramref name="max" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="min" /> is greater than <paramref name="max" />.
        /// </exception>
        int NextInt32(int min = int.MinValue, int max = int.MaxValue);

        /// <summary>
        ///     Returns a random <see cref="long" /> within the inclusive range.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        /// <returns>A random value in the range <paramref name="min" /> to <paramref name="max" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="min" /> is greater than <paramref name="max" />.
        /// </exception>
        long NextInt64(long min = long.MinValue, long max = long.MaxValue);

        /// <summary>
        ///     Returns a random <see cref="sbyte" /> within the inclusive range.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        /// <returns>A random value in the range <paramref name="min" /> to <paramref name="max" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="min" /> is greater than <paramref name="max" />.
        /// </exception>
        sbyte NextSByte(sbyte min = sbyte.MinValue, sbyte max = sbyte.MaxValue);

        /// <summary>
        ///     Returns a random <see cref="float" /> within the inclusive range.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        /// <returns>A random value in the range <paramref name="min" /> to <paramref name="max" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="min" /> is greater than <paramref name="max" />.
        /// </exception>
        float NextSingle(float min = float.MinValue, float max = float.MaxValue);

        /// <summary>
        ///     Returns a random <see cref="ushort" /> within the inclusive range.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        /// <returns>A random value in the range <paramref name="min" /> to <paramref name="max" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="min" /> is greater than <paramref name="max" />.
        /// </exception>
        ushort NextUInt16(ushort min = ushort.MinValue, ushort max = ushort.MaxValue);

        /// <summary>
        ///     Returns a random <see cref="uint" /> within the inclusive range.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        /// <returns>A random value in the range <paramref name="min" /> to <paramref name="max" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="min" /> is greater than <paramref name="max" />.
        /// </exception>
        uint NextUInt32(uint min = uint.MinValue, uint max = uint.MaxValue);

        /// <summary>
        ///     Returns a random <see cref="ulong" /> within the inclusive range.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        /// <returns>A random value in the range <paramref name="min" /> to <paramref name="max" />.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="min" /> is greater than <paramref name="max" />.
        /// </exception>
        ulong NextUInt64(ulong min = ulong.MinValue, ulong max = ulong.MaxValue);

        /// <summary>
        ///     Gets the seed used to initialize this source.
        /// </summary>
        /// <returns>The seed value, which can be supplied to a new <see cref="IRandomSource" /> to reproduce the sequence.</returns>
        int Seed { get; }
    }
}

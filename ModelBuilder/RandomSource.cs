namespace ModelBuilder
{
    using System;
    using System.Globalization;
    using System.Threading;

    /// <summary>
    ///     The <see cref="RandomSource" />
    ///     class provides a thread-safe, seedable implementation of <see cref="IRandomSource" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Each instance owns a single <see cref="Random" /> guarded by a lock, so the source is
    ///         safe to share across threads and produces a reproducible sequence for a given seed.
    ///         Wide-integer ranges use 64-bit arithmetic with rejection sampling (no modulo bias and
    ///         no precision loss through <see cref="double" />), and byte buffers are filled in a
    ///         single call.
    ///     </para>
    /// </remarks>
    public sealed class RandomSource : IRandomSource
    {
        private const decimal UnitDecimalScale = 1_000_000_000_000_000_000m;
        private const double UnitDoubleScale = 1.0 / 9007199254740992.0;
        private static int _seedRoot = Environment.TickCount;
        private readonly byte[] _buffer = new byte[8];
        private readonly Random _random;
        private readonly object _sync = new object();

        /// <summary>
        ///     Initializes a new instance of the <see cref="RandomSource" /> class with an
        ///     automatically generated seed.
        /// </summary>
        public RandomSource() : this(GenerateSeed())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RandomSource" /> class with the specified
        ///     seed.
        /// </summary>
        /// <param name="seed">The seed used to initialize the underlying random number generator.</param>
        public RandomSource(int seed)
        {
            Seed = seed;
            _random = new Random(seed);
        }

        /// <inheritdoc />
        public bool NextBool()
        {
            return (NextRawUInt64() & 1UL) == 1UL;
        }

        /// <inheritdoc />
        public byte NextByte(byte min = byte.MinValue, byte max = byte.MaxValue)
        {
            return (byte)NextInt32(min, max);
        }

        /// <inheritdoc />
        public void NextBytes(byte[] buffer)
        {
            buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));

            lock (_sync)
            {
                _random.NextBytes(buffer);
            }
        }

        /// <inheritdoc />
        public decimal NextDecimal()
        {
            return NextDecimal(decimal.MinValue, decimal.MaxValue);
        }

        /// <inheritdoc />
        public decimal NextDecimal(decimal min, decimal max)
        {
            ValidateRange(min, max);

            if (min == max)
            {
                return min;
            }

            var unit = NextUnitDecimal();

            try
            {
                var range = max - min;

                return min + range * unit;
            }
            catch (OverflowException)
            {
                // The span exceeds the decimal range; interpolate so neither term overflows.
                return min * (1m - unit) + max * unit;
            }
        }

        /// <inheritdoc />
        public double NextDouble(double min = double.MinValue, double max = double.MaxValue)
        {
            ValidateRange(min, max);

            if (min.Equals(max))
            {
                return min;
            }

            var unit = NextUnitDouble();
            var range = max - min;

            if (double.IsInfinity(range))
            {
                // The span is too large to represent; interpolate so neither term is infinite.
                return min * (1.0 - unit) + max * unit;
            }

            var result = min + range * unit;

            return result > max ? max : result;
        }

        /// <inheritdoc />
        public short NextInt16(short min = short.MinValue, short max = short.MaxValue)
        {
            return (short)NextInt32(min, max);
        }

        /// <inheritdoc />
        public int NextInt32(int min = int.MinValue, int max = int.MaxValue)
        {
            ValidateRange(min, max);

            var span = (long)max - min;
            var offset = NextBoundedUInt64((ulong)(span + 1));

            return (int)(min + (long)offset);
        }

        /// <inheritdoc />
        public long NextInt64(long min = long.MinValue, long max = long.MaxValue)
        {
            ValidateRange(min, max);

            var span = unchecked((ulong)max - (ulong)min);

            if (span == ulong.MaxValue)
            {
                return unchecked((long)((ulong)min + NextRawUInt64()));
            }

            var offset = NextBoundedUInt64(span + 1);

            return unchecked((long)((ulong)min + offset));
        }

        /// <inheritdoc />
        public sbyte NextSByte(sbyte min = sbyte.MinValue, sbyte max = sbyte.MaxValue)
        {
            return (sbyte)NextInt32(min, max);
        }

        /// <inheritdoc />
        public float NextSingle(float min = float.MinValue, float max = float.MaxValue)
        {
            ValidateRange(min, max);

            if (min.Equals(max))
            {
                return min;
            }

            var unit = NextUnitDouble();
            var range = (double)max - min;
            var result = min + range * unit;

            return result > max ? max : (float)result;
        }

        /// <inheritdoc />
        public ushort NextUInt16(ushort min = ushort.MinValue, ushort max = ushort.MaxValue)
        {
            return (ushort)NextInt32(min, max);
        }

        /// <inheritdoc />
        public uint NextUInt32(uint min = uint.MinValue, uint max = uint.MaxValue)
        {
            ValidateRange(min, max);

            var span = (ulong)max - min;
            var offset = NextBoundedUInt64(span + 1);

            return (uint)((ulong)min + offset);
        }

        /// <inheritdoc />
        public ulong NextUInt64(ulong min = ulong.MinValue, ulong max = ulong.MaxValue)
        {
            ValidateRange(min, max);

            var span = max - min;

            if (span == ulong.MaxValue)
            {
                return NextRawUInt64();
            }

            return min + NextBoundedUInt64(span + 1);
        }

        private static int GenerateSeed()
        {
            // Combine a monotonically increasing counter with the tick count so that sources created
            // in a tight loop receive distinct, well-distributed seeds without sharing a Random.
            var counter = Interlocked.Increment(ref _seedRoot);

            return unchecked(counter * 397 ^ Environment.TickCount);
        }

        private static void ValidateRange<T>(T min, T max) where T : IComparable<T>
        {
            if (min.CompareTo(max) > 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(min),
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The minimum value '{0}' cannot be greater than the maximum value '{1}'.",
                        min,
                        max));
            }
        }

        private ulong NextBoundedUInt64(ulong exclusiveBound)
        {
            // Rejection sampling removes the modulo bias that a plain '% exclusiveBound' would introduce.
            var limit = ulong.MaxValue - ulong.MaxValue % exclusiveBound;

            ulong value;

            do
            {
                value = NextRawUInt64();
            }
            while (value >= limit);

            return value % exclusiveBound;
        }

        private ulong NextRawUInt64()
        {
            lock (_sync)
            {
                _random.NextBytes(_buffer);

                return BitConverter.ToUInt64(_buffer, 0);
            }
        }

        private decimal NextUnitDecimal()
        {
            // Build a value in [0, 1) from two nine-digit components for eighteen digits of precision.
            var high = NextInt32(0, 999999999);
            var low = NextInt32(0, 999999999);
            var numerator = (decimal)high * 1000000000m + low;

            return numerator / UnitDecimalScale;
        }

        private double NextUnitDouble()
        {
            // Use the top 53 bits to produce a value in [0, 1) with full double precision.
            return (NextRawUInt64() >> 11) * UnitDoubleScale;
        }

        /// <inheritdoc />
        public int Seed { get; }
    }
}

using System;
using System.Diagnostics.CodeAnalysis;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="RandomExtensions"/>
    /// class is used to provide extension methods for the <see cref="Random"/> class.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Returns a next random float value.
        /// </summary>
        /// <param name="random">The random instance.</param>
        /// <returns>A random float value.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "float",
            Justification =
                "This is appropriate here because the purpose of the method is to create a framework specific value.")]
        public static float NextFloat(this Random random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            var mantissa = random.NextDouble()*2.0 - 1.0;
            var exponent = Math.Pow(2.0, random.Next(-126, 128));

            return (float) (mantissa*exponent);
        }

        /// <summary>
        /// Returns a next random long value.
        /// </summary>
        /// <param name="random">The random instance.</param>
        /// <returns>A random long value.</returns>
        public static long NextInt64(this Random random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            var buffer = new byte[sizeof (long)];

            random.NextBytes(buffer);

            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// Returns a next random unsigned int value.
        /// </summary>
        /// <param name="random">The random instance.</param>
        /// <returns>A random unsigned int value.</returns>
        public static uint NextUInt32(this Random random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            var buffer = new byte[sizeof (uint)];

            random.NextBytes(buffer);

            return BitConverter.ToUInt32(buffer, 0);
        }

        /// <summary>
        /// Returns a next random unsigned long value.
        /// </summary>
        /// <param name="random">The random instance.</param>
        /// <returns>A random unsigned long value.</returns>
        public static ulong NextUInt64(this Random random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            var buffer = new byte[sizeof (ulong)];

            random.NextBytes(buffer);

            return BitConverter.ToUInt64(buffer, 0);
        }
    }
}
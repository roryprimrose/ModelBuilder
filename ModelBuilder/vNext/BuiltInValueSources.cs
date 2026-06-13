namespace ModelBuilder.vNext
{
    using System;

    /// <summary>
    ///     The <see cref="BuiltInValueSources" /> class
    ///     creates the default registry of value sources for the in-box leaf types: primitives, core
    ///     value types, and the common BCL types that appear in models.
    /// </summary>
    public static class BuiltInValueSources
    {
        private const string StringAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>
        ///     Creates a new registry populated with all built-in value sources.
        /// </summary>
        /// <returns>The populated registry.</returns>
        public static ValueSourceRegistry CreateRegistry()
        {
            var registry = new ValueSourceRegistry();

            registry.Register<bool>(new DelegateValueSource<bool>(c => c.Random.NextBool()));
            registry.Register<byte>(new DelegateValueSource<byte>(c => c.Random.NextByte()));
            registry.Register<sbyte>(new DelegateValueSource<sbyte>(c => c.Random.NextSByte()));
            registry.Register<short>(new DelegateValueSource<short>(c => c.Random.NextInt16()));
            registry.Register<ushort>(new DelegateValueSource<ushort>(c => c.Random.NextUInt16()));
            registry.Register<int>(new DelegateValueSource<int>(c => c.Random.NextInt32()));
            registry.Register<uint>(new DelegateValueSource<uint>(c => c.Random.NextUInt32()));
            registry.Register<long>(new DelegateValueSource<long>(c => c.Random.NextInt64()));
            registry.Register<ulong>(new DelegateValueSource<ulong>(c => c.Random.NextUInt64()));
            registry.Register<float>(new DelegateValueSource<float>(c => c.Random.NextSingle()));
            registry.Register<double>(new DelegateValueSource<double>(c => c.Random.NextDouble()));
            registry.Register<decimal>(new DelegateValueSource<decimal>(c => c.Random.NextDecimal()));
            registry.Register<char>(new DelegateValueSource<char>(NextChar));
            registry.Register<string>(new DelegateValueSource<string>(NextString));
            registry.Register<Guid>(new DelegateValueSource<Guid>(NextGuid));

            registry.Register<DateTime>(new DelegateValueSource<DateTime>(NextDateTime));
            registry.Register<DateTimeOffset>(new DelegateValueSource<DateTimeOffset>(c => new DateTimeOffset(NextDateTime(c))));
            registry.Register<TimeSpan>(new DelegateValueSource<TimeSpan>(c => TimeSpan.FromMinutes(c.Random.NextInt32(0, 1440))));
            registry.Register<Uri>(new DelegateValueSource<Uri>(NextUri));
            registry.Register<Version>(new DelegateValueSource<Version>(NextVersion));
            registry.Register<byte[]>(new DelegateValueSource<byte[]>(NextBytes));

            return registry;
        }

        private static char NextChar(BuildContext context)
        {
            return StringAlphabet[context.Random.NextInt32(0, StringAlphabet.Length - 1)];
        }

        private static byte[] NextBytes(BuildContext context)
        {
            var buffer = new byte[context.Random.NextInt32(1, 16)];

            context.Random.NextBytes(buffer);

            return buffer;
        }

        private static DateTime NextDateTime(BuildContext context)
        {
            // A value within roughly the last ten years, kept well inside DateTime bounds.
            var ticksRange = TimeSpan.FromDays(3650).Ticks;
            var offset = context.Random.NextInt64(0, ticksRange);

            return new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddTicks(-offset);
        }

        private static Guid NextGuid(BuildContext context)
        {
            var buffer = new byte[16];

            context.Random.NextBytes(buffer);

            return new Guid(buffer);
        }

        private static string NextString(BuildContext context)
        {
            var length = context.Random.NextInt32(5, 12);
            var chars = new char[length];

            for (var index = 0; index < length; index++)
            {
                chars[index] = StringAlphabet[context.Random.NextInt32(0, StringAlphabet.Length - 1)];
            }

            return new string(chars);
        }

        private static Uri NextUri(BuildContext context)
        {
            return new Uri("https://" + NextString(context).ToLowerInvariant() + ".example.com/");
        }

        private static Version NextVersion(BuildContext context)
        {
            return new Version(
                context.Random.NextInt32(0, 20),
                context.Random.NextInt32(0, 20),
                context.Random.NextInt32(0, 99),
                context.Random.NextInt32(0, 9999));
        }

        /// <summary>
        ///     Gets the shared default registry of built-in value sources.
        /// </summary>
        public static ValueSourceRegistry Default { get; } = CreateRegistry();
    }
}

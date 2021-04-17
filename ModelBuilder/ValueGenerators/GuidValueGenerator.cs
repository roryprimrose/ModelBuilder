namespace ModelBuilder.ValueGenerators
{
    using System;

    /// <summary>
    ///     The <see cref="GuidValueGenerator" />
    ///     class is used to generate <see cref="Guid" /> values.
    /// </summary>
    public class GuidValueGenerator : ValueGeneratorMatcher, INullableBuilder
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GuidValueGenerator" /> class.
        /// </summary>
        public GuidValueGenerator() : base(typeof(Guid), typeof(Guid?))
        {
        }

        /// <inheritdoc />
        protected override object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName)
        {
            if (type == typeof(Guid) || AllowNull == false)
            {
                return Guid.NewGuid();
            }

            // Allow for a 10% the chance that this might be null
            var range = Generator.NextValue(0, 100000);

            if (range < 10000)
            {
                return null;
            }

            return Guid.NewGuid();
        }

        /// <inheritdoc />
        public bool AllowNull { get; set; } = false;
    }
}
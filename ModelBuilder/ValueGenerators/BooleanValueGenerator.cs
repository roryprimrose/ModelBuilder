namespace ModelBuilder.ValueGenerators
{
    using System;

    /// <summary>
    ///     The <see cref="BooleanValueGenerator" />
    ///     class is used to generate random <see cref="bool" /> values.
    /// </summary>
    public class BooleanValueGenerator : ValueGeneratorMatcher, INullableBuilder
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BooleanValueGenerator" /> class.
        /// </summary>
        public BooleanValueGenerator() : base(typeof(bool), typeof(bool?))
        {
        }

        /// <inheritdoc />
        protected override object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName)
        {
            var generateType = type;

            if (generateType.IsNullable())
            {
                if (AllowNull)
                {
                    // Allow for a % the chance that this might be null
                    var range = Generator.NextValue(0, 100000);

                    if (range < NullPercentageChance * 1000)
                    {
                        return null;
                    }
                }
            }

            var nextValue = Generator.NextValue(0, 1);

            if (nextValue == 0)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public bool AllowNull { get; set; } = false;

        /// <inheritdoc />
        public int NullPercentageChance { get; set; } = 10;
    }
}
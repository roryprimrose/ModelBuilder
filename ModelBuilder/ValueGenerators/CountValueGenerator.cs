namespace ModelBuilder.ValueGenerators
{
    using System;

    /// <summary>
    ///     The <see cref="CountValueGenerator" />
    ///     class is used to generate numeric values for parameters and properties that look like count or length values.
    /// </summary>
    public class CountValueGenerator : NumericValueGenerator
    {
        /// <inheritdoc />
        public override bool IsMatch(Type type, string referenceName, IBuildChain buildChain)
        {
            var baseSupported = base.IsMatch(type, referenceName, buildChain);

            if (baseSupported == false)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(referenceName))
            {
                return false;
            }

            if (referenceName.Equals("count", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (referenceName.Equals("length", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected override object GetMaximum(Type type, string referenceName, object context)
        {
            return MaxCount;
        }

        /// <inheritdoc />
        protected override object GetMinimum(Type type, string referenceName, object context)
        {
            return 1;
        }

        /// <summary>
        ///     Gets or sets the default maximum count that can be generated.
        /// </summary>
        public static int DefaultMaxCount { get; set; } = 30;

        /// <summary>
        ///     Gets or sets the maximum count generated by this instance.
        /// </summary>
        public int MaxCount { get; set; } = DefaultMaxCount;

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
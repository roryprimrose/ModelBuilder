namespace ModelBuilder.ValueGenerators
{
    using System;

    /// <summary>
    ///     The <see cref="GuidValueGenerator" />
    ///     class is used to generate <see cref="Guid" /> values.
    /// </summary>
    public class GuidValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GuidValueGenerator" /> class.
        /// </summary>
        public GuidValueGenerator() : base(typeof(Guid), typeof(Guid?))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            if (type == typeof(Guid))
            {
                return Guid.NewGuid();
            }

            // Weight the random distribution so that it is roughly 5 times more likely to get a new guid than a null
            var source = Generator.NextValue<double>(0, 5);

            Guid? value;

            if (source < 1)
            {
                value = null;
            }
            else
            {
                value = Guid.NewGuid();
            }

            return value;
        }
    }
}
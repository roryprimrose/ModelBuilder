namespace ModelBuilder.ValueGenerators
{
    using System;
    using System.Globalization;

    /// <summary>
    ///     The <see cref="CharValueGenerator" />
    ///     class is used to generate random <see cref="char" /> values.
    /// </summary>
    /// <remarks>This generator creates char values in the printable ASCII range of 33-126.</remarks>
    public class CharValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CharValueGenerator" /> class.
        /// </summary>
        public CharValueGenerator() : base(typeof(char))
        {
        }

        /// <inheritdoc />
        protected override object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName)
        {
            // We want to use printable ASCII characters which are in the range 33-126
            var internalValue = base.Generator.NextValue(typeof(byte), 33, 126);
         
            return Convert.ToChar(internalValue, CultureInfo.CurrentCulture);
        }
    }
}
namespace ModelBuilder
{
    using System;
    using System.Text.RegularExpressions;
    using ModelBuilder.Data;

    /// <summary>
    ///     The <see cref="StateValueGenerator" />
    ///     class is used to generate state addressing values.
    /// </summary>
    public class StateValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StateValueGenerator" /> class.
        /// </summary>
        public StateValueGenerator()
            : base(new Regex("State|Region", RegexOptions.IgnoreCase), typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            var person = TestData.NextPerson();

            return person.State;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
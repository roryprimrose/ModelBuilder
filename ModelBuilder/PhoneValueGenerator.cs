namespace ModelBuilder
{
    using System;
    using System.Text.RegularExpressions;
    using ModelBuilder.Data;

    /// <summary>
    ///     The <see cref="AgeValueGenerator" />
    ///     class is used to generate phone numbers.
    /// </summary>
    public class PhoneValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PhoneValueGenerator" /> class.
        /// </summary>
        public PhoneValueGenerator()
            : base(new Regex("Phone|Cell|Mobile|Fax", RegexOptions.Compiled | RegexOptions.IgnoreCase), typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            var person = TestData.NextPerson();

            return person.Phone;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
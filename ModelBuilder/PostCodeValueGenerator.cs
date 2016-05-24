using System;
using System.Text.RegularExpressions;
using ModelBuilder.Data;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="PostCodeValueGenerator"/>
    /// class is used to generate random post code values.
    /// </summary>
    public class PostCodeValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostCodeValueGenerator"/> class.
        /// </summary>
        public PostCodeValueGenerator()
            : base(new Regex("PostCode|Zip(Code)?", RegexOptions.Compiled | RegexOptions.IgnoreCase), typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, object context)
        {
            var person = TestData.NextPerson();

            return person.PostCode;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
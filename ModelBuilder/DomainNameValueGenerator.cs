namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using ModelBuilder.Data;

    /// <summary>
    /// The <see cref="DomainNameValueGenerator"/>
    /// class is used to generate random domain name values.
    /// </summary>
    public class DomainNameValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainNameValueGenerator"/> class.
        /// </summary>
        public DomainNameValueGenerator()
            : base(new Regex("Domain", RegexOptions.Compiled | RegexOptions.IgnoreCase), typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, LinkedList<object> buildChain)
        {
            var person = TestData.NextPerson();

            return person.Domain;
        }

        /// <inheritdoc />
        public override int Priority
        {
            get;
        } = 1000;
    }
}
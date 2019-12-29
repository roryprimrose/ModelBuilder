namespace ModelBuilder
{
    using System;
    using ModelBuilder.Data;

    /// <summary>
    ///     The <see cref="DomainNameValueGenerator" />
    ///     class is used to generate random domain name values.
    /// </summary>
    public class DomainNameValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DomainNameValueGenerator" /> class.
        /// </summary>
        public DomainNameValueGenerator() : base(PropertyExpression.Domain, typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            return TestData.Domains.Next();
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
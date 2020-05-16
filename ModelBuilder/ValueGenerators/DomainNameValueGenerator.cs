namespace ModelBuilder.ValueGenerators
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
        public DomainNameValueGenerator() : base(NameExpression.Domain, typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName)
        {
            return TestData.Domains.Next();
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
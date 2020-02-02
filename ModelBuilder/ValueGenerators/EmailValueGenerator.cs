namespace ModelBuilder.ValueGenerators
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using ModelBuilder.Data;

    /// <summary>
    ///     The <see cref="EmailValueGenerator" />
    ///     class is used to generate strings that should represent an email.
    /// </summary>
    public class EmailValueGenerator : RelativeValueGenerator
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EmailValueGenerator" /> class.
        /// </summary>
        public EmailValueGenerator() : base(PropertyExpression.Email, typeof(string))
        {
        }

        /// <inheritdoc />
        [SuppressMessage(
            "Microsoft.Globalization",
            "CA1308:NormalizeStringsToUppercase",
            Justification = "Email addresses are lower case by convention.")]
        public override object Generate(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            var context = executeStrategy?.BuildChain?.Last;
            var firstName = GetValue<string>(PropertyExpression.FirstName, context);
            var lastName = GetValue<string>(PropertyExpression.LastName, context);

            if (firstName == null)
            {
                if (IsMale(executeStrategy))
                {
                    firstName = TestData.MaleNames.Next();
                }
                else
                {
                    firstName = TestData.FemaleNames.Next();
                }
            }

            if (lastName == null)
            {
                lastName = TestData.LastNames.Next();
            }

            var domain = Domain;

            if (string.IsNullOrWhiteSpace(domain))
            {
                domain = GetValue<string>(PropertyExpression.Domain, context);
            }

            if (string.IsNullOrWhiteSpace(domain))
            {
                domain = TestData.Domains.Next();
            }

            var email = firstName + "." + lastName + "@" + domain;

#if NETSTANDARD2_0
            return email.Replace(" ", string.Empty).ToLowerInvariant();
#else
            return email.Replace(" ", string.Empty, StringComparison.CurrentCulture).ToLowerInvariant();
#endif
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;

        /// <summary>
        ///     Gets the domain for the email address.
        /// </summary>
        /// <value>The domain part of the email address.</value>
        protected virtual string Domain => null;
    }
}
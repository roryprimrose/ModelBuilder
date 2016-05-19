using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using ModelBuilder.Data;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="EmailValueGenerator"/>
    /// class is used to generate strings that should represent an email.
    /// </summary>
    public class EmailValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailValueGenerator"/> class.
        /// </summary>
        public EmailValueGenerator()
            : base(new Regex("email", RegexOptions.IgnoreCase), typeof(string))
        {
        }

        /// <inheritdoc />
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase",
            Justification = "Email addresses are lower case by convention.")]
        protected override object GenerateValue(Type type, string referenceName, object context)
        {
            string firstName = null;
            string lastName = null;

            if (context != null)
            {
                var firstNameProperty = context.FindProperties(PropertyExpression.FirstName).FirstOrDefault();

                if (firstNameProperty != null)
                {
                    firstName = (string) firstNameProperty.GetValue(context);
                }

                var lastNameProperty = context.FindProperties(PropertyExpression.LastName).LastOrDefault();

                if (lastNameProperty != null)
                {
                    lastName = (string) lastNameProperty.GetValue(context);
                }
            }

            var index = Generator.NextValue(0, TestData.People.Count - 1);
            var person = TestData.People[index];

            if (firstName == null &&
                lastName == null)
            {
                return person.Email;
            }

            if (firstName == null)
            {
                firstName = person.FirstName;
            }

            if (lastName == null)
            {
                lastName = person.LastName;
            }

            var email = firstName + "." + lastName + "@" + person.Domain;

            return email.Replace(" ", string.Empty).ToLowerInvariant();
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
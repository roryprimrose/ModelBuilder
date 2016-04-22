using System;
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
        protected override object GenerateValue(Type type, string referenceName, object context)
        {
            string firstName = null;
            string lastName = null;

            if (context != null)
            {
                var firstNameExpression = new Regex(PropertyExpression.FirstName);
                var firstNameProperty = context.FindProperties(firstNameExpression).FirstOrDefault();

                if (firstNameProperty != null)
                {
                    firstName = (string) firstNameProperty.GetValue(context);
                }

                var lastNameExpression = new Regex(PropertyExpression.LastName);
                var lastNameProperty = context.FindProperties(lastNameExpression).LastOrDefault();

                if (lastNameProperty != null)
                {
                    lastName = (string) lastNameProperty.GetValue(context);
                }
            }

            var index = Generator.Next(0, TestData.People.Count - 1);
            var person = TestData.People[index];

            if (firstName == null)
            {
                firstName = person.FirstName;
            }

            if (lastName == null)
            {
                lastName = person.LastName;
            }

            var prefix = firstName.Substring(0, 1);
            var email = prefix + lastName + "@" + person.Domain;

            return email.ToLowerInvariant();
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
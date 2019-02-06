namespace ModelBuilder.UnitTests
{
    using FluentAssertions;
    using Xunit;

    public class PropertyExpressionTests
    {
        [Theory]
        [InlineData("stuff", false)]
        [InlineData("WorkEmail", true)]
        [InlineData("PersonalEmail", true)]
        [InlineData("email", true)]
        [InlineData("Email", true)]
        [InlineData("EmailAddress", true)]
        [InlineData("emailAddress", true)]
        [InlineData("emailaddress", true)]
        [InlineData("Email_Address", true)]
        [InlineData("email_Address", true)]
        [InlineData("email_address", true)]
        public void EmailEvaluatesValueTest(string value, bool expected)
        {
            var target = PropertyExpression.Email;

            var actual = target.IsMatch(value);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("stuff", false)]
        [InlineData("firstname", true)]
        [InlineData("Firstname", true)]
        [InlineData("FirstName", true)]
        [InlineData("firstName", true)]
        [InlineData("first_name", true)]
        [InlineData("First_name", true)]
        [InlineData("First_Name", true)]
        [InlineData("first_Name", true)]
        [InlineData("givenname", true)]
        [InlineData("Givenname", true)]
        [InlineData("GivenName", true)]
        [InlineData("givenName", true)]
        [InlineData("given_name", true)]
        [InlineData("Given_name", true)]
        [InlineData("Given_Name", true)]
        [InlineData("given_Name", true)]
        public void FirstNameEvaluatesValueTest(string value, bool expected)
        {
            var target = PropertyExpression.FirstName;

            var actual = target.IsMatch(value);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("stuff", false)]
        [InlineData("Gender", true)]
        [InlineData("gender", true)]
        [InlineData("Sex", true)]
        [InlineData("sex", true)]
        public void GenderEvaluatesValueTest(string value, bool expected)
        {
            var target = PropertyExpression.Gender;

            var actual = target.IsMatch(value);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("stuff", false)]
        [InlineData("lastname", true)]
        [InlineData("Lastname", true)]
        [InlineData("LastName", true)]
        [InlineData("lastName", true)]
        [InlineData("last_name", true)]
        [InlineData("Last_name", true)]
        [InlineData("Last_Name", true)]
        [InlineData("last_Name", true)]
        [InlineData("surname", true)]
        [InlineData("Surname", true)]
        public void LastNameEvaluatesValueTest(string value, bool expected)
        {
            var target = PropertyExpression.LastName;

            var actual = target.IsMatch(value);

            actual.Should().Be(expected);
        }
    }
}
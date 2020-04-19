namespace ModelBuilder.UnitTests
{
    using FluentAssertions;
    using Xunit;

    public class NameExpressionTests
    {
        [Theory]
        [InlineData("stuff", false)]
        [InlineData("Age", true)]
        [InlineData("age", true)]
        [InlineData("MinAge", true)]
        public void AgeEvaluatesValueTest(string value, bool expected)
        {
            var sut = NameExpression.Age;

            var actual = sut.IsMatch(value);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("stuff", false)]
        [InlineData("City", true)]
        [InlineData("city", true)]
        [InlineData("SomeCity", true)]
        public void CityEvaluatesValueTest(string value, bool expected)
        {
            var sut = NameExpression.City;

            var actual = sut.IsMatch(value);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("stuff", false)]
        [InlineData("Country", true)]
        [InlineData("country", true)]
        [InlineData("SomeCountry", true)]
        public void CountryEvaluatesValueTest(string value, bool expected)
        {
            var sut = NameExpression.Country;

            var actual = sut.IsMatch(value);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("stuff", false)]
        [InlineData("dob", true)]
        [InlineData("DOB", true)]
        [InlineData("Dob", true)]
        [InlineData("dateofbirth", true)]
        [InlineData("date_of_birth", true)]
        [InlineData("Born", true)]
        [InlineData("born", true)]
        [InlineData("DateBorn", true)]
        [InlineData("dateborn", true)]
        [InlineData("BornDate", true)]
        [InlineData("borndate", true)]
        public void DateOfBirthEvaluatesValueTest(string value, bool expected)
        {
            var sut = NameExpression.DateOfBirth;

            var actual = sut.IsMatch(value);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("stuff", false)]
        [InlineData("Domain", true)]
        [InlineData("domain", true)]
        [InlineData("CompanyDomain", true)]
        [InlineData("Company_Domain", true)]
        public void DomainEvaluatesValueTest(string value, bool expected)
        {
            var sut = NameExpression.Domain;

            var actual = sut.IsMatch(value);

            actual.Should().Be(expected);
        }

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
            var sut = NameExpression.Email;

            var actual = sut.IsMatch(value);

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
            var sut = NameExpression.FirstName;

            var actual = sut.IsMatch(value);

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
            var sut = NameExpression.Gender;

            var actual = sut.IsMatch(value);

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
            var sut = NameExpression.LastName;

            var actual = sut.IsMatch(value);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("stuff", false)]
        [InlineData("postcode", true)]
        [InlineData("PostCode", true)]
        [InlineData("Zip", true)]
        [InlineData("zip", true)]
        [InlineData("ZipCode", true)]
        [InlineData("zipcode", true)]
        public void PostCodeEvaluatesValueTest(string value, bool expected)
        {
            var sut = NameExpression.PostCode;

            var actual = sut.IsMatch(value);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("stuff", false)]
        [InlineData("state", true)]
        [InlineData("State", true)]
        [InlineData("CurrentState", true)]
        [InlineData("region", true)]
        [InlineData("Region", true)]
        [InlineData("CurrentRegion", true)]
        public void StateEvaluatesValueTest(string value, bool expected)
        {
            var sut = NameExpression.State;

            var actual = sut.IsMatch(value);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("stuff", false)]
        [InlineData("timezone", true)]
        [InlineData("time_zone", true)]
        [InlineData("TimeZone", true)]
        [InlineData("Time_Zone", true)]
        public void TimeZoneEvaluatesValueTest(string value, bool expected)
        {
            var sut = NameExpression.TimeZone;

            var actual = sut.IsMatch(value);

            actual.Should().Be(expected);
        }
    }
}
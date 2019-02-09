namespace ModelBuilder.UnitTests
{
    using System;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using Xunit;

    public class CreationRuleTests
    {
        [Fact]
        public void CreateReturnsValueFromConstructorTest()
        {
            var type = typeof(string);
            var name = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;
            var expected = Guid.NewGuid().ToString();

            var target = new CreationRule((checkType, referenceName) => true, priority, expected);

            var actual = target.Create(type, name, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateReturnsValueFromLiteralValueWithRegularExpressionMatchTest()
        {
            var type = typeof(string);
            var name = "FirstName";
            var expression = new Regex(".+Name");
            var priority = Environment.TickCount;
            var expected = Guid.NewGuid().ToString();

            var target = new CreationRule(typeof(string), expression, priority, expected);

            var actual = target.Create(type, name, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateReturnsValueFromLiteralValueWithStringMatchTest()
        {
            var type = typeof(string);
            var name = "FirstName";
            var priority = Environment.TickCount;
            var expected = Guid.NewGuid().ToString();

            var target = new CreationRule(typeof(string), name, priority, expected);

            var actual = target.Create(type, name, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateReturnsValueFromValueExpressionWithMatchExpressionTest()
        {
            var type = typeof(string);
            var name = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;
            var expected = Guid.NewGuid().ToString();

            var target = new CreationRule((checkType, referenceName) => true,
                priority,
                (createType, referenceName, context) => expected);

            var actual = target.Create(type, name, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateReturnsValueFromValueExpressionWithRegularExpressionMatchTest()
        {
            var type = typeof(string);
            var name = "FirstName";
            var expression = new Regex(".+Name");
            var priority = Environment.TickCount;
            var expected = Guid.NewGuid().ToString();

            var target = new CreationRule(typeof(string),
                expression,
                priority,
                (createType, referenceName, context) => expected);

            var actual = target.Create(type, name, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateReturnsValueFromValueExpressionWithStringMatchTest()
        {
            var type = typeof(string);
            var name = "FirstName";
            var priority = Environment.TickCount;
            var expected = Guid.NewGuid().ToString();

            var target = new CreationRule(typeof(string),
                name,
                priority,
                (createType, referenceName, context) => expected);

            var actual = target.Create(type, name, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateThrowsExceptionWhenRuleDoesNotMatchCriteriaTest()
        {
            var target = new CreationRule(typeof(string), string.Empty, 10, (object) null);

            Action action = () => target.Create(typeof(Guid), string.Empty, null);

            action.Should().Throw<NotSupportedException>();
        }

        [Theory]
        [InlineData(typeof(string), "stuff", typeof(int), "no", false)]
        [InlineData(typeof(string), "stuff", typeof(string), "no", false)]
        [InlineData(typeof(string), "stuff", typeof(int), "stuff", false)]
        [InlineData(typeof(string), "stuff", typeof(string), "stuff", true)]
        public void IsMatchReturnsWhetherTypeAndNameMatchTest(
            Type type,
            string name,
            Type matchType,
            string matchName,
            bool expected)
        {
            var priority = Environment.TickCount;

            var target = new CreationRule(type, name, priority, (object) null);

            var actual = target.IsMatch(matchType, matchName);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(string), "stuff", typeof(int), "no", false)]
        [InlineData(typeof(string), "stuff", typeof(string), "no", false)]
        [InlineData(typeof(string), "stuff", typeof(int), "stuff", false)]
        [InlineData(typeof(string), "stuff", typeof(string), "stuff", true)]
        [InlineData(typeof(string), "First.+", typeof(string), "FirstName", true)]
        [InlineData(typeof(string), "(F|f)irst[_]?(N|n)ame", typeof(string), "FirstName", true)]
        [InlineData(typeof(string), "(F|f)irst[_]?(N|n)ame", typeof(string), "First_Name", true)]
        [InlineData(typeof(string), "(F|f)irst[_]?(N|n)ame", typeof(string), "First_name", true)]
        [InlineData(typeof(string), "(F|f)irst[_]?(N|n)ame", typeof(string), "Firstname", true)]
        [InlineData(typeof(string), "(F|f)irst[_]?(N|n)ame", typeof(string), "first_name", true)]
        [InlineData(typeof(string), "(F|f)irst[_]?(N|n)ame", typeof(string), "first_Name", true)]
        [InlineData(typeof(string), "(F|f)irst[_]?(N|n)ame", typeof(string), "firstname", true)]
        public void IsMatchReturnsWhetherTypeAndRegularExpressionMatchTest(
            Type type,
            string expression,
            Type matchType,
            string matchName,
            bool expected)
        {
            var priority = Environment.TickCount;
            var regex = new Regex(expression);

            var target = new CreationRule(type, regex, priority, (object) null);

            var actual = target.IsMatch(matchType, matchName);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ReturnsConstructorValuesTest()
        {
            var type = typeof(string);
            var name = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var target = new CreationRule(type, name, priority, (object) null);

            target.Priority.Should().Be(priority);
        }

        [Fact]
        public void ThrowsExceptionWithNullCreatorForEvaluatorCreatorConstructorTest()
        {
            Action action = () => new CreationRule((type, referenceName) => true, 10, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullCreatorForExpressionAndCreatorConstructorTest()
        {
            Action action = () => new CreationRule(typeof(string), new Regex(".+"), 10, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullCreatorForReferenceNameAndCreatorConstructorTest()
        {
            Action action = () => new CreationRule(typeof(string), Guid.NewGuid().ToString(), 10, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullEvaluatorForEvaluatorCreatorConstructorTest()
        {
            Action action = () => new CreationRule(null, 10, (type, referenceName, context) => null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullEvaluatorForEvaluatorValueConstructorTest()
        {
            Action action = () => new CreationRule(null, 10, (object) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullTypeAndExpressionForExpressionAndCreatorConstructorTest()
        {
            Action action = () => new CreationRule(null, (Regex) null, 10, (type, referenceName, context) => null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullTypeAndExpressionForExpressionAndValueConstructorTest()
        {
            Action action = () => new CreationRule(null, (Regex) null, 10, (object) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullTypeAndReferenceNameForReferenceNameAndCreatorConstructorTest()
        {
            Action action = () => new CreationRule(null, (string) null, 10, (type, referenceName, context) => null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullTypeAndReferenceNameForReferenceNameAndValueConstructorTest()
        {
            Action action = () => new CreationRule(null, (string) null, 10, (object) null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
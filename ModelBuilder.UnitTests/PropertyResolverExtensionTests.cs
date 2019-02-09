namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using Xunit;

    public class PropertyResolverExtensionTests
    {
        [Theory]
        [InlineData(typeof(Person), "Stuff", false)]
        [InlineData(typeof(WithStatic), "Second", false)]
        [InlineData(typeof(WithStatic), "First", true)]
        [InlineData(typeof(Person), "FirstName", true)]
        public void GetPropertiesReturnsAvailablePropertiesTest(Type type, string propertyName, bool expected)
        {
            var target = new DefaultPropertyResolver();

            var properties = target.GetProperties(type);

            var actual = properties.Any(x => x.Name == propertyName);

            actual.Should().Be(expected);
        }

        [Fact]
        public void GetPropertiesThrowsExceptionWithNullResolverTest()
        {
            var type = typeof(Person);

            var target = (IPropertyResolver) null;

            Action action = () => target.GetProperties(type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetPropertiesThrowsExecptionWithNullTypeTest()
        {
            var target = new DefaultPropertyResolver();

            Action action = () => target.GetProperties(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(Person), "Stuff", false)]
        [InlineData(typeof(WithStatic), "Second", false)]
        [InlineData(typeof(WithStatic), "First", true)]
        [InlineData(typeof(Person), "Name$", true)]
        [InlineData(typeof(Person), "(First|Last)Name", true)]
        [InlineData(typeof(Person), "(F|f)irstName", true)]
        public void GetPropertiesWithExpressionReturnsPropertyMatchTest(Type type, string expression, bool shouldExist)
        {
            var regex = new Regex(expression);

            var target = new DefaultPropertyResolver();

            var actual = target.GetProperties(type, regex);

            if (shouldExist)
            {
                actual.Should().NotBeEmpty();
            }
            else
            {
                actual.Should().BeEmpty();
            }
        }

        [Fact]
        public void GetPropertiesWithExpressionThrowsExceptionWithNullResolverTest()
        {
            var type = typeof(Person);
            var expression = new Regex("Stuff");

            var target = (IPropertyResolver) null;

            Action action = () => target.GetProperties(type, expression);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetPropertiesWithExpressionThrowsExecptionWithNullExpressionTest()
        {
            var type = typeof(Person);

            var target = new DefaultPropertyResolver();

            Action action = () => target.GetProperties(type, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetPropertiesWithExpressionThrowsExecptionWithNullTypeTest()
        {
            var expression = new Regex("Stuff");

            var target = new DefaultPropertyResolver();

            Action action = () => target.GetProperties(null, expression);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
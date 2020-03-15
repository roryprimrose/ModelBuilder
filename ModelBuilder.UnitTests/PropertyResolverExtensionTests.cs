namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
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
            var sut = new DefaultPropertyResolver();

            var properties = sut.GetProperties(type);

            var actual = properties.Any(x => x.Name == propertyName);

            actual.Should().Be(expected);
        }

        [Fact]
        public void GetPropertiesThrowsExceptionWithNullResolver()
        {
            var type = typeof(Person);

            Action action = () => ((IPropertyResolver) null).GetProperties(type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetPropertiesThrowsExceptionWithNullType()
        {
            var sut = new DefaultPropertyResolver();

            Action action = () => sut.GetProperties(null);

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

            var sut = new DefaultPropertyResolver();

            var actual = sut.GetProperties(type, regex);

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
        public void GetPropertiesWithExpressionThrowsExceptionWithNullExpression()
        {
            var type = typeof(Person);

            var sut = new DefaultPropertyResolver();

            Action action = () => sut.GetProperties(type, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetPropertiesWithExpressionThrowsExceptionWithNullResolver()
        {
            var type = typeof(Person);
            var expression = new Regex("Stuff");

            Action action = () => ((IPropertyResolver) null).GetProperties(type, expression);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetPropertiesWithExpressionThrowsExceptionWithNullType()
        {
            var expression = new Regex("Stuff");

            var sut = new DefaultPropertyResolver();

            Action action = () => sut.GetProperties(null, expression);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
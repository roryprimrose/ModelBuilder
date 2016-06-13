using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class ObjectExtensionTests
    {
        [Theory]
        [InlineData(typeof(Person), "Stuff", false)]
        [InlineData(typeof(WithStatic), "Second", false)]
        [InlineData(typeof(WithStatic), "First", true)]
        [InlineData(typeof(Person), "Name$", true)]
        [InlineData(typeof(Person), "(First|Last)Name", true)]
        [InlineData(typeof(Person), "(F|f)irstName", true)]
        public void FindPropertiesReturnsPropertyMatchTest(Type type, string expression, bool shouldExist)
        {
            var regex = new Regex(expression);

            var target = Activator.CreateInstance(type);

            var actual = target.FindProperties(regex);

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
        public void FindPropertiesThrowsExceptionWithNullSourceTest()
        {
            var regex = new Regex("Stuff");

            var target = (object) null;

            Action action = () => target.FindProperties(regex);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void FindPropertiesThrowsExecptionWithNullNameTest()
        {
            var target = new object();

            Action action = () => target.FindProperties(null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
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

        [Theory]
        [InlineData(typeof(Person), "Stuff", false)]
        [InlineData(typeof(WithStatic), "Second", false)]
        [InlineData(typeof(WithStatic), "First", true)]
        [InlineData(typeof(Person), "FirstName", true)]
        [InlineData(typeof(Person), "firstName", true)]
        public void FindPropertyReturnsPropertyMatchTest(Type type, string name, bool shouldExist)
        {
            var target = Activator.CreateInstance(type);

            var actual = target.FindProperty(name);

            if (shouldExist)
            {
                actual.Should().NotBeNull();
            }
            else
            {
                actual.Should().BeNull();
            }
        }

        [Fact]
        public void FindPropertyThrowsExceptionWithNullSourceTest()
        {
            var target = (object) null;

            Action action = () => target.FindProperty("Stuff");

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void FindPropertyThrowsExecptionWithNullNameTest()
        {
            var target = new object();

            Action action = () => target.FindProperty(null);

            action.ShouldThrow<ArgumentNullException>();
        }


        [Theory]
        [InlineData(typeof(Person), "Stuff", false)]
        [InlineData(typeof(WithStatic), "Second", false)]
        [InlineData(typeof(WithStatic), "First", true)]
        [InlineData(typeof(Person), "Name$", true)]
        [InlineData(typeof(Person), "(First|Last)Name", true)]
        [InlineData(typeof(Person), "(F|f)irstName", true)]
        public void HasPropertyWithExpressionReturnsPropertyMatchTest(Type type, string expression, bool shouldExist)
        {
            var regex = new Regex(expression);

            var target = Activator.CreateInstance(type);

            var actual = target.HasProperty(regex);

            actual.Should().Be(shouldExist);
        }

        [Fact]
        public void HasPropertyWithExpressionThrowsExceptionWithNullSourceTest()
        {
            var regex = new Regex("Stuff");

            var target = (object) null;

            Action action = () => target.HasProperty(regex);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void HasPropertyWithExpressionThrowsExecptionWithNullNameTest()
        {
            var target = new object();

            Action action = () => target.HasProperty((Regex) null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(Person), "Stuff", false)]
        [InlineData(typeof(WithStatic), "Second", false)]
        [InlineData(typeof(WithStatic), "First", true)]
        [InlineData(typeof(Person), "FirstName", true)]
        [InlineData(typeof(Person), "firstName", true)]
        public void HasPropertyWithNameReturnsPropertyMatchTest(Type type, string name, bool shouldExist)
        {
            var target = Activator.CreateInstance(type);

            var actual = target.HasProperty(name);

            actual.Should().Be(shouldExist);
        }

        [Fact]
        public void HasPropertyWithNameThrowsExceptionWithNullSourceTest()
        {
            var target = (object) null;

            Action action = () => target.HasProperty("Stuff");

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void HasPropertyWithNameThrowsExecptionWithNullNameTest()
        {
            var target = new object();

            Action action = () => target.HasProperty((string) null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
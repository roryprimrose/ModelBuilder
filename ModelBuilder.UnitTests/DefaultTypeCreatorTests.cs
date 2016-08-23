namespace ModelBuilder.UnitTests
{
    using System;
    using System.IO;
    using FluentAssertions;
    using Xunit;

    public class DefaultTypeCreatorTests
    {
        [Fact]
        public void CreateReturnsInstanceCreatedWithDefaultConstructorTest()
        {
            var target = new DefaultTypeCreator();

            var actual = target.Create(typeof(Person), null, null);

            actual.Should().NotBeNull();
        }

        [Fact]
        public void CreateReturnsInstanceCreatedWithMatchingParameterConstructorTest()
        {
            var args = new object[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                DateTime.UtcNow,
                true,
                Guid.NewGuid(),
                Environment.TickCount
            };

            var target = new DefaultTypeCreator();

            var actual = target.Create(typeof(Person), null, null, args);

            actual.Should().BeOfType<Person>();

            var person = (Person) actual;

            person.FirstName.Should().Be((string) args[0]);
            person.LastName.Should().Be((string) args[1]);
            person.DOB.Should().Be((DateTime) args[2]);
            person.IsActive.Should().Be((bool) args[3]);
            person.Id.Should().Be((Guid) args[4]);
            person.Priority.Should().Be((int) args[5]);
        }


        [Fact]
        public void CreateThrowsExceptionWhenNoAppropriateConstructorFoundTest()
        {
            var args = new object[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid(),
                Environment.TickCount
            };

            var target = new DefaultTypeCreator();

            Action action = () => target.Create(typeof(Person), null, null, args);

            action.ShouldThrow<MissingMemberException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenNoTypeNotSupportedTest()
        {
            var target = new DefaultTypeCreator();

            Action action = () => target.Create(typeof(Stream), null, null);

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullTypeTest()
        {
            var target = new DefaultTypeCreator();

            Action action = () => target.Create(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
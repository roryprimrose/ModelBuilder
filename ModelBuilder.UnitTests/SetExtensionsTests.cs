namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class SetExtensionsTests
    {
        [Fact]
        public void SetAppliesActionToInstance()
        {
            var instance = new Person();

            instance.Set(x => x.Age = 42);

            instance.Age.Should().Be(42);
        }

        [Fact]
        public void SetReturnsSameInstance()
        {
            var instance = new Person();

            var actual = instance.Set(x => x.Age = 1);

            actual.Should().BeSameAs(instance);
        }

        [Fact]
        public void SetThrowsWithNullAction()
        {
            var instance = new Person();

            Action action = () => instance.Set(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetThrowsWithNullInstance()
        {
            Person instance = null!;

            Action action = () => instance.Set(x => x.Age = 1);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Person
        {
            public int Age { get; set; }
        }
    }
}

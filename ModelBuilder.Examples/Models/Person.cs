namespace ModelBuilder.Examples.Models
{
    using System;

    /// <summary>
    ///     A sample person model used across the examples. It mixes name fields (filled with
    ///     entity-style data), an <see cref="Email" /> derived from those names, and a nested
    ///     <see cref="Address" /> so the examples can show whole-graph building.
    /// </summary>
    public class Person
    {
        public Person()
        {
        }

        public Person(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public int Age { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Address Address { get; set; } = new();
    }
}

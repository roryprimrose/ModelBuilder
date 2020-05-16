namespace ModelBuilder.UnitTests.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Constructors are called indirectly.")]
    public class WithMixedValueParameters
    {
        public WithMixedValueParameters()
        {
        }

        public WithMixedValueParameters(string someIgnoredValue)
        {
            FirstName = Guid.NewGuid().ToString();
        }

        public WithMixedValueParameters(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public WithMixedValueParameters(string firstName, string lastName, DateTime dob, bool isActive, Guid id)
        {
            FirstName = firstName;
            LastName = lastName;
            DOB = dob;
            IsActive = isActive;
            Id = id;
        }

        public int Age => DateTime.UtcNow.Subtract(DOB).Days / 365;

        public DateTime DOB { get; set; }

        public string? FirstName { get; set; }

        public Guid Id { get; set; }

        public bool IsActive { get; set; }

        public string? LastName { get; set; }
    }
}
using System;

namespace ModelBuilder.UnitTests
{
    public class WithMixedValueParameters
    {
        public WithMixedValueParameters()
        {
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

        public int Age => DateTime.UtcNow.Subtract(DOB).Days/365;

        public DateTime DOB { get; set; }

        public string FirstName { get; set; }

        public Guid Id { get; set; }

        public bool IsActive { get; set; }

        public string LastName { get; set; }
    }
}
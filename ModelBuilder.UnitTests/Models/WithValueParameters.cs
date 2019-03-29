namespace ModelBuilder.UnitTests.Models
{
    using System;

    public class WithValueParameters
    {
        public WithValueParameters(
            string firstName,
            string lastName,
            DateTime dob,
            bool isActive,
            Guid id,
            int priority)
        {
            FirstName = firstName;
            LastName = lastName;
            DOB = dob;
            IsActive = isActive;
            Id = id;
            Priority = priority;
        }

        public int Age => DateTime.UtcNow.Subtract(DOB).Days / 365;

        public DateTime DOB { get; set; }

        public string FirstName { get; set; }

        public Guid Id { get; set; }

        public bool IsActive { get; set; }

        public string LastName { get; set; }

        public int Priority { get; set; }
    }
}
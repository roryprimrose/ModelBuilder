using System;

namespace ModelBuilder.UnitTests
{
    public class Person : Entity
    {
        public Person()
        {
        }

        public Person(Entity entity)
        {
            Id = entity.Id;
            IsActive = entity.IsActive;
        }

        public Person(string firstName, string lastName, DateTime dob, bool isActive, Guid id, int priority)
        {
            FirstName = firstName;
            LastName = lastName;
            DOB = dob;
            IsActive = isActive;
            Id = id;
            Priority = priority;
        }

        public int Age => DateTime.UtcNow.Subtract(DOB).Days/365;

        public DateTime DOB { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public SimpleEnum Order { get; set; }

        public string PersonalEmail { get; set; }

        public int Priority { get; set; }

        public string WorkEmail { get; set; }
    }
}
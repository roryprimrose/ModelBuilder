namespace ModelBuilder.UnitTests.Models
{
    using System;

    public class Simple
    {
        public int Age => DateTime.UtcNow.Subtract(DOB).Days / 365;

        public DateTime DOB { get; set; }

        public string? FirstName { get; set; }

        public Guid Id { get; set; }

        public bool IsActive { get; set; }

        public string? LastName { get; set; }

        public double Priority { get; set; }
    }
}
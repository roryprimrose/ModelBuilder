namespace ModelBuilder.UnitTests.Models
{
    using System;

    public class PersonWithoutGender
    {
        public DateTime DOB { get; set; }

        public string? FirstName { get; set; } = null;

        public string? LastName { get; set; } = null;

        public string? Mobile { get; set; } = null;

        public SimpleEnum Order { get; set; }

        public string? PersonalEmail { get; set; } = null;

        public string? Phone { get; set; } = null;

        public int Priority { get; set; }
    }
}
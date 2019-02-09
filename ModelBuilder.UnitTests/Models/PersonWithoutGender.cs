namespace ModelBuilder.UnitTests
{
    using System;

    public class PersonWithoutGender
    {
        public DateTime DOB { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Mobile { get; set; }

        public SimpleEnum Order { get; set; }

        public string PersonalEmail { get; set; }

        public string Phone { get; set; }

        public int Priority { get; set; }
    }
}
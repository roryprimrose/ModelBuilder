namespace ModelBuilder.UnitTests.Models
{
    using System;

    public class WithConstructorParameters
    {
        public WithConstructorParameters(Company firstCompany, Guid id, int? refNumber, int number, bool value)
        {
            First = firstCompany;
            Id = id;
            RefNumber = refNumber;
            Number = number;
            Value = value;
        }

        public Person Customer { get; set; }

        public Company First { get; set; }

        public Guid Id { get; set; }

        public int Number { get; set; }

        public int OtherNumber { get; set; }

        public int? RefNumber { get; set; }

        public Company Second { get; set; }

        public bool Value { get; set; }
    }
}
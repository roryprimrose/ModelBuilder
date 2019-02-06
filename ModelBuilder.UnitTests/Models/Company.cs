namespace ModelBuilder.UnitTests
{
    using System.Collections.Generic;

    public class Company
    {
        public string Address { get; set; }

        public string Name { get; set; }

        public IEnumerable<Person> Staff { get; set; }
    }
}
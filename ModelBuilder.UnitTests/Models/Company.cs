using System.Collections.Generic;

namespace ModelBuilder.UnitTests
{
    public class Company
    {
        public string Address { get; set; }
        public string Name { get; set; }

        public IEnumerable<Person> Staff { get; set; }
    }
}
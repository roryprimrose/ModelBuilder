namespace ModelBuilder.UnitTests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class EnumerableParent
    {
        public Collection<Person> Collection { get; set; }
        public IEnumerable<Person> Enumerable { get; set; }

        public ICollection<Person> InterfaceCollection { get; set; }

        public IList<Person> InterfaceList { get; set; }
        public List<Person> List { get; set; }
    }
}
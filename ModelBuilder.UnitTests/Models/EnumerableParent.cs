namespace ModelBuilder.UnitTests.Models
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage(
        "Microsoft.Usage",
        "CA2227",
        Justification = "This base type is used specifically for testing scenarios for that data type.")]
    public class EnumerableParent
    {
        public Collection<Person> Collection { get; set; }

        public IEnumerable<Person> Enumerable { get; set; }

        public ICollection<Person> InterfaceCollection { get; set; }

        public IList<Person> InterfaceList { get; set; }

        public List<Person> List { get; set; }
    }
}
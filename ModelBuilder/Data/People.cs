using System.Collections.Generic;
using System.Xml.Serialization;

namespace ModelBuilder.Data
{
    [XmlRoot("People")]
    public class People
    {
        public People()
        {
            Items = new List<Person>();
        }

        [XmlElement("Person")]
        public List<Person> Items { get; set; }
    }
}
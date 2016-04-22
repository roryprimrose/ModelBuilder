using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace ModelBuilder.Data
{
    /// <summary>
    /// The <see cref="People"/>
    /// class is used to assist in deserializing test data of random people.
    /// </summary>
    [XmlRoot("People")]
    public class People
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="People"/> class.
        /// </summary>
        public People()
        {
            Items = new Collection<Person>();
        }

        /// <summary>
        /// Gets or sets the available people.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "Setter is required to support serialization.")]
        [XmlElement("Person")]
        public Collection<Person> Items { get; set; }
    }
}
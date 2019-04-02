namespace ModelBuilder.UnitTests.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Constructors are called indirectly.")]
    public class Other
    {
        public Other(Other source)
        {
        }

        public Other(Other source, string value)
        {
        }

        public Other(Guid id, string value, int priority)
        {
        }

        public Other(Guid id, string value, int priority, Stream data)
        {
        }

        private Other()
        {
        }

        public Other Create()
        {
            return new Other {Value = Value};
        }

        public string Value { get; set; }
    }
}
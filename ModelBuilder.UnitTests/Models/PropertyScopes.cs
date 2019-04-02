namespace ModelBuilder.UnitTests.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class PropertyScopes
    {
        public PropertyScopes()
        {
            PrivateSet = Guid.Empty;
            GlobalValue = Guid.Empty;
        }

        public static Guid GlobalValue { get; set; }

        [SuppressMessage("Microsoft.Design",
            "CA1822",
            Justification = "The code is written in this way to validate a test scenario.")]
        public Guid CannotSetValue => Guid.Empty;

        public Guid PrivateSet { get; }

        public Guid Public { get; set; }
    }
}
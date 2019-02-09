namespace ModelBuilder.UnitTests
{
    using System;

    public class PropertyScopes
    {
        public PropertyScopes()
        {
            PrivateSet = Guid.Empty;
            GlobalValue = Guid.Empty;
        }

        public static Guid GlobalValue { get; set; }

        public Guid PrivateSet { get; }

        public Guid Public { get; set; }

        public Guid ReadOnly => Guid.Empty;
    }
}
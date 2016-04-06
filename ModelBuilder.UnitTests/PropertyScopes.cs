using System;

namespace ModelBuilder.UnitTests
{
    public class PropertyScopes
    {
        public PropertyScopes()
        {
            InternalSet = Guid.Empty;
            GlobalValue = Guid.Empty;
        }

        public Guid InternalSet { get; private set; }

        public Guid Public { get; set; }

        public Guid ReadOnly => Guid.Empty;

        public static Guid GlobalValue { get; set; }
    }
}
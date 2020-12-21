using System;
using System.Collections.Generic;
using System.Text;

namespace ModelBuilder.UnitTests.Models
{
    public class EntityWithInternalConstructor
    {
        internal EntityWithInternalConstructor(Guid entityId, string entityName)
        {
            EntityId = entityId;
            EntityName = entityName;
        }

        public Guid EntityId { get; }

        public string EntityName { get; }
    }
}

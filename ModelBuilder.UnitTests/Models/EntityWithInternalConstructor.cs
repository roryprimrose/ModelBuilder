namespace ModelBuilder.UnitTests.Models
{
    using System;

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
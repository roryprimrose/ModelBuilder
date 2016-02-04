using System;

namespace ModelBuilder.UnitTests
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
    }
}
namespace ModelBuilder.UnitTests
{
    using System;

    public abstract class Entity
    {
        public Guid Id { get; set; }

        public bool IsActive { get; set; }
    }
}
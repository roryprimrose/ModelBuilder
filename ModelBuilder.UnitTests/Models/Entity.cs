namespace ModelBuilder.UnitTests.Models
{
    using System;

    public abstract class Entity
    {
        public Guid Id { get; set; }

        public bool IsActive { get; set; }
    }
}
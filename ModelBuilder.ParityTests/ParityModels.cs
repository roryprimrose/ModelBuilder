namespace ModelBuilder.ParityTests
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The model shapes used to compare the v8 reflection engine against the vNext source-generated
    ///     engine. They are public so both engines can build them.
    /// </summary>
    public enum Priority
    {
        Low,
        Medium,
        High
    }

    public sealed class FlatPoco
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public Guid Token { get; set; }

        public bool IsActive { get; set; }
    }

    public sealed class Address
    {
        public string? Street { get; set; }

        public string? City { get; set; }
    }

    public sealed class NestedPoco
    {
        public string? Title { get; set; }

        public Address? Location { get; set; }
    }

    public sealed class CollectionPoco
    {
        public List<string>? Tags { get; set; }

        public List<Address>? Places { get; set; }
    }

    public sealed class EnumPoco
    {
        public Priority Priority { get; set; }
    }
}

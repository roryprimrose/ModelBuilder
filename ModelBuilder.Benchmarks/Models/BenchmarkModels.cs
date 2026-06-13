namespace ModelBuilder.Benchmarks.Models
{
    using System;
    using System.Collections.Generic;

    // The model shapes below span the spread of graph sizes/complexities the baseline
    // measures: a flat primitive POCO, a small nested graph, a deep/wide graph, one with
    // collections, one with enums/nullable, and one with constructor-arg matching. They are
    // intentionally simple data holders so the numbers reflect ModelBuilder's traversal and
    // value-generation cost rather than any behaviour of the models themselves.

    public enum Priority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum Status
    {
        Pending,
        Active,
        Suspended,
        Closed
    }

    /// <summary>A flat POCO of primitive properties only.</summary>
    public sealed class FlatPoco
    {
        public int Id { get; set; }

        public long Reference { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public bool IsEnabled { get; set; }

        public double Ratio { get; set; }

        public decimal Amount { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public Guid Token { get; set; }

        public TimeSpan Duration { get; set; }
    }

    public sealed class Address
    {
        public string? Line1 { get; set; }

        public string? Line2 { get; set; }

        public string? City { get; set; }

        public string? PostCode { get; set; }

        public string? Country { get; set; }
    }

    public sealed class Contact
    {
        public string? Email { get; set; }

        public string? Phone { get; set; }
    }

    /// <summary>A small graph: a root with a couple of nested references one level deep.</summary>
    public sealed class SmallNested
    {
        public Guid Id { get; set; }

        public string? DisplayName { get; set; }

        public Address? HomeAddress { get; set; }

        public Contact? PrimaryContact { get; set; }
    }

    public sealed class Level3
    {
        public int Value { get; set; }

        public string? Label { get; set; }
    }

    public sealed class Level2
    {
        public Level3? First { get; set; }

        public Level3? Second { get; set; }

        public Level3? Third { get; set; }
    }

    public sealed class Level1
    {
        public Level2? Left { get; set; }

        public Level2? Middle { get; set; }

        public Level2? Right { get; set; }
    }

    /// <summary>A deep and wide graph (three levels, three children per node).</summary>
    public sealed class DeepWideGraph
    {
        public Guid Id { get; set; }

        public Level1? Alpha { get; set; }

        public Level1? Beta { get; set; }

        public Level1? Gamma { get; set; }
    }

    public sealed class CollectionItem
    {
        public int Index { get; set; }

        public string? Name { get; set; }
    }

    /// <summary>A model with arrays, lists and dictionaries of nested items.</summary>
    public sealed class WithCollections
    {
        public Guid Id { get; set; }

        public int[]? Numbers { get; set; }

        public List<string>? Tags { get; set; }

        public List<CollectionItem>? Items { get; set; }

        public Dictionary<string, int>? Counts { get; set; }
    }

    /// <summary>A model exercising enums and nullable value types.</summary>
    public sealed class WithEnumsNullable
    {
        public Priority Priority { get; set; }

        public Status Status { get; set; }

        public Priority? OptionalPriority { get; set; }

        public int? OptionalCount { get; set; }

        public DateTime? OptionalDate { get; set; }

        public bool? OptionalFlag { get; set; }
    }

    /// <summary>A model whose constructor parameters match property names (constructor-arg matching).</summary>
    public sealed class CtorArgMatching
    {
        public CtorArgMatching(int id, string name, DateTimeOffset createdAt, bool isActive)
        {
            Id = id;
            Name = name;
            CreatedAt = createdAt;
            IsActive = isActive;
        }

        public int Id { get; }

        public string Name { get; }

        public DateTimeOffset CreatedAt { get; }

        public bool IsActive { get; }
    }
}

namespace ModelBuilder.IntegrationTests.Models
{
    using System.Collections.Generic;

    /// <summary>
    ///     A self-referencing type used to verify the depth/circular-reference guard prevents infinite
    ///     recursion when a member's type matches an ancestor already being built.
    /// </summary>
    public class GraphNode
    {
        public string Name { get; set; } = string.Empty;

        public GraphNode? Next { get; set; }
    }

    /// <summary>
    ///     Two mutually referencing types (rather than a single type referencing itself) used to verify
    ///     the same guard applies across a cycle spanning more than one type.
    /// </summary>
    public class Employee
    {
        public string Name { get; set; } = string.Empty;

        public Manager? Manager { get; set; }
    }

    /// <summary>
    ///     See <see cref="Employee" />.
    /// </summary>
    public class Manager
    {
        public string Name { get; set; } = string.Empty;

        public List<Employee> DirectReports { get; set; } = new();
    }
}

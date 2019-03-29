namespace ModelBuilder.UnitTests.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [Flags]
    [SuppressMessage("Microsoft.Design", "CA1028", Justification = "This base type is used specifically for testing scenarios for that data type.")]
    public enum NoValues
    {
    }
}
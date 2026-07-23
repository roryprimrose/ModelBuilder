namespace ModelBuilder.IntegrationTests.Models
{
    using System;

    /// <summary>
    ///     A <c>[Flags]</c> enum used to verify flags-shaped enums build to a defined (possibly
    ///     combined) value rather than an arbitrary out-of-range integer.
    /// </summary>
    [Flags]
    public enum Permissions
    {
        None = 0,

        Read = 1,

        Write = 2,

        Execute = 4,

        All = Read | Write | Execute
    }
}

namespace ModelBuilder.UnitTests
{
    using System;
    using ModelBuilder.UnitTests.Models;

    public interface IEntityResolver
    {
        Entity ResolveById(Guid id);
    }
}
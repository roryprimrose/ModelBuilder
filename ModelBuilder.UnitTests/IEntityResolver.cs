namespace ModelBuilder.UnitTests
{
    using System;
    using Models;

    public interface IEntityResolver
    {
        Entity ResolveById(Guid id);
    }
}
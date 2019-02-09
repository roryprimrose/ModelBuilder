namespace ModelBuilder.UnitTests
{
    using System;

    public interface IEntityResolver
    {
        Entity ResolveById(Guid id);
    }
}
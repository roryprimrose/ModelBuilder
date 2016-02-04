using System;

namespace ModelBuilder.UnitTests
{
    public interface IEntityResolver
    {
        Entity ResolveById(Guid id);
    }
}
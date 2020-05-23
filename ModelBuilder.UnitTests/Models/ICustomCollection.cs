namespace ModelBuilder.UnitTests.Models
{
    using System.Collections.Generic;

    public interface ICustomCollection<T> : IEnumerable<T> where T : class
    {
    }
}
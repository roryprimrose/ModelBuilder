namespace ModelBuilder.Examples.Models
{
    using System.Collections.Generic;

    /// <summary>
    ///     A sample organisation with a <see cref="Staff" /> collection, used to show collection
    ///     building and the <c>SetEach</c> post-build helpers.
    /// </summary>
    public class Organisation
    {
        public string Name { get; set; } = string.Empty;

        public List<Person> Staff { get; set; } = new();
    }
}

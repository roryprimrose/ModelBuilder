namespace ModelBuilder.IntegrationTests.Models
{
    /// <summary>
    ///     A class with a closed generic member (<c>Box&lt;Widget&gt;</c>) built with no mapping
    ///     registration, proving generic member resolution works on its own.
    /// </summary>
    public class BoxHolder
    {
        public Box<Widget>? Item { get; set; }
    }
}

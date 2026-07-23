namespace ModelBuilder.IntegrationTests.Models
{
    /// <summary>
    ///     An interface with a single concrete implementation, used to verify
    ///     <c>Model.Mapping&lt;TSource, TTarget&gt;()</c> resolves an interface-typed member to a
    ///     concrete built instance.
    /// </summary>
    public interface IEngine
    {
        string Name { get; }

        int Horsepower { get; }
    }

    /// <summary>
    ///     The concrete <see cref="IEngine" /> implementation mapped in the mapping scenario tests.
    /// </summary>
    public class DieselEngine : IEngine
    {
        public string Name { get; set; } = string.Empty;

        public int Horsepower { get; set; }
    }

    /// <summary>
    ///     A class with an interface-typed member that has no default concrete type, requiring a
    ///     <c>Model.Mapping&lt;,&gt;()</c> registration for the build to populate it.
    /// </summary>
    public class Vehicle
    {
        public string Model { get; set; } = string.Empty;

        public IEngine? Engine { get; set; }
    }
}

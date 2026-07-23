namespace ModelBuilder.IntegrationTests.Models
{
    /// <summary>
    ///     A value type (struct) used to verify struct roots and struct-typed members build correctly.
    /// </summary>
    public struct PointStruct
    {
        public int X { get; set; }

        public int Y { get; set; }
    }

    /// <summary>
    ///     A class holding a plain struct member and a nullable struct member, to verify both shapes
    ///     populate.
    /// </summary>
    public class ShapeProfile
    {
        public PointStruct Origin { get; set; }

        public PointStruct? Bounds { get; set; }

        public string Label { get; set; } = string.Empty;
    }
}

namespace ModelBuilder.IntegrationTests.Models
{
    using System;

    /// <summary>
    ///     Pairs a nullable member with its non-nullable counterpart so tests can assert both build
    ///     consistently under different <see cref="ModelBuilder.BuildOptions.NullPercentage" /> settings.
    /// </summary>
    public class NullableProfile
    {
        public int? NullableInt { get; set; }

        public int NonNullableInt { get; set; }

        public DateTime? NullableDate { get; set; }

        public Guid? NullableGuid { get; set; }

        public bool? NullableBool { get; set; }

        public Colour? NullableColour { get; set; }

        public string? NullableText { get; set; }

        public string NonNullableText { get; set; } = string.Empty;
    }
}

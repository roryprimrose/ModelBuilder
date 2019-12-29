namespace ModelBuilder.UnitTests.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class Location
    {
        public Uri First { get; set; }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1056",
            Justification = "The code is written in this way to validate a test scenario.")]
        public string SecondUrl { get; set; }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1056",
            Justification = "The code is written in this way to validate a test scenario.")]
        public string UriThird { get; set; }
    }
}
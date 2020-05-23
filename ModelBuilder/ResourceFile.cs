namespace ModelBuilder
{
    using System.IO;
    using System.Reflection;

    internal static class ResourceFile
    {
        private static string Read(string name)
        {
            var assembly = typeof(ResourceFile).GetTypeInfo().Assembly;
            var resourceName = "ModelBuilder.Resources." + name + ".txt";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        internal static string Companies => Read(nameof(Companies));

        internal static string Domains => Read(nameof(Domains));

        internal static string FemaleNames => Read(nameof(FemaleNames));

        internal static string LastNames => Read(nameof(LastNames));

        internal static string Locations => Read(nameof(Locations));

        internal static string MaleNames => Read(nameof(MaleNames));

        internal static string TimeZones => Read(nameof(TimeZones));
    }
}
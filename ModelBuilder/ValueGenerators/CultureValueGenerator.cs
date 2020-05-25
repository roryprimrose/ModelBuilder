namespace ModelBuilder.ValueGenerators
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     The <see cref="CultureValueGenerator" />
    ///     class is used to generate random culture values.
    /// </summary>
    public class CultureValueGenerator : ValueGeneratorMatcher
    {
        private static readonly Regex _matchNameExpression = new Regex("Culture", RegexOptions.IgnoreCase);

        /// <summary>
        ///     Initializes a new instance of the <see cref="DomainNameValueGenerator" /> class.
        /// </summary>
        public CultureValueGenerator() : base(_matchNameExpression, typeof(string), typeof(CultureInfo))
        {
        }

        /// <inheritdoc />
        protected override object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName)
        {
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(x => string.IsNullOrWhiteSpace(x.Name) == false).ToList();
            var index = Generator.NextValue(0, cultures.Count - 1);
            var culture = cultures[index];

            if (type == typeof(string))
            {
                return culture.Name;
            }

            return culture;
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
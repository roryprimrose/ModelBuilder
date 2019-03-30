namespace ModelBuilder
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using ModelBuilder.Data;

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
        public CultureValueGenerator()
            : base(_matchNameExpression, typeof(string), typeof(CultureInfo))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
#if NET45
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var index = Generator.NextValue(0, cultures.Length - 1);
            var culture = cultures[index];

            if (type == typeof(string))
            {
                return culture.Name;
            }

            return culture;
#else
            var cultureName = TestData.Cultures.Next();

            if (type == typeof(string))
            {
                return cultureName;
            }

            // netstandard 1.5 does not support loading or enumerating system cultures
            // TODO: Review loading cultures under netstandard 2.0 when released
            return CultureInfo.CurrentUICulture;
#endif
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
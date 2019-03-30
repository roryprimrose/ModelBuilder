namespace ModelBuilder
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     The <see cref="GenderValueGenerator" />
    ///     class is used to generate random gender values.
    /// </summary>
    public class GenderValueGenerator : ValueGeneratorMatcher
    {
        private static readonly Regex _matchNameExpression = new Regex("Gender|Sex", RegexOptions.IgnoreCase);

        /// <summary>
        ///     Initializes a new instance of the <see cref="GenderValueGenerator" /> class.
        /// </summary>
        public GenderValueGenerator()
            : base(_matchNameExpression, typeof(string))
        {
        }

        /// <inheritdoc />
        protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
        {
            var index = Generator.NextValue(0, 1);

            if (index == 0)
            {
                return "Male";
            }

            return "Female";
        }

        /// <inheritdoc />
        public override int Priority { get; } = 1000;
    }
}
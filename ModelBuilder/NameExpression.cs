namespace ModelBuilder
{
    using System.Text.RegularExpressions;

    /// <summary>
    ///     The <see cref="NameExpression" />
    ///     class defines regular expressions for matching values.
    /// </summary>
    public static class NameExpression
    {
        /// <summary>
        ///     Defines the expression for matching age properties.
        /// </summary>
        public static readonly Regex Age = new Regex("Age", RegexOptions.IgnoreCase);

        /// <summary>
        ///     Defines the expression for matching city properties.
        /// </summary>
        public static readonly Regex City = new Regex("City", RegexOptions.IgnoreCase);

        /// <summary>
        ///     Defines the expression for matching country properties.
        /// </summary>
        public static readonly Regex Country = new Regex("Country", RegexOptions.IgnoreCase);

        /// <summary>
        ///     Defines the expression for matching DOB properties.
        /// </summary>
        public static readonly Regex DateOfBirth = new Regex("dob|date[_]?of[_]?birth|born", RegexOptions.IgnoreCase);

        /// <summary>
        ///     Defines the expression for matching domain properties.
        /// </summary>
        public static readonly Regex Domain = new Regex("Domain", RegexOptions.IgnoreCase);

        /// <summary>
        ///     Defines the expression for matching email properties.
        /// </summary>
        public static readonly Regex Email = new Regex("Email", RegexOptions.IgnoreCase);

        /// <summary>
        ///     Defines the expression for matching first/given name properties.
        /// </summary>
        public static readonly Regex FirstName = new Regex("(Given|First)[_]?Name", RegexOptions.IgnoreCase);

        /// <summary>
        ///     Defines the expression for matching gender/sex properties.
        /// </summary>
        public static readonly Regex Gender = new Regex("Gender|Sex", RegexOptions.IgnoreCase);

        /// <summary>
        ///     Defines the expression for matching last name/surname properties.
        /// </summary>
        public static readonly Regex LastName = new Regex("Surname|(Last[_]?Name)", RegexOptions.IgnoreCase);

        /// <summary>
        ///     Defines the expression for matching postcode properties.
        /// </summary>
        public static readonly Regex PostCode = new Regex("PostCode|Zip(Code)?", RegexOptions.IgnoreCase);

        /// <summary>
        ///     Defines the expression for matching state properties.
        /// </summary>
        public static readonly Regex State = new Regex("State|Region", RegexOptions.IgnoreCase);

        /// <summary>
        ///     Defines the expression for matching timezone properties.
        /// </summary>
        public static readonly Regex TimeZone = new Regex("Time[_]?Zone", RegexOptions.IgnoreCase);
    }
}
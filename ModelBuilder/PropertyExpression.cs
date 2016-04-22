using System.Text.RegularExpressions;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="PropertyExpression"/>
    /// class defines regular expressions for matching values.
    /// </summary>
    public static class PropertyExpression
    {
        /// <summary>
        /// Defines the expression for matching first/given name properties.
        /// </summary>
        public static readonly Regex FirstName = new Regex("(Given|First)[_]?Name", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Defines the expression for matching gender/sex properties.
        /// </summary>
        public static readonly Regex Gender = new Regex("Gender|Sex", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Defines the expression for matching last name/surname properties.
        /// </summary>
        public static readonly Regex LastName = new Regex("Surname|(Last[_]?Name)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Defines the expression for matching email properties.
        /// </summary>
        public static readonly Regex Email = new Regex("Email", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
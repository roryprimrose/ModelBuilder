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
        public const string FirstName = "(((G|g)iven)|((F|f)irst))[_]?(N|n)ame";

        /// <summary>
        /// Defines the expression for matching gender/sex properties.
        /// </summary>
        public const string Gender = "((G|g)ender)|((S|s)ex)";

        /// <summary>
        /// Defines the expression for matching last name/surname properties.
        /// </summary>
        public const string LastName = "((S|s)urname)|(((L|l)ast)[_]?(N|n)ame)";

        /// <summary>
        /// Defines the expression for matching email properties.
        /// </summary>
        public const string Email = "(E|e)mail";
    }
}
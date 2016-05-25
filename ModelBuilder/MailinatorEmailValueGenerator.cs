namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="MailinatorEmailValueGenerator"/>
    /// class is used to generate email addresses that always point to the mailinator.com domain.
    /// </summary>
    public class MailinatorEmailValueGenerator : EmailValueGenerator
    {
        /// <inheritdoc />
        protected override string Domain => "mailinator.com";
    }
}
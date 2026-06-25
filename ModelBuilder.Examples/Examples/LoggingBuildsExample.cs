namespace ModelBuilder.Examples.Examples
{
    using System;
    using ModelBuilder;
    using ModelBuilder.Examples.Models;

    /// <summary>
    ///     The <see cref="LoggingBuildsExample" /> class
    ///     shows rendering the structured build log with <c>Model.WriteLog</c>.
    /// </summary>
    public static class LoggingBuildsExample
    {
        /// <summary>
        ///     Runs the logging example and writes the rendered build log to the console.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine("== Logging the build process ==");

            // WriteLog sends the rendered build tree to the sink after the build completes.
            var person = Model.WriteLog(Console.WriteLine).Create<Person>();

            Console.WriteLine($"(built {person.FirstName} {person.LastName} with the log above)");
            Console.WriteLine();
        }
    }
}

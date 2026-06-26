namespace ModelBuilder.Examples.Examples
{
    using System;
    using ModelBuilder;
    using ModelBuilder.Examples.Models;

    /// <summary>
    ///     The <see cref="HandlingFailuresExample" /> class
    ///     shows catching a <see cref="ModelBuildException" /> and branching on its
    ///     <see cref="FailureKind" /> rather than the message.
    /// </summary>
    public static class HandlingFailuresExample
    {
        /// <summary>
        ///     Runs the failure-handling example and writes the diagnosis to the console.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine("== Handling build failures ==");

            // A custom value source that throws surfaces as a ModelBuildException carrying the failing
            // target and the original exception, so the cause can be diagnosed without a debugger.
            var faultySource = new DelegateValueSource<string>(_ => throw new InvalidOperationException("value source boom"));

            try
            {
                var person = Model.AddValueSource(faultySource).Create<Person>();

                Console.WriteLine($"Unexpectedly built {person}");
            }
            catch (ModelBuildException ex) when (ex.FailureKind == FailureKind.ValueSourceThrew)
            {
                Console.WriteLine($"Caught {ex.FailureKind} at {ex.TargetType?.Name}.{ex.TargetMember}: {ex.InnerException?.Message}");
            }

            Console.WriteLine();
        }
    }
}

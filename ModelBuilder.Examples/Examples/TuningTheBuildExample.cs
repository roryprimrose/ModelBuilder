namespace ModelBuilder.Examples.Examples
{
    using System;
    using ModelBuilder;
    using ModelBuilder.Examples.Models;

    /// <summary>
    ///     The <see cref="TuningTheBuildExample" /> class
    ///     shows <c>Model.SetOptions</c> for tuning collection sizes, the chance of <c>null</c> values,
    ///     and the maximum graph depth.
    /// </summary>
    public static class TuningTheBuildExample
    {
        /// <summary>
        ///     Runs the tuning example and writes the results to the console.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine("== Tuning the build ==");

            // Always populate nullables, and make collections hold exactly three items.
            var organisation = Model.SetOptions(x =>
                {
                    x.NullPercentage = 0;
                    x.MinCount = 3;
                    x.MaxCount = 3;
                })
                .Create<Organisation>();

            Console.WriteLine($"Tuned build produced exactly {organisation.Staff.Count} staff members");
            Console.WriteLine();
        }
    }
}

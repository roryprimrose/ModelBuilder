namespace ModelBuilder.Examples.Examples
{
    using System;
    using ModelBuilder;
    using ModelBuilder.Examples.Models;

    /// <summary>
    ///     The <see cref="ChangingAfterCreationExample" /> class
    ///     shows the post-build <c>Set</c> and <c>SetEach</c> helpers for tweaking an instance or a
    ///     sequence after it is built.
    /// </summary>
    public static class ChangingAfterCreationExample
    {
        /// <summary>
        ///     Runs the post-build change examples and writes the results to the console.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine("== Changing the model after creation ==");

            var person = Model.Create<Person>()
                .Set(x => x.FirstName = "Joe")
                .Set(x => x.Email = null);

            Console.WriteLine($"After Set: FirstName '{person.FirstName}', Email '{person.Email ?? "<null>"}'");

            var organisation = Model.Create<Organisation>();

            organisation.Staff.SetEach(x => x.Email = null);

            Console.WriteLine($"Cleared Email on {organisation.Staff.Count} staff members");

            // The index-aware overload passes the zero-based index alongside each item.
            organisation.Staff.SetEach((index, member) => member.Age = index + 1);

            Console.WriteLine($"Assigned sequential ages; first staff age is {organisation.Staff[0].Age}");
            Console.WriteLine();
        }
    }
}

namespace ModelBuilder.CodeFixes.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Xunit;

    public class ModelCreateArgsCodeFixProviderTests
    {
        private static readonly ImmutableArray<MetadataReference> _references = BuildReferences();

        [Fact]
        public async Task RewritesModelCreateWithArgumentsToConstructFrom()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Person
    {
        public Person(string firstName, string lastName)
        {
        }
    }

    public static class Caller
    {
        public static void Build()
        {
            var person = Model.Create<Person>(""Fred"", ""Smith"");
        }
    }
}";

            var fixedText = await ApplyFix(source);

            fixedText.Should().Contain("Model.Construct<Person>().From(\"Fred\", \"Smith\")");
            fixedText.Should().NotContain("Model.Create<Person>(\"Fred\"");
        }

        [Fact]
        public async Task RewriteKeepsASingleArgumentAndTheReceiver()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Order
    {
        public Order(System.Guid customerId)
        {
        }
    }

    public static class Caller
    {
        public static void Build(System.Guid id)
        {
            var order = global::ModelBuilder.Model.Create<Order>(id);
        }
    }
}";

            var fixedText = await ApplyFix(source);

            fixedText.Should().Contain("global::ModelBuilder.Model.Construct<Order>().From(id)");
        }

        private static async Task<string> ApplyFix(string source)
        {
            var workspace = new AdhocWorkspace();

            var project = workspace.CurrentSolution
                .AddProject("Test", "Test", LanguageNames.CSharp)
                .WithCompilationOptions(
                    new CSharpCompilationOptions(
                        OutputKind.DynamicallyLinkedLibrary,
                        nullableContextOptions: NullableContextOptions.Enable))
                .WithParseOptions(new CSharpParseOptions(LanguageVersion.Latest))
                .WithMetadataReferences(_references);

            var document = project.AddDocument("Test.cs", source);

            var compilation = await document.Project.GetCompilationAsync();
            compilation.Should().NotBeNull();

            var obsoleteErrors = compilation!.GetDiagnostics()
                .Where(diagnostic => diagnostic.Id == "CS0619")
                .ToImmutableArray();

            // Sanity check: the removed v8 overload is what produced the diagnostic the fix targets.
            obsoleteErrors.Should().NotBeEmpty();

            var provider = new ModelCreateArgsCodeFixProvider();
            var actions = new List<CodeAction>();

            foreach (var diagnostic in obsoleteErrors)
            {
                var context = new CodeFixContext(
                    document,
                    diagnostic,
                    (action, _) => actions.Add(action),
                    CancellationToken.None);

                await provider.RegisterCodeFixesAsync(context);
            }

            actions.Should().NotBeEmpty();

            var operations = await actions[0].GetOperationsAsync(CancellationToken.None);
            var applyChanges = operations.OfType<ApplyChangesOperation>().Single();
            var changedDocument = applyChanges.ChangedSolution.GetDocument(document.Id);

            changedDocument.Should().NotBeNull();

            var text = await changedDocument!.GetTextAsync();

            return text.ToString();
        }

        private static ImmutableArray<MetadataReference> BuildReferences()
        {
            var references = new List<MetadataReference>();

            var trusted = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");

            if (trusted != null)
            {
                foreach (var path in trusted.Split(Path.PathSeparator))
                {
                    if (path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        references.Add(MetadataReference.CreateFromFile(path));
                    }
                }
            }

            references.Add(MetadataReference.CreateFromFile(typeof(ModelBuilder.Model).Assembly.Location));

            return references.ToImmutableArray();
        }
    }
}

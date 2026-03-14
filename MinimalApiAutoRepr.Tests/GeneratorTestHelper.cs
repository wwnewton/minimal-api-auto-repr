namespace MinimalApiAutoRepr.Tests;

using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

internal static class GeneratorTestHelper
{
    // IEndpoint.g.cs (emitted by EndpointInterfaceGenerator) has no `using` for
    // Microsoft.AspNetCore.Builder. Types in the global namespace are accessible
    // from any namespace without a using, so we stub IEndpointRouteBuilder globally
    // to keep test compilations entirely self-contained.
    private const string GlobalStubs = "public interface IEndpointRouteBuilder { }";

    /// <summary>Compiles <paramref name="source"/> and runs all <paramref name="generators"/> against it.</summary>
    internal static GeneratorDriverRunResult RunGenerators(string source, params IIncrementalGenerator[] generators)
    {
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);

        // Use every assembly the runtime trusts so the test compilation has a full
        // set of base-class-library references without hard-coding any paths.
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p));

        var compilation = CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees:
            [
                CSharpSyntaxTree.ParseText(GlobalStubs, parseOptions),
                CSharpSyntaxTree.ParseText(source, parseOptions),
            ],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: generators.Select(g => g.AsSourceGenerator()),
            parseOptions: parseOptions);

        driver = driver.RunGenerators(compilation);
        return driver.GetRunResult();
    }
}

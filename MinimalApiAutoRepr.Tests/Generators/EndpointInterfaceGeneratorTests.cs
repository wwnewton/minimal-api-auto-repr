namespace MinimalApiAutoRepr.Tests.Generators;

using MinimalApiAutoRepr.Generators;

public class EndpointInterfaceGeneratorTests
{
    [Fact]
    public void WhenGeneratorRuns_ThenEmitsIEndpointGFile()
    {
        var result = GeneratorTestHelper.RunGenerators(string.Empty, new EndpointInterfaceGenerator());

        var file = result.Results
            .SelectMany(r => r.GeneratedSources)
            .SingleOrDefault(s => s.HintName == "IEndpoint.g.cs");

        Assert.NotEqual(default, file);
    }

    [Theory]
    [InlineData("interface IEndpoint")]
    [InlineData("interface IGroupEndpoint")]
    [InlineData("interface IGroupEndpoint<TParentGroup>")]
    [InlineData("interface IEndpoint<TGroup>")]
    public void WhenGeneratorRuns_ThenGeneratedFileContainsInterface(string expectedFragment)
    {
        var result = GeneratorTestHelper.RunGenerators(string.Empty, new EndpointInterfaceGenerator());

        var source = result.Results
            .SelectMany(r => r.GeneratedSources)
            .Single(s => s.HintName == "IEndpoint.g.cs")
            .SourceText.ToString();

        Assert.Contains(expectedFragment, source);
    }

    [Fact]
    public void WhenGeneratorRuns_ThenGeneratedFileIsInMinimalApiAutoReprGeneratedNamespace()
    {
        var result = GeneratorTestHelper.RunGenerators(string.Empty, new EndpointInterfaceGenerator());

        var source = result.Results
            .SelectMany(r => r.GeneratedSources)
            .Single(s => s.HintName == "IEndpoint.g.cs")
            .SourceText.ToString();

        Assert.Contains("MinimalApiAutoRepr.Generated", source);
    }

    [Fact]
    public void WhenGeneratorRuns_ThenNoGeneratorErrorsAreReported()
    {
        var result = GeneratorTestHelper.RunGenerators(string.Empty, new EndpointInterfaceGenerator());

        Assert.Empty(result.Diagnostics.Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error));
    }
}

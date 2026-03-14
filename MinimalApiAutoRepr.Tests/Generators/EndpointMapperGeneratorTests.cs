namespace MinimalApiAutoRepr.Tests.Generators;

using Microsoft.CodeAnalysis;
using MinimalApiAutoRepr.Generators;

public class EndpointMapperGeneratorTests
{
    // Run both generators: EndpointInterfaceGenerator emits IGroupEndpoint/IEndpoint
    // via post-initialisation output, making them visible to EndpointMapperGenerator's
    // semantic-model transforms before any source analysis runs.
    private static GeneratorDriverRunResult Run(string source) =>
        GeneratorTestHelper.RunGenerators(source, new EndpointInterfaceGenerator(), new EndpointMapperGenerator());

    private static string MappingsText(GeneratorDriverRunResult result) =>
        result.Results
            .SelectMany(r => r.GeneratedSources)
            .Single(s => s.HintName == "GeneratedEndpointMappings.g.cs")
            .SourceText.ToString();

    // ── Empty input ────────────────────────────────────────────────────────────

    [Fact]
    public void WhenNoTypes_ThenEmitsMapAutoReprEndpointsMethod()
    {
        var text = MappingsText(Run(string.Empty));

        Assert.Contains("MapAutoReprEndpoints", text);
    }

    [Fact]
    public void WhenNoTypes_ThenEmitsReturnApp()
    {
        var text = MappingsText(Run(string.Empty));

        Assert.Contains("return app;", text);
    }

    [Fact]
    public void WhenNoTypes_ThenNoGroupVarsAreEmitted()
    {
        var text = MappingsText(Run(string.Empty));

        // "var " would only appear for emitted group variables
        Assert.DoesNotContain("var ", text);
    }

    // ── Root group ─────────────────────────────────────────────────────────────

    [Fact]
    public void WhenRootGroupDefined_ThenEmitsGroupVarMappedToApp()
    {
        var source = """
            using MinimalApiAutoRepr.Generated;
            public class RootGroup : IGroupEndpoint
            {
                public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) => app;
            }
            """;

        var text = MappingsText(Run(source));

        Assert.Contains("var rootGroup = global::RootGroup.Map(app);", text);
    }

    // ── Child group (BFS ordering) ─────────────────────────────────────────────

    private const string ChildGroupSource = """
        using MinimalApiAutoRepr.Generated;
        public class ParentGroup : IGroupEndpoint
        {
            public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) => app;
        }
        public class ChildGroup : IGroupEndpoint<ParentGroup>
        {
            public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) => app;
        }
        """;

    [Fact]
    public void WhenChildGroupDefined_ThenEmitsParentGroupVar()
    {
        var text = MappingsText(Run(ChildGroupSource));

        Assert.Contains("var parentGroup = global::ParentGroup.Map(app);", text);
    }

    [Fact]
    public void WhenChildGroupDefined_ThenEmitsChildMappedToParentVar()
    {
        var text = MappingsText(Run(ChildGroupSource));

        Assert.Contains("var childGroup = global::ChildGroup.Map(parentGroup);", text);
    }

    [Fact]
    public void WhenChildGroupDefined_ThenParentIsEmittedBeforeChild()
    {
        var text = MappingsText(Run(ChildGroupSource));

        var parentIndex = text.IndexOf("var parentGroup", StringComparison.Ordinal);
        var childIndex = text.IndexOf("var childGroup", StringComparison.Ordinal);

        Assert.True(parentIndex < childIndex, "Parent group must be emitted before its child (BFS order).");
    }

    // ── Endpoints ──────────────────────────────────────────────────────────────

    [Fact]
    public void WhenEndpointWithGroupType_ThenMapsEndpointToGroupVar()
    {
        var source = """
            using MinimalApiAutoRepr.Generated;
            public class MyGroup : IGroupEndpoint
            {
                public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) => app;
            }
            public class MyEndpoint : IEndpoint<MyGroup>
            {
                public static void Map(IEndpointRouteBuilder app) { }
            }
            """;

        var text = MappingsText(Run(source));

        Assert.Contains("global::MyEndpoint.Map(myGroup);", text);
    }

    [Fact]
    public void WhenStandaloneEndpoint_ThenMapsEndpointToApp()
    {
        var source = """
            using MinimalApiAutoRepr.Generated;
            public class StandaloneEndpoint : IEndpoint
            {
                public static void Map(IEndpointRouteBuilder app) { }
            }
            """;

        var text = MappingsText(Run(source));

        Assert.Contains("global::StandaloneEndpoint.Map(app);", text);
    }

    // ── Cycle detection (MAAR001) ──────────────────────────────────────────────

    [Fact]
    public void WhenGroupParentsFormCycle_ThenEmitsMAAR001Diagnostic()
    {
        var source = """
            using MinimalApiAutoRepr.Generated;
            public class GroupA : IGroupEndpoint<GroupB>
            {
                public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) => app;
            }
            public class GroupB : IGroupEndpoint<GroupA>
            {
                public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) => app;
            }
            """;

        var result = Run(source);

        Assert.Contains(result.Diagnostics, d => d.Id == "MAAR001");
    }

    [Fact]
    public void WhenGroupParentsFormCycle_ThenEmitsDiagnosticForEachNodeInTheCycle()
    {
        var source = """
            using MinimalApiAutoRepr.Generated;
            public class GroupA : IGroupEndpoint<GroupB>
            {
                public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) => app;
            }
            public class GroupB : IGroupEndpoint<GroupA>
            {
                public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) => app;
            }
            """;

        var result = Run(source);

        // A two-node cycle (A→B→A) produces one MAAR001 per cycle node.
        Assert.Equal(2, result.Diagnostics.Count(d => d.Id == "MAAR001"));
    }

    // ── ToCamelCase edge cases ─────────────────────────────────────────────────

    [Fact]
    public void WhenGroupNameIsSingleCharacter_ThenVarNameIsLowercased()
    {
        // Exercises the s.Length == 1 branch in ToCamelCase.
        var source = """
            using MinimalApiAutoRepr.Generated;
            public class A : IGroupEndpoint
            {
                public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) => app;
            }
            """;

        var text = MappingsText(Run(source));

        Assert.Contains("var a = global::A.Map(app);", text);
    }

    // ── Variable name deduplication ────────────────────────────────────────────

    [Fact]
    public void WhenTwoGroupNamesCamelCaseToTheSameName_ThenSecondVarGetsSuffix()
    {
        // "MyGroup" and "myGroup" both camel-case to "myGroup", so the second
        // emitted group must use "myGroup2" to avoid a duplicate variable name.
        var source = """
            using MinimalApiAutoRepr.Generated;
            public class MyGroup : IGroupEndpoint
            {
                public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) => app;
            }
            public class myGroup : IGroupEndpoint
            {
                public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) => app;
            }
            """;

        var text = MappingsText(Run(source));

        Assert.Contains("myGroup2", text);
    }
}

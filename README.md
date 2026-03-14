# MinimalApiAutoRepr

A lightweight source generator for ASP.NET Core Minimal APIs that automatically discovers and maps your endpoints and route groups using a convention-based approach.

## Features

- 🚀 **Zero runtime overhead** - Source generation happens at compile time
- 📁 **Organize by feature** - Structure your endpoints in feature folders without boilerplate
- 🔗 **Automatic route group hierarchy** - Define parent-child relationships declaratively
- 🎯 **Type-safe** - Leverage static typing with generated interfaces
- 🔍 **Cycle detection** - Compile-time warnings for circular group references
- 📝 **OpenAPI-ready** - Full support for ASP.NET Core's OpenAPI metadata

## Installation

Install the NuGet package:

```bash
dotnet add package MinimalApiAutoRepr
```

## Quick Start

### 1. Reference the Generator

Add a project reference to your web project:

```xml
<ItemGroup>
  <ProjectReference Include="MinimalApiAutoRepr.csproj" 
                    OutputItemType="Analyzer" 
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

Or install the NuGet package which includes the analyzer automatically.

### 2. Add Global Using (Optional but Recommended)

In your `Program.cs` or a `GlobalUsings.cs` file:

```csharp
global using MinimalApiAutoRepr.Generated;
```

### 3. Wire Up in Program.cs

Replace manual endpoint mappings with a single call:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(); // Optional: for OpenAPI support

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 🎉 This automatically maps all your endpoints!
app.MapAutoReprEndpoints();

app.Run();
```

### 4. Create Your First Endpoint

Define an endpoint class that implements `IEndpoint` or `IEndpoint<TGroup>`:

```csharp
namespace MyApi.Features.Todo;

using Microsoft.AspNetCore.Http.HttpResults;

public class CreateTodo : IEndpoint<TodoGroup>
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/", Handle);

    public record Request(string Title);
    public record Response(int Id, string Title, bool IsCompleted);

    private static async Task<Results<CreatedAtRoute<Response>, ValidationProblem>> Handle(
        Request req,
        CancellationToken ct)
    {
        await Task.Delay(100, ct); // Your business logic here
        var resp = new Response(new Random().Next(1, 1000), req.Title, false);
        return TypedResults.CreatedAtRoute(resp, nameof(GetTodoById), new { id = resp.Id });
    }
}
```

### 5. Create a Route Group

Define a route group to organize related endpoints:

```csharp
namespace MyApi.Features.Todo;

public class TodoGroup : IGroupEndpoint
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) =>
        app.MapGroup("/api/todos")
           .WithTags("Todo Endpoints");
}
```

The generator will automatically:
- Find all `CreateTodo` endpoints that implement `IEndpoint<TodoGroup>`
- Map them under the `/api/todos` prefix
- Apply the "Todo Endpoints" tag for OpenAPI

## Core Concepts

### Endpoints

An endpoint is a class that implements `IEndpoint` or `IEndpoint<TGroup>`:

- **`IEndpoint`**: Maps directly to the root app builder (not common)
- **`IEndpoint<TGroup>`**: Maps to a specific route group

**Requirements:**
- Must have a `public static void Map(IEndpointRouteBuilder app)` method
- Should call `app.MapGet/MapPost/MapPut/MapDelete/etc.` to define the route

**Example:**

```csharp
public class GetTodoById : IEndpoint<TodoGroup>
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/{id:int}", Handle)
           .WithName(nameof(GetTodoById));

    private static async Task<Results<Ok<Response>, NotFound>> Handle(
        [AsParameters] Request req,
        CancellationToken ct)
    {
        // Your logic here
    }
}
```

### Route Groups

A route group is a class that implements `IGroupEndpoint` or `IGroupEndpoint<TParent>`:

- **`IGroupEndpoint`**: Creates a top-level group (or returns `app` as-is)
- **`IGroupEndpoint<TParent>`**: Nests under another group, creating a hierarchy

**Requirements:**
- Must have a `public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app)` method
- Should return the result of `app.MapGroup(...)` or `app` itself

**Example - Top-Level Group:**

```csharp
public class ApiGroup : IGroupEndpoint
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) =>
        app.MapGroup("/api");
}
```

**Example - Nested Group:**

```csharp
public class TodoGroup : IGroupEndpoint<ApiGroup>
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) =>
        app.MapGroup("/todos")
           .WithTags("Todo Endpoints")
           .RequireAuthorization(); // Add middleware, metadata, etc.
}
```

### Group Hierarchies

You can create deep hierarchies by chaining group relationships:

```csharp
// Top-level: /api
public class ApiGroup : IGroupEndpoint
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) =>
        app.MapGroup("/api");
}

// Second level: /api/v1
public class V1Group : IGroupEndpoint<ApiGroup>
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) =>
        app.MapGroup("/v1");
}

// Third level: /api/v1/todos
public class TodoGroup : IGroupEndpoint<V1Group>
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) =>
        app.MapGroup("/todos")
           .WithTags("Todos");
}
```

All endpoints implementing `IEndpoint<TodoGroup>` will be mapped under `/api/v1/todos`.

## Project Structure Example

A typical feature-based structure:

```
MyApi/
├── Program.cs
├── Features/
│   ├── Todo/
│   │   ├── TodoGroup.cs           (IGroupEndpoint)
│   │   ├── CreateTodo.cs          (IEndpoint<TodoGroup>)
│   │   ├── GetTodoById.cs         (IEndpoint<TodoGroup>)
│   │   ├── UpdateTodo.cs          (IEndpoint<TodoGroup>)
│   │   └── DeleteTodo.cs          (IEndpoint<TodoGroup>)
│   └── Notes/
│       ├── NotesGroup.cs          (IGroupEndpoint)
│       ├── CreateNote/
│       │   ├── Endpoint.cs        (IEndpoint<NotesGroup>)
│       │   ├── Request.cs
│       │   └── Response.cs
│       └── GetNoteById/
│           ├── Endpoint.cs        (IEndpoint<NotesGroup>)
│           ├── Request.cs
│           └── Response.cs
└── Common/
    └── Groups/
        └── ApiGroup.cs            (IGroupEndpoint)
```

## Advanced Usage

### Applying Middleware and Metadata

You can apply middleware, filters, and metadata in your `Map` methods:

```csharp
public class SecureTodoGroup : IGroupEndpoint
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) =>
        app.MapGroup("/secure-todos")
           .RequireAuthorization()
           .RequireRateLimiting("fixed")
           .WithTags("Secure Todos")
           .WithOpenApi();
}
```

### Validation

Use ASP.NET Core's built-in validation with data annotations:

```csharp
public class CreateTodo : IEndpoint<TodoGroup>
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/", Handle);

    public record Request(
        [Required, MinLength(3), MaxLength(100)] string Title,
        [Range(1, 5)] int Priority
    );

    private static async Task<Results<Created<Response>, ValidationProblem>> Handle(
        Request req,
        CancellationToken ct)
    {
        // Validation happens automatically
    }
}
```

Make sure to enable validation in `Program.cs`:

```csharp
builder.Services.AddProblemDetails();
```

### Named Routes

Use named routes for generating links:

```csharp
public class GetTodoById : IEndpoint<TodoGroup>
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/{id:int}", Handle)
           .WithName(nameof(GetTodoById)); // Name the route
}

public class CreateTodo : IEndpoint<TodoGroup>
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/", Handle);

    private static Task<CreatedAtRoute<Response>> Handle(Request req, CancellationToken ct)
    {
        var resp = new Response(123, req.Title);
        return TypedResults.CreatedAtRoute(resp, nameof(GetTodoById), new { id = 123 });
    }
}
```

## Diagnostics

The generator provides compile-time diagnostics:

### MAAR001: IGroupEndpoint Parent Cycle

If you create circular group references, you'll get a warning:

```csharp
public class GroupA : IGroupEndpoint<GroupB> { ... }
public class GroupB : IGroupEndpoint<GroupA> { ... } // ⚠️ MAAR001
```

**Fix:** Break the cycle by restructuring your group hierarchy.

## How It Works

1. **Compile-Time Discovery**: The source generator scans your assembly for classes implementing `IEndpoint` and `IGroupEndpoint`
2. **Hierarchy Resolution**: Groups are organized into a tree based on their parent relationships
3. **Code Generation**: A `MapAutoReprEndpoints()` extension method is generated that:
   - Instantiates each group in dependency order
   - Calls each endpoint's `Map()` method with the appropriate group builder
4. **Type Safety**: All interfaces are generated, ensuring compile-time safety

## Generated Code Example

For the examples above, the generator produces code like this:

```csharp
// <auto-generated />
namespace MinimalApiAutoRepr.Generated;

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

public static class GeneratedEndpointMappings
{
    public static IEndpointRouteBuilder MapAutoReprEndpoints(this IEndpointRouteBuilder app)
    {
        var apiGroup = global::MyApi.Common.Groups.ApiGroup.Map(app);
        var todoGroup = global::MyApi.Features.Todo.TodoGroup.Map(apiGroup);

        global::MyApi.Features.Todo.CreateTodo.Map(todoGroup);
        global::MyApi.Features.Todo.GetTodoById.Map(todoGroup);
        global::MyApi.Features.Todo.UpdateTodo.Map(todoGroup);
        global::MyApi.Features.Todo.DeleteTodo.Map(todoGroup);

        return app;
    }
}
```

## Compatibility

- **Target Framework**: .NET Standard 2.0 (generator), .NET 6.0+ (runtime usage)
- **ASP.NET Core**: 6.0+
- **C# Version**: 10.0+

## License

[Include your license here]

## Contributing

[Include contribution guidelines here]
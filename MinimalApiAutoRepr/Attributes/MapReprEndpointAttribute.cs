namespace MinimalApiAutoRepr.Attributes;

using System;
using System.Collections.Generic;
using System.Text;

[AttributeUsage(AttributeTargets.Class)]
/// <summary>
/// Base attribute for mapping HTTP endpoints.
/// Contains common metadata that is shared across specific HTTP method attributes.
/// </summary>
/// <param name="name">Optional logical name for the route (used for link generation).</param>
/// <param name="group">Optional type used to group endpoints together.</param>
public abstract class MapEndpointAttributeBase(string? name = null, Type? group = null) : Attribute
{
    /// <summary>
    /// Optional logical name for the route.
    /// </summary>
    public string? Name { get; } = name;

    /// <summary>
    /// Short summary for the endpoint used in generated documentation.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Longer description for the endpoint used in generated documentation.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Optional group type used to assign the endpoint to a route group.
    /// </summary>
    public Type? Group { get; } = group;
}

[AttributeUsage(AttributeTargets.Class)]
/// <summary>
/// Maps a GET endpoint to the specified route template.
/// </summary>
/// <param name="route">The route template for the GET endpoint.</param>
/// <param name="name">Optional logical name for the route.</param>
/// <param name="group">Optional group type used to group the endpoint.</param>
public sealed class MapGetAttribute(string route, string? name = null, Type? group = null) : MapEndpointAttributeBase(name, group)
{
    /// <summary>
    /// The route template for the GET endpoint.
    /// </summary>
    public string Route { get; } = route;
}

[AttributeUsage(AttributeTargets.Class)]
/// <summary>
/// Maps a POST endpoint to the specified route template.
/// </summary>
/// <param name="route">The route template for the POST endpoint.</param>
/// <param name="name">Optional logical name for the route.</param>
/// <param name="group">Optional group type used to group the endpoint.</param>
public sealed class MapPostAttribute(string route, string? name = null, Type? group = null) : MapEndpointAttributeBase(name, group)
{
    /// <summary>
    /// The route template for the POST endpoint.
    /// </summary>
    public string Route { get; } = route;
}

[AttributeUsage(AttributeTargets.Class)]
/// <summary>
/// Maps a PUT endpoint to the specified route template.
/// </summary>
/// <param name="route">The route template for the PUT endpoint.</param>
/// <param name="name">Optional logical name for the route.</param>
/// <param name="group">Optional group type used to group the endpoint.</param>
public sealed class MapPutAttribute(string route, string? name = null, Type? group = null) : MapEndpointAttributeBase(name, group)
{
    /// <summary>
    /// The route template for the PUT endpoint.
    /// </summary>
    public string Route { get; } = route;
}

[AttributeUsage(AttributeTargets.Class)]
/// <summary>
/// Maps a DELETE endpoint to the specified route template.
/// </summary>
/// <param name="route">The route template for the DELETE endpoint.</param>
/// <param name="name">Optional logical name for the route.</param>
/// <param name="group">Optional group type used to group the endpoint.</param>
public sealed class MapDeleteAttribute(string route, string? name = null, Type? group = null) : MapEndpointAttributeBase(name, group)
{
    /// <summary>
    /// The route template for the DELETE endpoint.
    /// </summary>
    public string Route { get; } = route;
}

[AttributeUsage(AttributeTargets.Class)]
/// <summary>
/// Maps a PATCH endpoint to the specified route template.
/// </summary>
/// <param name="route">The route template for the PATCH endpoint.</param>
/// <param name="name">Optional logical name for the route.</param>
/// <param name="group">Optional group type used to group the endpoint.</param>
public sealed class MapPatchAttribute(string route, string? name = null, Type? group = null) : MapEndpointAttributeBase(name, group)
{
    /// <summary>
    /// The route template for the PATCH endpoint.
    /// </summary>
    public string Route { get; } = route;
}


[AttributeUsage(AttributeTargets.Class)]
/// <summary>
/// Maps a route group using the specified prefix.
/// </summary>
/// <param name="prefix">The route prefix for the group (for example, "api/todos").</param>
/// <param name="name">Optional logical name for the group.</param>
/// <param name="group">Optional parent group type.</param>
public sealed class MapGroupAttribute(string prefix, string? name = null, Type? group = null) : MapEndpointAttributeBase(name, group)
{
    /// <summary>
    /// The route prefix for the group.
    /// </summary>
    public string Prefix { get; } = prefix;
}

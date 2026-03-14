namespace MinimalApiAutoRepr.TestApi.Features.Notes.CreateNote;

using System.ComponentModel.DataAnnotations;

public record Request(
    [property: Required, MinLength(1), MaxLength(200)] string Title,
    [property: MaxLength(2000)] string? Content);

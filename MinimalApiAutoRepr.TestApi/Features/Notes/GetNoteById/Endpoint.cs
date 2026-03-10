namespace MinimalApiAutoRepr.TestApi.Features.Notes.GetNoteById;

using Microsoft.AspNetCore.Http.HttpResults;
using MinimalApiAutoRepr.TestApi.Features.Notes;

[MapGet("/{id}", "GetNoteById", typeof(NotesGroup))]
public class Endpoint
{
    public static async Task<Results<Ok<Response>, NotFound>> Handle(
        [AsParameters] Request req,
        CancellationToken ct)
    {
        if (req.Id == 0)
        {
            return TypedResults.NotFound();
        }

        await Task.Delay(50, ct); // Simulate some async work
        return TypedResults.Ok(new Response(req.Id, $"Note {req.Id}", req.Id % 2 == 0 ? "Even note content" : null));
    }
}

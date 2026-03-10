namespace MinimalApiAutoRepr.TestApi.Features.Notes.CreateNote;

using Microsoft.AspNetCore.Http.HttpResults;
using MinimalApiAutoRepr.TestApi.Features.Notes.GetNoteById;

public class Endpoint : IEndpoint<NotesGroup>
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/", Handle);

	private static async Task<Results<CreatedAtRoute<Response>, ValidationProblem>> Handle(
        Request req,
        CancellationToken ct)
    {
        await Task.Delay(50, ct); // Simulate async work
        var resp = new Response(new Random().Next(1, 1000), req.Title, req.Content);
        return TypedResults.CreatedAtRoute(resp, "GetNoteById", new { id = resp.Id });
    }
}

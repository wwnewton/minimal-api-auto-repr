namespace MinimalApiAutoRepr.TestApi.Features.Todo;

using Microsoft.AspNetCore.Http.HttpResults;

public class DeleteTodo : IEndpoint<TodoGroup>
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapDelete("/{id:int}", Handle)
           .WithName(nameof(DeleteTodo));
    public record Request(int Id);

    private static async Task<Results<NoContent, NotFound>> Handle(
        [AsParameters] Request req,
        CancellationToken ct)
    {
        if (req.Id == 0)
        {
            return TypedResults.NotFound();
        }

        await Task.Delay(50, ct); // Simulate async work
        return TypedResults.NoContent();
    }
}

namespace MinimalApiAutoRepr.TestApi.Features.Todo;

using Microsoft.AspNetCore.Http.HttpResults;

[MapDelete("/{id}", nameof(DeleteTodo), typeof(TodoGroup))]
public class DeleteTodo
{
    public record Request(int Id);

    public static async Task<Results<NoContent, NotFound>> Handle(
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

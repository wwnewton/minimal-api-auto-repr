namespace MinimalApiAutoRepr.TestApi.Features.Todo;

using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

public class UpdateTodo : IEndpoint<TodoGroup>
{
	public static void Map(IEndpointRouteBuilder app) => 
        app.MapPut("/{id}", Handle)
           .WithName(nameof(UpdateTodo))
           .WithSummary("Update a Todo by Id")
           .WithDescription("Updates an existing Todo item identified by the route {id}. Accepts a request body containing a required Title (3-100 characters) and IsCompleted flag. Returns 200 OK with the updated Todo on success, 404 Not Found if the item does not exist, or 400 ValidationProblem when input validation fails. This operation is performed asynchronously.");
	public record Request([property: Required] int Id,
                          [property: Required, MinLength(3), MaxLength(100)] string Title,
                          bool IsCompleted);

    private static async Task<Results<Ok<GetTodoById.Response>, NotFound, ValidationProblem>> Handle(
        [AsParameters] Request req,
        CancellationToken ct)
    {
        if (req.Id == 0)
        {
            return TypedResults.NotFound();
        }

        await Task.Delay(50, ct); // Simulate async work
        var resp = new GetTodoById.Response(req.Id, req.Title, req.IsCompleted);
        return TypedResults.Ok(resp);
    }
}

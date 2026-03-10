namespace MinimalApiAutoRepr.TestApi.Features.Todo;

using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

public class CreateTodo : IEndpoint<TodoGroup>
{
	public static void Map(IEndpointRouteBuilder app) => 
		app.MapPost("/", Handle);

	public record Request([property: Required, MinLength(3), MaxLength(100)]string Title);
	public record Response(int Id, string Title, bool IsCompleted);

	private static async Task<Results<CreatedAtRoute<Response>, ValidationProblem>> Handle(
		Request req,
		CancellationToken ct)
	{
		await Task.Delay(100, ct); // Simulate some async work
		var resp = new Response(new Random().Next(1, 1000), req.Title, false);
		return TypedResults.CreatedAtRoute(resp, nameof(GetTodoById), new { id = resp.Id });
	}

	
}

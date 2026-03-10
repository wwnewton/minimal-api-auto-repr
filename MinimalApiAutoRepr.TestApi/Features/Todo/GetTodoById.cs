namespace MinimalApiAutoRepr.TestApi.Features.Todo;

using Microsoft.AspNetCore.Http.HttpResults;

public class GetTodoById : IEndpoint<TodoGroup>
{

	public static void Map(IEndpointRouteBuilder app) =>
		app.MapGet("/{id:int}", Handle)
		   .WithName(nameof(GetTodoById));


	public record Request(int Id);
	public record Response(int Id, string Title, bool IsCompleted);
	private static async Task<Results<Ok<Response>, NotFound>> Handle(
		[AsParameters] Request req,
		CancellationToken ct)
	{
		if(req.Id == 0)
		{
			return TypedResults.NotFound();
		}

		await Task.Delay(100, ct); // Simulate some async work
		return TypedResults.Ok(new Response(req.Id, $"Todo Item {req.Id}", req.Id % 2 == 0));
	}

	
}

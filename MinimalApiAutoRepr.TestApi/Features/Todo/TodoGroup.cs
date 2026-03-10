namespace MinimalApiAutoRepr.TestApi.Features.Todo;

using MinimalApiAutoRepr.TestApi.Common.Groups;

[MapGroup("api/todos",  name: "Todos", group: typeof(TopLevelGroup))]
public class TodoGroup
{
	public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app)
	{
		return app;
	}
}

namespace MinimalApiAutoRepr.TestApi.Features.Todo;

using MinimalApiAutoRepr.TestApi.Common.Groups;

public class TodoGroup : IGroupEndpoint<TopLevelGroup>
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) =>
        app.MapGroup("/todos")
           .WithTags("Todo Endpoints");

}

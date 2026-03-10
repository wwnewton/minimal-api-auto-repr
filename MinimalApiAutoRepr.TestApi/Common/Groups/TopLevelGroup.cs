namespace MinimalApiAutoRepr.TestApi.Common.Groups;

[MapGroup("")]
public class TopLevelGroup
{
	public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app)
	{
		return app;
	}
}

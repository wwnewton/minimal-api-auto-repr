namespace MinimalApiAutoRepr.TestApi.Common.Groups;


public class TopLevelGroup : IGroupEndpoint
{
	public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app)
	{
		return app;
	}
}

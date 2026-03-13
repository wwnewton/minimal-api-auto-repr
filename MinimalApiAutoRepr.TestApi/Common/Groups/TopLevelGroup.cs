namespace MinimalApiAutoRepr.TestApi.Common.Groups;


public class TopLevelGroup : IGroupEndpoint
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app)
    {
        return app;
    }
}

public class NextLevelGroup : IGroupEndpoint<FourthLevelGroup>
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) =>
        app.MapGroup("/next-level")
           .WithTags("Next Level Endpoints");
}

public class ThirdLevelGroup : IGroupEndpoint<NextLevelGroup>
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) =>
        app.MapGroup("/third-level")
           .WithTags("Third Level Endpoints");
}

public class FourthLevelGroup : IGroupEndpoint<ThirdLevelGroup>
{
    public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) =>
        app.MapGroup("/fourth-level")
           .WithTags("Fourth Level Endpoints");
}
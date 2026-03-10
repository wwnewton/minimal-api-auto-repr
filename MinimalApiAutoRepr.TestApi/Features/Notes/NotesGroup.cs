namespace MinimalApiAutoRepr.TestApi.Features.Notes;

public class NotesGroup : IGroupEndpoint
{
	public static IEndpointRouteBuilder Map(IEndpointRouteBuilder app) => 
		app.MapGroup("/api/notes")
		   .WithTags("Notes Endpoints");
}

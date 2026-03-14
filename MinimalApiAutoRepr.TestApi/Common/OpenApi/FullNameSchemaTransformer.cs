namespace MinimalApiAutoRepr.TestApi.Common.OpenApi;

using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

public class FullNameSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        if (context.JsonTypeInfo.Type.IsValueType ||
                    context.JsonTypeInfo.Type == typeof(string))
        {
            return Task.CompletedTask;
        }

        var fullName = context.JsonTypeInfo.Type.FullName;
        schema.Metadata?["x-schema-id"] = fullName!;
        return Task.CompletedTask;
    }
}

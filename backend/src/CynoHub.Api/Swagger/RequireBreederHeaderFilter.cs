using CynoHub.Api.Attributes;
using CynoHub.Api.Services;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CynoHub.Api.Swagger;

public class RequireBreederHeaderFilter : IOperationFilter
{
    private readonly OpenApiParameter _breederIdParameter = new()
    {
        Name = BreederService.BreederIdHeader,
        In = ParameterLocation.Header,
        Required = true,
        Schema = new OpenApiSchema { Type = "string", Format = "uuid" }
    };

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!HasAttribute<RequireBreederAttribute>(context))
            return;

        operation.Parameters ??= [];
        operation.Parameters.Add(_breederIdParameter);
    }

    private bool HasAttribute<T>(OperationFilterContext context) where T : Attribute =>
        context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<T>().Any() ?? false
        || context.MethodInfo.GetCustomAttributes(true).OfType<T>().Any();
}

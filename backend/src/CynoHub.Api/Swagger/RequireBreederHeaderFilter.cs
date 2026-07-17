using CynoHub.Api.Attributes;
using CynoHub.Api.Services;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CynoHub.Api.Swagger;

public class RequireBreederHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasRequireBreederAttribute =
            context
                .MethodInfo.DeclaringType?.GetCustomAttributes(true)
                .OfType<RequireBreederAttribute>()
                .Any() == true
            || context.MethodInfo.GetCustomAttributes(true).OfType<RequireBreederAttribute>().Any();

        if (!hasRequireBreederAttribute)
        {
            return;
        }

        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = BreederService.BreederIdHeader,
                In = ParameterLocation.Header,
                Description = "ID of the breeder making the request",
                Required = true,
                Schema = new OpenApiSchema { Type = "string", Format = "uuid" },
            }
        );
    }
}

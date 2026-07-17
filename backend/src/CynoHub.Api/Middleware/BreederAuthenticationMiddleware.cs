using CynoHub.Api.Attributes;
using CynoHub.Application.Interfaces.Services;
using CynoHub.Domain.Exceptions;
using Microsoft.AspNetCore.Http.Features;

namespace CynoHub.Api.Middleware;

public class BreederAuthenticationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IBreederService breederService)
    {
        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        var attribute = endpoint?.Metadata.GetMetadata<RequireBreederAttribute>();

        if (attribute is not null && !breederService.IsAuthenticated)
        {
            throw new UnauthorizedException(
                "Breeder authentication required. Missing or invalid X-Breeder-Id header."
            );
        }

        await next(context);
    }
}

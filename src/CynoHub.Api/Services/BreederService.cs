using CynoHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System;

namespace CynoHub.Api.Services;

public class BreederService(IHttpContextAccessor httpContextAccessor) : IBreederService
{
    public const string BreederIdHeader = "X-Breeder-Id";

    public Guid? CurrentBreederId
    {
        get
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext is null)
            {
                return null;
            }

            if (!httpContext.Request.Headers.TryGetValue(BreederIdHeader, out var headerValue))
            {
                return null;
            }

            return Guid.TryParse(headerValue, out var breederId)
                ? breederId
                : null;
        }
    }

    public bool IsAuthenticated => CurrentBreederId.HasValue;
}

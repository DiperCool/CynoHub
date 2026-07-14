using System.Text.Json;
using CynoHub.Domain.Exceptions;

namespace CynoHub.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger
)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            ForbiddenException => (StatusCodes.Status403Forbidden, "Forbidden"),
            ConflictException => (StatusCodes.Status409Conflict, "Concurrency Conflict"),
            DomainException => (
                StatusCodes.Status422UnprocessableEntity,
                "Business Rule Violation"
            ),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error"),
        };

        var response = new
        {
            type = $"https://httpstatuses.io/{statusCode}",
            title,
            status = statusCode,
            detail = exception.Message,
            traceId = context.TraceIdentifier,
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(
                response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            )
        );
    }
}

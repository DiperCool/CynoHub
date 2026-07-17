namespace CynoHub.Domain.Exceptions;

/// <summary>
/// Thrown when a requested resource does not exist.
/// Maps to HTTP 404 Not Found.
/// </summary>
public class NotFoundException(string entityName, object key)
    : Exception($"{entityName} with key '{key}' was not found.");

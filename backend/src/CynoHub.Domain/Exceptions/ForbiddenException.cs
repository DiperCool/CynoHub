namespace CynoHub.Domain.Exceptions;

/// <summary>
/// Thrown when the current actor is not allowed to perform the requested operation.
/// Maps to HTTP 403 Forbidden.
/// </summary>
public class ForbiddenException(string message) : Exception(message);

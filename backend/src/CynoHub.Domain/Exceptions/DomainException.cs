namespace CynoHub.Domain.Exceptions;

/// <summary>
/// Thrown when a business rule or invariant is violated.
/// Maps to HTTP 422 Unprocessable Entity.
/// </summary>
public class DomainException(string message) : Exception(message);

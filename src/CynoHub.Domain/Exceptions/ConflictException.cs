namespace CynoHub.Domain.Exceptions;

public class ConflictException(string message = "A concurrency conflict occurred. Please retry the operation.") : Exception(message);

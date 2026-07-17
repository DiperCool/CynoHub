namespace CynoHub.Domain.Interfaces.Repositories;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    /// <summary>
    /// Executes the given action atomically within a database transaction.
    /// Commits on success, rolls back on failure.
    /// NOTE: In-Memory EF provider ignores transactions at the DB level,
    ///       but using this abstraction keeps the code production-ready.
    /// </summary>
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default);
}

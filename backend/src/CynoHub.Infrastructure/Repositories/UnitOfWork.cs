using CynoHub.Domain.Exceptions;
using CynoHub.Domain.Interfaces.Repositories;
using CynoHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CynoHub.Infrastructure.Repositories;

public sealed class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            return await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            db.ChangeTracker.Clear();
            throw new ConflictException();
        }
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default)
    {
        // IDbContextTransaction is a no-op for In-Memory, but the pattern is correct
        // for any relational provider (SQLite, SQL Server, etc.)
        await using IDbContextTransaction tx = await db.Database.BeginTransactionAsync(ct);
        try
        {
            await action();
            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            await tx.RollbackAsync(ct);
            db.ChangeTracker.Clear();
            throw new ConflictException();
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace CynoHub.Infrastructure.Extensions;

/// <summary>
/// EF Core extensions for paginating <see cref="IQueryable{T}"/> without repeating
/// CountAsync + Skip + Take boilerplate across every repository.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Executes a COUNT query then fetches one page of results.
    /// Ordering must be applied by the caller <b>before</b> invoking this method.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    /// <param name="query">Source query (already filtered and ordered).</param>
    /// <param name="pageNumber">1-based page number.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A value tuple with the current-page items and the overall total count.</returns>
    public static async Task<(IReadOnlyList<T> Items, int TotalCount)> ToPagedAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default
    )
    {
        var totalCount = await query.CountAsync(ct);

        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return (items.AsReadOnly(), totalCount);
    }
}

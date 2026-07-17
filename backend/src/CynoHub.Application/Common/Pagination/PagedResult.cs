namespace CynoHub.Application.Common.Pagination;

/// <summary>
/// Generic paginated response envelope.
/// JSON shape:
/// <code>
/// {
///   "data": [ ... ],
///   "pagination": {
///     "totalCount": 42,
///     "pageNumber": 2,
///     "pageSize": 10,
///     "totalPages": 5,
///     "hasNextPage": true,
///     "hasPreviousPage": true
///   }
/// }
/// </code>
/// </summary>
public record PagedResult<T>(IReadOnlyList<T> Data, PaginationMeta Pagination)
{
    /// <summary>Convenience factory — builds the pagination meta automatically.</summary>
    public static PagedResult<T> Create(
        IReadOnlyList<T> data,
        int totalCount,
        int pageNumber,
        int pageSize
    ) => new(data, PaginationMeta.From(totalCount, pageNumber, pageSize));
}

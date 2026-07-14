namespace CynoHub.Application.Common.Pagination;

/// <summary>
/// Metadata block returned alongside paginated data.
/// Serializes as a clean <c>pagination</c> JSON object.
/// </summary>
public record PaginationMeta(
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage
)
{
    public static PaginationMeta From(int totalCount, int pageNumber, int pageSize)
    {
        var totalPages = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;

        return new PaginationMeta(
            TotalCount: totalCount,
            PageNumber: pageNumber,
            PageSize: pageSize,
            TotalPages: totalPages,
            HasNextPage: pageNumber < totalPages,
            HasPreviousPage: pageNumber > 1
        );
    }
}

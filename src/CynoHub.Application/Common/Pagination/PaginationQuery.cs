using System.ComponentModel.DataAnnotations;

namespace CynoHub.Application.Common.Pagination;

/// <summary>
/// Encapsulates pagination parameters with self-normalization.
/// Bind directly from query string: ?pageNumber=2&amp;pageSize=20
/// </summary>
public record PaginationQuery
{
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be ≥ 1.")]
    public int PageNumber { get; init; } = 1;

    [Range(1, MaxPageSize, ErrorMessage = "PageSize must be between 1 and 100.")]
    public int PageSize { get; init; } = DefaultPageSize;

    /// <summary>
    /// Returns a copy with values clamped to valid bounds.
    /// Keeps the service layer free of manual Math.Max/Clamp calls.
    /// </summary>
    public PaginationQuery Normalize() =>
        this with
        {
            PageNumber = Math.Max(1, PageNumber),
            PageSize = Math.Clamp(PageSize, 1, MaxPageSize),
        };
}

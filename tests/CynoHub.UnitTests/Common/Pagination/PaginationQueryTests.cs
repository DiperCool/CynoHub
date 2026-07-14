using CynoHub.Application.Common.Pagination;

namespace CynoHub.UnitTests.Common.Pagination;

public sealed class PaginationQueryTests
{
    [Theory]
    [InlineData(0, 10, 1, 10)]
    [InlineData(-99, 10, 1, 10)]
    [InlineData(1, 0, 1, 1)]
    [InlineData(1, 999, 1, 100)]
    [InlineData(3, 20, 3, 20)]
    public void PaginationQuery_Normalize_ClampsValues(
        int rawPage,
        int rawSize,
        int expectedPage,
        int expectedSize
    )
    {
        var q = new PaginationQuery { PageNumber = rawPage, PageSize = rawSize }.Normalize();

        Assert.Equal(expectedPage, q.PageNumber);
        Assert.Equal(expectedSize, q.PageSize);
    }
}

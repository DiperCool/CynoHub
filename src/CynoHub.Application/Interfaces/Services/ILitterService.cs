using CynoHub.Application.Common.Pagination;
using CynoHub.Application.DTOs;
using CynoHub.Domain.Enums;

namespace CynoHub.Application.Interfaces.Services;

public interface ILitterService
{
    Task PublishAsync(Guid litterId, CancellationToken ct = default);

    Task<PagedResult<LitterDto>> GetPagedAsync(
        LitterStatus? status,
        PaginationQuery pagination,
        CancellationToken ct = default
    );
}

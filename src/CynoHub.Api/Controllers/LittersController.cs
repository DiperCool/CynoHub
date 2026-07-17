using CynoHub.Application.Common.Pagination;
using CynoHub.Application.Interfaces.Services;
using CynoHub.Application.DTOs;
using CynoHub.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using CynoHub.Api.Attributes;

namespace CynoHub.Api.Controllers;

[ApiController]
[Route("api/litters")]
[RequireBreeder]
public sealed class LittersController(ILitterService litterService) : ControllerBase
{
    // POST /api/litters/{litterId}/publish
    [HttpPost("{litterId:guid}/publish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Publish(
        Guid litterId,
        CancellationToken ct
    )
    {
        await litterService.PublishAsync(litterId, ct);
        return Ok();
    }

    // GET /api/litters?status=Approved&pageNumber=1&pageSize=10
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<LitterDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<LitterDto>>> GetList(
        [FromQuery] LitterStatus? status,
        [FromQuery] PaginationQuery pagination,
        CancellationToken ct = default
    )
    {
        var result = await litterService.GetPagedAsync(status, pagination, ct);
        return Ok(result);
    }
}

using CynoHub.Application.Common.Pagination;
using CynoHub.Application.Interfaces.Services;
using CynoHub.Application.DTOs;
using CynoHub.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CynoHub.Api.Controllers;

[ApiController]
[Route("api/litters")]
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
        [FromHeader(Name = "X-Breeder-Id")] Guid breederId,
        CancellationToken ct
    )
    {
        await litterService.PublishAsync(litterId, breederId, ct);
        return Ok();
    }

    // GET /api/litters?status=Approved&pageNumber=1&pageSize=10
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<LitterDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<LitterDto>>> GetList(
        [FromHeader(Name = "X-Breeder-Id")] Guid breederId,
        [FromQuery] LitterStatus? status,
        [FromQuery] PaginationQuery pagination,
        CancellationToken ct = default
    )
    {
        var result = await litterService.GetPagedAsync(breederId, status, pagination, ct);
        return Ok(result);
    }
}

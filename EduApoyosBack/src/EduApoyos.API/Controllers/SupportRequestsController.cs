using EduApoyos.Application.DTOs.SupportRequests;
using EduApoyos.Application.Features.SupportRequests.Commands;
using EduApoyos.Application.Features.SupportRequests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduApoyos.API.Controllers
{
    [ApiController]
    [Route("api/support-requests")]
    [Authorize]
    public class SupportRequestsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SupportRequestsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        [Authorize(Roles = "Advisor")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? status,
            [FromQuery] string? type,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
            => Ok(await _mediator.Send(new GetSupportRequestsQuery(status, type, page, pageSize), ct));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupportRequestRequest request, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = User.FindFirstValue(ClaimTypes.Role)!;
            var result = await _mediator.Send(new CreateSupportRequestCommand(request, userId, role), ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = User.FindFirstValue(ClaimTypes.Role)!;
            return Ok(await _mediator.Send(new GetSupportRequestByIdQuery(id, userId, role), ct));
        }

        [HttpPatch("{id:guid}/status")]
        [Authorize(Roles = "Advisor")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request, CancellationToken ct)
        {
            var advisorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(await _mediator.Send(new UpdateStatusCommand(id, request, advisorId), ct));
        }
    }
}

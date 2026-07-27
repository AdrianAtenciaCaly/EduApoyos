using EduApoyos.Application.DTOs.Students;
using EduApoyos.Application.Features.Students.Commands;
using EduApoyos.Application.Features.Students.Queries;
using EduApoyos.Application.Features.SupportRequests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace EduApoyos.API.Controllers
{
    [ApiController]
    [Route("api/students")]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public StudentsController(IMediator mediator) => _mediator = mediator;

        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMe(CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(await _mediator.Send(new GetCurrentStudentQuery(userId), ct));
        }

        [HttpGet]
        [Authorize(Roles = "Advisor")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
            => Ok(await _mediator.Send(new GetStudentsQuery(page, pageSize), ct));

        [HttpPost]
        [Authorize(Roles = "Advisor")]
        public async Task<IActionResult> Create([FromBody] CreateStudentRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(new CreateStudentCommand(request), ct);
            return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}/support-requests")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetSupportRequests(Guid id, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(await _mediator.Send(new GetSupportRequestsByStudentQuery(id, userId), ct));
        }

  
    }
}

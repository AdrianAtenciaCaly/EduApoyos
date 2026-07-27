using EduApoyos.Application.DTOs.SupportRequests;
using MediatR;

namespace EduApoyos.Application.Features.SupportRequests.Commands
{
    public record UpdateStatusCommand(Guid SupportRequestId,UpdateStatusRequest Request, Guid AdvisorId) : IRequest<SupportRequestDto>;
}

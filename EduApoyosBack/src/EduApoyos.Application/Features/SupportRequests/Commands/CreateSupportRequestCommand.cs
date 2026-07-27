using EduApoyos.Application.DTOs.SupportRequests;
using MediatR;

namespace EduApoyos.Application.Features.SupportRequests.Commands
{
    public record CreateSupportRequestCommand(
      CreateSupportRequestRequest Request,
      Guid CurrentUserId,
      string CurrentUserRole) : IRequest<SupportRequestDto>;
}

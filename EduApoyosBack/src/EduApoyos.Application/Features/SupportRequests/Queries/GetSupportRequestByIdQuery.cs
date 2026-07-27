using EduApoyos.Application.DTOs.SupportRequests;
using MediatR;

namespace EduApoyos.Application.Features.SupportRequests.Queries
{
    public record GetSupportRequestByIdQuery(
        Guid Id,
        Guid CurrentUserId,
        string CurrentUserRole) : IRequest<SupportRequestDto>;
}

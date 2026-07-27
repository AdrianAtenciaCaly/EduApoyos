using EduApoyos.Application.DTOs.SupportRequests;
using MediatR;

namespace EduApoyos.Application.Features.SupportRequests.Queries
{
    public record GetSupportRequestsByStudentQuery( Guid StudentId,Guid CurrentUserId) : IRequest<List<SupportRequestDto>>;
}

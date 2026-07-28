using EduApoyos.Application.Common;
using EduApoyos.Application.DTOs.SupportRequests;
using MediatR;

namespace EduApoyos.Application.Features.SupportRequests.Queries
{
    public record GetSupportRequestsQuery(
       string? Status = null,
       string? Type = null,
       int Page = 1,
       int PageSize = 10) : IRequest<PagedResult<SupportRequestDto>>;
}

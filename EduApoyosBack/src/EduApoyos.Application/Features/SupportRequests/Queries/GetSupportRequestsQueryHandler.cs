using EduApoyos.Application.Common;
using EduApoyos.Application.DTOs.SupportRequests;
using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Interfaces;
using MediatR;

namespace EduApoyos.Application.Features.SupportRequests.Queries
{
    public class GetSupportRequestsQueryHandler : IRequestHandler<GetSupportRequestsQuery, PagedResult<SupportRequestDto>>
    {
        private readonly ISupportRequestRepository _repository;

        public GetSupportRequestsQueryHandler(ISupportRequestRepository repository) => _repository = repository;

        public async Task<PagedResult<SupportRequestDto>> Handle(GetSupportRequestsQuery request, CancellationToken cancellationToken)
        {
            RequestStatus? status = null;
            SupportType? type = null;

            if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<RequestStatus>(request.Status, true, out var s))
                status = s;
            if (!string.IsNullOrEmpty(request.Type) && Enum.TryParse<SupportType>(request.Type, true, out var t))
                type = t;

            var (items, total) = await _repository.GetPagedAsync(status, type, request.Page, request.PageSize, cancellationToken);

            return new PagedResult<SupportRequestDto>
            {
                Items = items.Select(Map).ToList(),
                TotalCount = total,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        private static SupportRequestDto Map(SupportRequest s) => new(
            s.Id, s.StudentId, s.Student.User.FullName, s.Type.ToString(), s.RequestedAmount,
            s.Description, s.Status.ToString(), s.CreatedAt, s.UpdatedAt, s.AdvisorId,
            s.StatusHistories.Select(h => new StatusHistoryDto(
                h.PreviousStatus.ToString(), h.NewStatus.ToString(), h.ChangedAt, h.Observation)).ToList());
    }
}

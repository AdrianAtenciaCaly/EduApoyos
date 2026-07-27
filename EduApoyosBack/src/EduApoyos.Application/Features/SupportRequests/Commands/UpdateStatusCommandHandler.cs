using EduApoyos.Application.DTOs.SupportRequests;
using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Interfaces;
using MediatR;

namespace EduApoyos.Application.Features.SupportRequests.Commands
{
    public class UpdateStatusCommandHandler : IRequestHandler<UpdateStatusCommand, SupportRequestDto>
    {
        private readonly ISupportRequestRepository _repository;

        public UpdateStatusCommandHandler(ISupportRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<SupportRequestDto> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
        {
            if (!Enum.TryParse<RequestStatus>(request.Request.NewStatus, true, out var newStatus))
                throw new ArgumentException("Invalid status. Use: Pending, UnderReview, Approved, Rejected.");

            var entity = await _repository.ChangeStatusAsync(
                request.SupportRequestId,
                newStatus,
                request.AdvisorId,
                request.Request.Observation,
                cancellationToken);

            return new SupportRequestDto(
                entity.Id,
                entity.StudentId,
                entity.Student.User.FullName,
                entity.Type.ToString(),
                entity.RequestedAmount,
                entity.Description,
                entity.Status.ToString(),
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.AdvisorId,
                entity.StatusHistories
                    .OrderBy(h => h.ChangedAt)
                    .Select(h => new StatusHistoryDto(
                        h.PreviousStatus.ToString(),
                        h.NewStatus.ToString(),
                        h.ChangedAt,
                        h.Observation))
                    .ToList());
        }
    }
}

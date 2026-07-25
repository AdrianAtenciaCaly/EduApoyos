using EduApoyos.Application.DTOs.SupportRequests;
using EduApoyos.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Application.Features.SupportRequests.Queries
{
    public class GetSupportRequestByIdQueryHandler : IRequestHandler<GetSupportRequestByIdQuery, SupportRequestDto>
    {
        private readonly ISupportRequestRepository _repository;
        private readonly IStudentRepository _studentRepository;

        public GetSupportRequestByIdQueryHandler(
            ISupportRequestRepository repository,
            IStudentRepository studentRepository)
        {
            _repository = repository;
            _studentRepository = studentRepository;
        }

        public async Task<SupportRequestDto> Handle(GetSupportRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdWithDetailsAsync(request.Id, cancellationToken)
                ?? throw new KeyNotFoundException("Support request not found.");

            if (request.CurrentUserRole.Equals("Student", StringComparison.OrdinalIgnoreCase))
            {
                var student = await _studentRepository.GetByUserIdAsync(request.CurrentUserId, cancellationToken);
                if (student is null || student.Id != entity.StudentId)
                    throw new UnauthorizedAccessException("You can only view your own support requests.");
            }

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
                entity.StatusHistories.Select(h => new StatusHistoryDto(
                    h.PreviousStatus.ToString(),
                    h.NewStatus.ToString(),
                    h.ChangedAt,
                    h.Observation)).ToList());
        }
    }
}

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
    public class GetSupportRequestsByStudentQueryHandler
        : IRequestHandler<GetSupportRequestsByStudentQuery, List<SupportRequestDto>>
    {
        private readonly ISupportRequestRepository _requestRepository;
        private readonly IStudentRepository _studentRepository;

        public GetSupportRequestsByStudentQueryHandler(
            ISupportRequestRepository requestRepository,
            IStudentRepository studentRepository)
        {
            _requestRepository = requestRepository;
            _studentRepository = studentRepository;
        }

        public async Task<List<SupportRequestDto>> Handle(
            GetSupportRequestsByStudentQuery request,
            CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdWithUserAsync(request.StudentId, cancellationToken)
                ?? throw new KeyNotFoundException("Student not found.");

            if (student.UserId != request.CurrentUserId)
                throw new UnauthorizedAccessException("You can only view your own support requests.");

            var items = await _requestRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);

            return items.Select(s => new SupportRequestDto(
                s.Id, s.StudentId, s.Student.User.FullName, s.Type.ToString(), s.RequestedAmount,
                s.Description, s.Status.ToString(), s.CreatedAt, s.UpdatedAt, s.AdvisorId,
                s.StatusHistories.Select(h => new StatusHistoryDto(
                    h.PreviousStatus.ToString(), h.NewStatus.ToString(), h.ChangedAt, h.Observation)).ToList()
            )).ToList();
        }
    }
}

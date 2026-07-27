using EduApoyos.Application.DTOs.SupportRequests;
using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Interfaces;
using MediatR;

namespace EduApoyos.Application.Features.SupportRequests.Commands
{
    public class CreateSupportRequestCommandHandler : IRequestHandler<CreateSupportRequestCommand, SupportRequestDto>
    {
        private readonly ISupportRequestRepository _requestRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateSupportRequestCommandHandler( ISupportRequestRepository requestRepository,IStudentRepository studentRepository,IUnitOfWork unitOfWork)
        {
            _requestRepository = requestRepository;
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<SupportRequestDto> Handle(CreateSupportRequestCommand request, CancellationToken cancellationToken)
        {
            Guid studentId = request.Request.StudentId;

            if (request.CurrentUserRole.Equals("Student", StringComparison.OrdinalIgnoreCase))
            {
                var mine = await _studentRepository.GetByUserIdAsync(request.CurrentUserId, cancellationToken)
                    ?? throw new UnauthorizedAccessException("No student profile found.");
                studentId = mine.Id;
            }

            var student = await _studentRepository.GetByIdWithUserAsync(studentId, cancellationToken)
                ?? throw new KeyNotFoundException("Student not found.");

            if (!Enum.TryParse<SupportType>(request.Request.Type, true, out var type))
                throw new ArgumentException("Invalid support type. Use: Scholarship, Credit, Subsidy.");

            var entity = new SupportRequest(
                request.Request.StudentId,
                type,
                request.Request.RequestedAmount,
                request.Request.Description);

            await _requestRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SupportRequestDto(
                entity.Id,
                entity.StudentId,
                student.User.FullName,
                entity.Type.ToString(),
                entity.RequestedAmount,
                entity.Description,
                entity.Status.ToString(),
                entity.CreatedAt,
                entity.UpdatedAt,
                entity.AdvisorId,
                new List<StatusHistoryDto>());
        }
    }
}

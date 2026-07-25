using EduApoyos.Application.DTOs.SupportRequests;
using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Application.Features.SupportRequests.Commands
{
    public class CreateSupportRequestCommandHandler : IRequestHandler<CreateSupportRequestCommand, SupportRequestDto>
    {
        private readonly ISupportRequestRepository _requestRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateSupportRequestCommandHandler(
            ISupportRequestRepository requestRepository,
            IStudentRepository studentRepository,
            IUnitOfWork unitOfWork)
        {
            _requestRepository = requestRepository;
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<SupportRequestDto> Handle(CreateSupportRequestCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdWithUserAsync(request.Request.StudentId, cancellationToken)
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

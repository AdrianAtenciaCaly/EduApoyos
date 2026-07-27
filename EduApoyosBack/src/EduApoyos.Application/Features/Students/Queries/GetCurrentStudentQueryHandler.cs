using EduApoyos.Application.DTOs.Students;
using EduApoyos.Domain.Interfaces;
using MediatR;

namespace EduApoyos.Application.Features.Students.Queries
{
    public class GetCurrentStudentQueryHandler : IRequestHandler<GetCurrentStudentQuery, StudentDto>
    {
        private readonly IStudentRepository _repository;

        public GetCurrentStudentQueryHandler(IStudentRepository repository) => _repository = repository;

        public async Task<StudentDto> Handle(GetCurrentStudentQuery request, CancellationToken cancellationToken)
        {
            var student = await _repository.GetByUserIdAsync(request.CurrentUserId, cancellationToken)
                ?? throw new UnauthorizedAccessException("No student profile is linked to this user.");

            return new StudentDto(
                student.Id,
                student.UserId,
                student.User.FullName,
                student.User.Email,
                student.DocumentNumber,
                student.DocumentType,
                student.AcademicProgram,
                student.Semester);
        }
    }
}

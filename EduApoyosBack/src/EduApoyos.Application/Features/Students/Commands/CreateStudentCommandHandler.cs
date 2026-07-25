using EduApoyos.Application.DTOs.Students;
using EduApoyos.Application.Interfaces;
using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Application.Features.Students.Commands
{
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, StudentDto>
    {
        private readonly IIdentityService _identityService;
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateStudentCommandHandler(
            IIdentityService identityService,
            IStudentRepository studentRepository,
            IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<StudentDto> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            var (succeeded, userId, error) = await _identityService.CreateUserAsync(
                request.Request.Email,
                request.Request.Password,
                request.Request.FullName,
                UserRole.Student,
                cancellationToken);

            if (!succeeded)
                throw new InvalidOperationException(error ?? "Could not create student user.");

            var student = new Student(
                userId,
                request.Request.DocumentNumber,
                request.Request.DocumentType,
                request.Request.AcademicProgram,
                request.Request.Semester);

            await _studentRepository.AddAsync(student, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new StudentDto(
                student.Id,
                userId,
                request.Request.FullName,
                request.Request.Email,
                student.DocumentNumber,
                student.DocumentType,
                student.AcademicProgram,
                student.Semester);
        }
    }
}

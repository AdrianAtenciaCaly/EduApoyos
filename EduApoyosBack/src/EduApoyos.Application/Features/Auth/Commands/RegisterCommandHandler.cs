using EduApoyos.Application.DTOs.Auth;
using EduApoyos.Application.Interfaces;
using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Application.Features.Auth.Commands
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IStudentRepository _studentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(
            IIdentityService identityService,
            IJwtTokenService jwtTokenService,
            IStudentRepository studentRepository,
            IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _jwtTokenService = jwtTokenService;
            _studentRepository = studentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var role = request.Request.Role.Equals("Advisor", StringComparison.OrdinalIgnoreCase)
                ? UserRole.Advisor
                : UserRole.Student;

            var (succeeded, userId, error) = await _identityService.CreateUserAsync(
                request.Request.Email,
                request.Request.Password,
                request.Request.FullName,
                role,
                cancellationToken);

            if (!succeeded)
                throw new InvalidOperationException(error ?? "Registration failed.");

            if (role == UserRole.Student)
            {
                var student = new Student(
                    userId,
                    request.Request.DocumentNumber ?? "0000000000",
                    request.Request.DocumentType ?? "CC",
                    request.Request.AcademicProgram ?? "Not specified",
                    request.Request.Semester ?? 1);

                await _studentRepository.AddAsync(student, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            var user = await _identityService.GetUserByIdAsync(userId, cancellationToken)
                ?? throw new InvalidOperationException("User was created but could not be retrieved.");

            var token = _jwtTokenService.GenerateToken(user);
            return new AuthResponse(
                token,
                _jwtTokenService.GetExpiration(),
                user.Id,
                user.FullName,
                user.Email,
                user.Role.ToString());
        }
    }
}

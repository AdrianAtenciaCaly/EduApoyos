using EduApoyos.Application.DTOs.Auth;
using EduApoyos.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Application.Features.Auth.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginCommandHandler(IIdentityService identityService, IJwtTokenService jwtTokenService)
        {
            _identityService = identityService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var (succeeded, user) = await _identityService.ValidateCredentialsAsync(
                request.Request.Email, request.Request.Password, cancellationToken);

            if (!succeeded || user is null)
                throw new UnauthorizedAccessException("Invalid credentials.");

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

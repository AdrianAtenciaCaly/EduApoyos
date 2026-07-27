using EduApoyos.Application.DTOs.Auth;
using MediatR;

namespace EduApoyos.Application.Features.Auth.Commands
{
    public record LoginCommand(LoginRequest Request) : IRequest<AuthResponse>;
}

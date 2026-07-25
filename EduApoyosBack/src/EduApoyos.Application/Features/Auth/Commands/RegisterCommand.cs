using EduApoyos.Application.DTOs.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Application.Features.Auth.Commands
{
    public record RegisterCommand(RegisterRequest Request) : IRequest<AuthResponse>;
}

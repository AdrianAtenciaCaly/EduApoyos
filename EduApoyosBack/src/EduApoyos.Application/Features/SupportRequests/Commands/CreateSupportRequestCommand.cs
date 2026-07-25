using EduApoyos.Application.DTOs.SupportRequests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Application.Features.SupportRequests.Commands
{
    public record CreateSupportRequestCommand(
      CreateSupportRequestRequest Request,
      Guid CurrentUserId,
      string CurrentUserRole) : IRequest<SupportRequestDto>;
}

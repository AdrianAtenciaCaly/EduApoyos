using EduApoyos.Application.DTOs.SupportRequests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Application.Features.SupportRequests.Commands
{
    public record UpdateStatusCommand(
        Guid SupportRequestId,
        UpdateStatusRequest Request,
        Guid AdvisorId) : IRequest<SupportRequestDto>;
}

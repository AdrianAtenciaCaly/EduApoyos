using EduApoyos.Application.DTOs.SupportRequests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Application.Features.SupportRequests.Queries
{
    public record GetSupportRequestsByStudentQuery(
        Guid StudentId,
        Guid CurrentUserId) : IRequest<List<SupportRequestDto>>;
}

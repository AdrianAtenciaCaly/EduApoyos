using EduApoyos.Application.Common;
using EduApoyos.Application.DTOs.SupportRequests;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Application.Features.SupportRequests.Queries
{
    public record GetSupportRequestsQuery(
       string? Status = null,
       string? Type = null,
       int Page = 1,
       int PageSize = 10) : IRequest<PagedResult<SupportRequestDto>>;
}

using EduApoyos.Application.Common;
using EduApoyos.Application.DTOs.Students;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Application.Features.Students.Queries
{
    public record GetStudentsQuery(int Page = 1, int PageSize = 10) : IRequest<PagedResult<StudentDto>>;
}

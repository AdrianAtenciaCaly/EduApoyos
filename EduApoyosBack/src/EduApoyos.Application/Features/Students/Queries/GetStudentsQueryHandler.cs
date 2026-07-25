using EduApoyos.Application.Common;
using EduApoyos.Application.DTOs.Students;
using EduApoyos.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Application.Features.Students.Queries
{
    public class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, PagedResult<StudentDto>>
    {
        private readonly IStudentRepository _repository;

        public GetStudentsQueryHandler(IStudentRepository repository) => _repository = repository;

        public async Task<PagedResult<StudentDto>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
        {
            var (items, total) = await _repository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);

            return new PagedResult<StudentDto>
            {
                Items = items.Select(s => new StudentDto(
                    s.Id, s.UserId, s.User.FullName, s.User.Email,
                    s.DocumentNumber, s.DocumentType, s.AcademicProgram, s.Semester)).ToList(),
                TotalCount = total,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}

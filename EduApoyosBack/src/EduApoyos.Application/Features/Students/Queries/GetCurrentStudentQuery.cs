using EduApoyos.Application.DTOs.Students;
using MediatR;

namespace EduApoyos.Application.Features.Students.Queries
{
    public record GetCurrentStudentQuery(Guid CurrentUserId) : IRequest<StudentDto>;
}

using EduApoyos.Application.DTOs.Students;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Application.Features.Students.Commands
{
    public record CreateStudentCommand(CreateStudentRequest Request) : IRequest<StudentDto>;
}

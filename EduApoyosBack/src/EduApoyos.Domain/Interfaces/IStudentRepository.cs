using EduApoyos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Domain.Interfaces
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<(IReadOnlyList<Student> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<Student?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<Student?> GetByIdWithUserAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

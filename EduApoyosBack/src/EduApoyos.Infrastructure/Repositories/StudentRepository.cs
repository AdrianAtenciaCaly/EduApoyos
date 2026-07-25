using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Interfaces;
using EduApoyos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Infrastructure.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(ApplicationDbContext context) : base(context) { }

        public async Task<(IReadOnlyList<Student> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = Context.Students.Include(x => x.User).AsNoTracking();
            var total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.User.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            return (items, total);
        }

        public async Task<Student?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => await Context.Students.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        public async Task<Student?> GetByIdWithUserAsync(Guid id, CancellationToken cancellationToken = default)
            => await Context.Students.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}

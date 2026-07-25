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
    public class SupportRequestRepository : Repository<SupportRequest>, ISupportRequestRepository
    {
        public SupportRequestRepository(ApplicationDbContext context) : base(context) { }

        public async Task<SupportRequest?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
            => await Context.SupportRequests
                .Include(x => x.Student).ThenInclude(s => s.User)
                .Include(x => x.StatusHistories)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<(IReadOnlyList<SupportRequest> Items, int TotalCount)> GetPagedAsync(
            RequestStatus? status, SupportType? type, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = Context.SupportRequests
                .Include(x => x.Student).ThenInclude(s => s.User)
                .Include(x => x.StatusHistories)
                .AsNoTracking()
                .AsQueryable();

            if (status.HasValue) query = query.Where(x => x.Status == status.Value);
            if (type.HasValue) query = query.Where(x => x.Type == type.Value);

            var total = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }
        public async Task<SupportRequest> ChangeStatusAsync(
    Guid id,
    RequestStatus newStatus,
    Guid advisorId,
    string? observation,
    CancellationToken cancellationToken = default)
        {
            // Load ONLY the aggregate root (no Includes) to avoid spurious updates on children
            var entity = await Context.SupportRequests
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                ?? throw new KeyNotFoundException("Support request not found.");

            var previousStatus = entity.Status;

            if (previousStatus == newStatus)
            {
                // Still return with details for the response
                return await GetByIdWithDetailsAsync(id, cancellationToken)
                    ?? entity;
            }

            // ExecuteUpdate writes SQL directly – avoids concurrency issues with private setters / graph
            var rows = await Context.SupportRequests
                .Where(x => x.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Status, newStatus)
                    .SetProperty(x => x.AdvisorId, advisorId)
                    .SetProperty(x => x.UpdatedAt, DateTime.UtcNow),
                    cancellationToken);

            if (rows == 0)
                throw new InvalidOperationException($"Support request '{id}' was not updated. It may have been deleted.");

            // Insert history row (new entity only)
            Context.StatusHistories.Add(new StatusHistory(
                id,
                previousStatus,
                newStatus,
                advisorId,
                observation));

            await Context.SaveChangesAsync(cancellationToken);

            // Return full details for the API response
            return await GetByIdWithDetailsAsync(id, cancellationToken)
                ?? throw new InvalidOperationException("Support request was updated but could not be reloaded.");
        }

        public async Task<IReadOnlyList<SupportRequest>> GetByStudentIdAsync(
            Guid studentId, CancellationToken cancellationToken = default)
            => await Context.SupportRequests
                .Include(x => x.Student).ThenInclude(s => s.User)
                .Include(x => x.StatusHistories)
                .Where(x => x.StudentId == studentId)
                .OrderByDescending(x => x.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
    }


}

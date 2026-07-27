using EduApoyos.Domain.Entities;

namespace EduApoyos.Domain.Interfaces
{
    public interface ISupportRequestRepository : IRepository<SupportRequest>
    {
        Task<SupportRequest?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<SupportRequest> Items, int TotalCount)> GetPagedAsync(
            RequestStatus? status, SupportType? type, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SupportRequest>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
        Task<SupportRequest> ChangeStatusAsync(
        Guid id,
        RequestStatus newStatus,
        Guid advisorId,
        string? observation,
        CancellationToken cancellationToken = default);
    }
}

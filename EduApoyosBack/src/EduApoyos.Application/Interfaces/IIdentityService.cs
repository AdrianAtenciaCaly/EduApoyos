using EduApoyos.Domain.Entities;

namespace EduApoyos.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<(bool Succeeded, Guid UserId, string? Error)> CreateUserAsync(string email, string password, string fullName, UserRole role, CancellationToken ct = default);
        Task<(bool Succeeded, User? User)> ValidateCredentialsAsync(string email, string password, CancellationToken ct = default);
        Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct = default);
    }
}

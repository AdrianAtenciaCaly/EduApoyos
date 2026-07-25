using EduApoyos.Domain.Entities;

namespace EduApoyos.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
        DateTime GetExpiration();
    }
}

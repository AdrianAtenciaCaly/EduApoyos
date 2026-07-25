namespace EduApoyos.Application.DTOs.Auth
{
    public record LoginRequest(string Email, string Password);
    public record RegisterRequest(string FullName, string Email, string Password, string Role, string? DocumentNumber = null, string? DocumentType = null, string? AcademicProgram = null, int? Semester = null);
    public record AuthResponse(string Token, DateTime Expiration, Guid UserId, string FullName, string Email, string Role);
}

namespace EduApoyos.Application.DTOs.SupportRequests
{
    public record SupportRequestDto(Guid Id, Guid StudentId, string StudentName, string Type, decimal RequestedAmount, string Description, string Status, DateTime CreatedAt, DateTime UpdatedAt, Guid? AdvisorId, List<StatusHistoryDto> History);
    public record StatusHistoryDto(string PreviousStatus, string NewStatus, DateTime ChangedAt, string? Observation);
    public record CreateSupportRequestRequest(Guid StudentId, string Type, decimal RequestedAmount, string Description);
    public record UpdateStatusRequest(string NewStatus, string? Observation);
}

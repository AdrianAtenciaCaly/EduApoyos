namespace EduApoyos.Domain.Entities
{
    public class SupportRequest
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid StudentId { get; private set; }
        public SupportType Type { get; private set; }
        public decimal RequestedAmount { get; private set; }
        public string Description { get; private set; } = null!;
        public RequestStatus Status { get; private set; } = RequestStatus.Pending;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
        public Guid? AdvisorId { get; private set; }
        public Student Student { get; private set; } = null!;
        public User? Advisor { get; private set; }
        public ICollection<StatusHistory> StatusHistories { get; private set; } = new List<StatusHistory>();

        private SupportRequest() { }

        public SupportRequest(Guid studentId, SupportType type, decimal requestedAmount, string description)
        {
            if (requestedAmount <= 0)
                throw new ArgumentOutOfRangeException(nameof(requestedAmount), "Amount must be greater than zero.");
            StudentId = studentId;
            Type = type;
            RequestedAmount = requestedAmount;
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public void ChangeStatus(RequestStatus newStatus, Guid advisorId, string? observation)
        {
            if (Status == newStatus) return;
            StatusHistories.Add(new StatusHistory(Id, Status, newStatus, advisorId, observation));
            Status = newStatus;
            AdvisorId = advisorId;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public enum SupportType { Scholarship = 1, Credit = 2, Subsidy = 3 }
    public enum RequestStatus { Pending = 1, UnderReview = 2, Approved = 3, Rejected = 4 }
}

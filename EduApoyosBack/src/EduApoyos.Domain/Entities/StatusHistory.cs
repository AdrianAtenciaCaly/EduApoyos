using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Domain.Entities
{
    public class StatusHistory
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid SupportRequestId { get; private set; }
        public RequestStatus PreviousStatus { get; private set; }
        public RequestStatus NewStatus { get; private set; }
        public DateTime ChangedAt { get; private set; } = DateTime.UtcNow;
        public Guid UserId { get; private set; }
        public string? Observation { get; private set; }
        public SupportRequest SupportRequest { get; private set; } = null!;
        public User User { get; private set; } = null!;

        private StatusHistory() { }

        public StatusHistory(Guid supportRequestId, RequestStatus previous, RequestStatus next, Guid userId, string? observation)
        {
            SupportRequestId = supportRequestId;
            PreviousStatus = previous;
            NewStatus = next;
            UserId = userId;
            Observation = observation;
        }
    }
}

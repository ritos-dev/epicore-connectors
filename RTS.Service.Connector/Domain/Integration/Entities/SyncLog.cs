using RTS.Service.Connector.Domain.Enums;

namespace RTS.Service.Connector.Domain.Integration.Entities
{
    public class SyncLog
    {
        public Guid Id { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string Message { get; private set; }
        public SyncLogEventType EventType { get; private set; }
        public string? Details { get; private set; }

        internal SyncLog(DateTime timestamp, string message, SyncLogEventType eventType, string? details)
        {
            Id = Guid.NewGuid();
            Timestamp = timestamp;
            Message = message;
            EventType = eventType;
            Details = details;
        }
    }
}

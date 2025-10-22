using Ardalis.GuardClauses;
using Connector.Domain.Integration.Aggregates.IntegrationJob;
using RTS.Service.Connector.Domain.Enums;
using RTS.Service.Connector.Domain.Integration.Entities;
using RTS.SharedKernel.DDD;

namespace RTS.Service.Connector.Domain.Integration
{
    public class IntegrationJob : AggregateRoot
    {
        private readonly List<Mapping> _mappings = new();
        private readonly List<SyncLog> _logs = new();

        public Guid Id { get; private set; } // ID for the internal job
        public string ExternalRef { get; private set; } // This is the order ID from Tracelink
        public string JobType { get; private set; } // CreateDraft or Resync
        public IntegrationJobStatus Status { get; private set; } 
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        public IReadOnlyCollection<Mapping> Mappings => _mappings.AsReadOnly();
        public IReadOnlyCollection<SyncLog> Logs => _logs.AsReadOnly();

        private IntegrationJob(string externalRef, string jobType)
        {
            Id = Guid.NewGuid();
            ExternalRef = externalRef;
            JobType = jobType;
            Status = IntegrationJobStatus.Planned;
            CreatedAt = DateTime.UtcNow;
            AddLog("Integration job created.", SyncLogEventType.Info);
        }

        public static IntegrationJob Create(string externalRef) => new(externalRef, "SyncOrder"); // Factory method to create a job in "planned" status. 

        public void Start()
        {
            if (Status != IntegrationJobStatus.Planned)
            {
                throw new InvalidOperationException("Job already started or completed.");
            }

            Status = IntegrationJobStatus.Running;
            AddLog("Integration job started.", SyncLogEventType.Info);
        }

        public void RecordMappingFromTraceLink(string field, string traceLinkValue, string economicValue)
        {
            var mapping = new Mapping(field, traceLinkValue, economicValue);
            _mappings.Add(mapping);

            AddLog($"Mapping recorded from TraceLink: {field} = {traceLinkValue} → {economicValue}", SyncLogEventType.Info);
        }

        public void AddLog(string message, SyncLogEventType eventType, string? details = null)
            => _logs.Add(new SyncLog(DateTime.UtcNow, message, eventType, details));

        public void MarkSucceeded()
        {
            if (Status != IntegrationJobStatus.Running)
            {
                throw new InvalidOperationException("Job not running.");
            }

            Status = IntegrationJobStatus.Succeeded;
            CompletedAt = DateTime.UtcNow;
            AddLog("Integration job succeeded.", SyncLogEventType.Info);
        }

        public void MarkFailed(string reason)
        {
            if (Status != IntegrationJobStatus.Running)
            {
                throw new InvalidOperationException("Job not running.");
            }  

            Status = IntegrationJobStatus.Failed;
            CompletedAt = DateTime.UtcNow;
            AddLog($"Integration job failed: {reason}", SyncLogEventType.Error);
        }

        public void MarkCancelled(string reason)
        {
            if (Status == IntegrationJobStatus.Succeeded)
            {
                throw new InvalidOperationException("Cannot cancel succeeded job.");
            }

            Status = IntegrationJobStatus.Cancelled;
            CompletedAt = DateTime.UtcNow;
            AddLog($"Integration job cancelled: {reason}", SyncLogEventType.Warning);
        }
    }
}

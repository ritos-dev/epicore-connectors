using RTS.Service.Connector.Domain.Enums;

namespace RTS.Service.Connector.Domain.Integration.Entities
{
    public class Mapping
    {
        public Guid Id { get; private set; }
        public DomainMappingType Type { get; private set; }
        public string SourceValue { get; private set; }
        public string TargetValue { get; private set; }
        public string RuleUsed { get; private set; }
        public string Version { get; private set; }
        public bool IsFallback { get; private set; }
        public DateTime CreatedAt { get; private set; }

        internal Mapping(DomainMappingType type, string sourceValue, string targetValue, string ruleUsed, string version, bool isFallback)
        {
            Id = Guid.NewGuid();
            Type = type;
            SourceValue = sourceValue;
            TargetValue = targetValue;
            RuleUsed = ruleUsed;
            Version = version;
            IsFallback = isFallback;
            CreatedAt = DateTime.UtcNow;
        }
    }
}

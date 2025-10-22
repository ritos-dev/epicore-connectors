using RTS.Service.Connector.Domain.Enums;

namespace RTS.Service.Connector.Domain.Configuration
{
    public class ConfigurationSetting
    {
        public Guid Id { get; private set; }
        public IntegrationSystem System { get; private set; } // TraceLink or Economic
        public string BaseUrl { get; private set; }
        public string ApiKey { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private ConfigurationSetting(IntegrationSystem system, string baseUrl, string apiKey)
        {
            Id = Guid.NewGuid();
            System = system;
            BaseUrl = baseUrl;
            ApiKey = apiKey;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
        }

        public static ConfigurationSetting Create(IntegrationSystem system, string baseUrl, string apiKey)
            => new(system, baseUrl, apiKey);

        public void UpdateApiKey(string newKey)
        {
            ApiKey = newKey;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

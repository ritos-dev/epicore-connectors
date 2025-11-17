namespace RTS.Service.Connector.Infrastructure.Economic
{
    public sealed class EconomicOptions
    {
        public const string SectionName = "Economic";
        public string BaseUrl { get; init; } = string.Empty;
        public string AppSecretToken { get; init; } = string.Empty;
        public string AgreementGrantToken { get; init; } = string.Empty;
        public int TimeoutSeconds { get; init; } = 30;
        public int TestTotalAmount { get; init; }

        // Endpoints
        public EndpointsOptions Endpoints { get; init; } = new();
        public sealed class EndpointsOptions
        {
            public string CreateDraft { get; init; } = string.Empty;
            public string GetOrderDraft { get; init; } = string.Empty;
        }
    }
}

namespace RTS.Service.Connector.Infrastructure.Tracelink
{
    public sealed class TracelinkOptions
    {
        public const string SectionName = "Tracelink";

        public string BaseUrl { get; init; } = string.Empty;
        public string ApiToken { get; init; } = string.Empty;
        public int TimeoutSeconds { get; init; } = 30;
        public int RetryCount { get; init; } = 2;
    }
}

namespace RTS.Service.Connector.API.Options
{
    public sealed class TracelinkOptions
    {
        public const string? SectionName = "Tracelink";
        public string? WebhookSecret { get; init; }
    }
}

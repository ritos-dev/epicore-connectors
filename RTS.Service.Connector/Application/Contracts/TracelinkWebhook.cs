namespace RTS.Service.Connector.Application.Contracts
{
    public sealed class OrderWebhookRequest
    {
        public string? OrderNumber { get; init; }
    }
}

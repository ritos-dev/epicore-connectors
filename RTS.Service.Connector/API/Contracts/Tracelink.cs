namespace RTS.Service.Connector.API.Contracts
{
    public sealed class OrderWebhookRequest
    {
        public string? OrderNumber { get; init; }
    }
}

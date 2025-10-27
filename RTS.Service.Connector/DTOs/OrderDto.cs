namespace RTS.Service.Connector.DTOs
{
    public sealed record OrderDto
    {
        public string OrderId { get; init; } = default!;
        public string Company { get; init; } = default!;
        public string Number { get; init; } = default!;
        public string Name { get; init; } = default!;
        public string? Description { get; init; }
        public string State { get; init; } = default!;
        public DateTime? StartDate { get; init; }
        public DateTime? DeadlineDate { get; init; }
        public string? OrderSourceData { get; init; }
        public int CustomerId { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}

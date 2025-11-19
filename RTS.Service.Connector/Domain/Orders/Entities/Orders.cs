namespace RTS.Service.Connector.Domain.Orders.Entities
{
    public class Orders
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public string? CrmId { get; set; } // CRM activity ID, RTS tracks this throughout the process
        public int? OrderNumber { get; set; } = 0; // Tracelink stores an order number for each CRM activity converted to order
        
    }
}

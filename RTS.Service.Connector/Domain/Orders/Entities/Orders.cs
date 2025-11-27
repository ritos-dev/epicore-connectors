namespace RTS.Service.Connector.Domain.Orders.Entities
{
    public class Orders
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }
        public string? CrmNumber { get; set; } // RTS uses this to track projects throughout the process
        public int? OrderNumber { get; set; } = 0; // Tracelink stores an order number for each CRM activity converted to order
        
    }
}

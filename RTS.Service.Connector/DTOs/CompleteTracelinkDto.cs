// IMPORTANT! This dto is only used to save tracelink data to db AND to help create an invoice in economic.
// This dto is NOT tied to tracelink endpoints.

namespace RTS.Service.Connector.DTOs
{
    public class CompleteTracelinkDto
    {
        // From /order/list
        public string? OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string? CustomerName { get; set; }

        // From /order/{id}
        public string? Description { get; set; }
        public string? State { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DeadlineDate { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // From /object/list/module/customer
        public string CustomerId { get; set; } = string.Empty;
        public string? CustomerAddress { get; set; }
        public string? CustomerPostalCode { get; set; }
        public string? CustomerCity { get; set; }

        // From /object/list/module/crm
        public string? CrmNumber { get; set; }
    }
}

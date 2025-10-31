using RTS.Service.Connector.Domain.Enums;

namespace RTS.Service.Connector.Domain.Invoices.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public string? ExternalInvoiceId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public string? CustomerNumber { get; set; }
        public Currency Currency { get; set; } = Currency.DKK;
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Draft";
        public string SourceSystem { get; set; } = "TraceLink";
        public string TargetSystem { get; set; } = "e-conomic";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation property for invoice lines
        public ICollection<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
    }
}

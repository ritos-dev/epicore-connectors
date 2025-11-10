using RTS.Service.Connector.Domain.Enums;

namespace RTS.Service.Connector.Domain.Invoices.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        public string CrmId { get; set; } = string.Empty;
        public string? TLOrderNumber { get; set; }
        public string? InvoiceId { get; set; }
        public string CustomerName { get; set; } = null!;
        public Currency Currency { get; set; } = Currency.DKK;
        public DateTime InvoiceCreateDate { get; set; } = DateTime.UtcNow;
        public DateTime? InvoiceDueDate { get; set; }
        public decimal InvoiceAmount { get; set; }
        public int InvoiceNumber { get; set; } // example: 1 out of 3
        public string Status { get; set; } = "Draft";
        public DateTime? UpdatedAt { get; set; }


        // Navigation property for invoice lines
        public ICollection<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
    }
}

using RTS.Service.Connector.Domain.Enums;
using RTS.Service.Connector.Domain.Invoices.Entities;

namespace RTS.Service.Connector.Domain.SummaryInvoiceReport.Entities
{
    public class SummaryReport
    {
        public int Id { get; set; }
        public string CrmId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public int ExpectedInvoices { get; set; }
        public int InvoiceCount { get; set; }
        public decimal TotalAmount { get; set; }
        public Currency Currency { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}




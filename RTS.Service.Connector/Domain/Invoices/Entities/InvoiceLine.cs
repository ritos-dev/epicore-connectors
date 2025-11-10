namespace RTS.Service.Connector.Domain.Invoices.Entities
{
    public class InvoiceLine
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }  // FK

        public int LineNumber { get; set; }
        public string ProductNumber { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal VatRate { get; set; }
        public decimal LineTotal { get; set; }

        // Navigation
        public Invoice Invoice { get; set; } = null!;
    }
}

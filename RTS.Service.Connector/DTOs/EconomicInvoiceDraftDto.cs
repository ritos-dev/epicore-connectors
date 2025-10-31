namespace RTS.Service.Connector.Infrastructure.Economic.Models
{
    public sealed class EconomicInvoiceDraft
    {
        public string Date { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd");
        public string? Currency { get; set; }

        public EconomicCustomer Customer { get; set; } = new();
        public EconomicPaymentTerms PaymentTerms { get; set; } = new();
        public EconomicLayout Layout { get; set; } = new();
        public EconomicRecipient Recipient { get; set; } = new();
        public EconomicReferences References { get; set; } = new() { Other = "TraceLink" };
        public EconomicTotals Totals { get; set; } = new();
        public EconomicNotes Notes { get; set; } = new();
        public List<EconomicInvoiceLine> Lines { get; set; } = new();
    }

    public sealed class EconomicCustomer { public int CustomerNumber { get; set; } }
    public sealed class EconomicPaymentTerms { public int PaymentTermsNumber { get; set; } }
    public sealed class EconomicLayout { public int LayoutNumber { get; set; } }
    public sealed class EconomicVatZone { public int VatZoneNumber { get; set; } }
    public sealed class EconomicReferences { public string Other { get; set; } = string.Empty; }

    public sealed class EconomicInvoiceLine
    {
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VatRate { get; set; }
    }

    public sealed class EconomicTotals
    {
        public decimal NetAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal GrossAmount { get; set; }
    }

    public sealed class EconomicRecipient
    {
        public string? Name { get; set; }
        public EconomicVatZone VatZone { get; set; } = new();
    }

    public sealed class EconomicNotes
    {
        public string? TextLine1 { get; set; } = string.Empty;
        public string? TextLine2 { get; set; } = string.Empty;
    }
}

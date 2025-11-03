using Newtonsoft.Json;

namespace RTS.Service.Connector.DTO;

public sealed class EconomicInvoiceDraft
{
    [JsonProperty("date")]
    public string Date { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd");

    [JsonProperty("currency")]
    public string? Currency { get; set; }

    [JsonProperty("customer")]
    public EconomicCustomer Customer { get; set; } = new();

    [JsonProperty("paymentTerms")]
    public EconomicPaymentTerms PaymentTerms { get; set; } = new();

    [JsonProperty("layout")]
    public EconomicLayout Layout { get; set; } = new();

    [JsonProperty("recipient")]
    public EconomicRecipient Recipient { get; set; } = new();

    [JsonProperty("references")]
    public EconomicReferences References { get; set; } = new() { Other = "TraceLink" };

    [JsonProperty("totals")]
    public EconomicTotals Totals { get; set; } = new();

    [JsonProperty("notes")]
    public EconomicNotes Notes { get; set; } = new();

    [JsonProperty("lines")]
    public List<EconomicInvoiceLine> Lines { get; set; } = new();
}

public sealed class EconomicCustomer 
{
    [JsonProperty("customerNumber")]
    public int CustomerNumber { get; set; } 
}
public sealed class EconomicPaymentTerms 
{
    [JsonProperty("paymentTermsNumber")]
    public int PaymentTermsNumber { get; set; } 
}
public sealed class EconomicLayout 
{
    [JsonProperty("layoutNumber")]
    public int LayoutNumber { get; set; } 
}
public sealed class EconomicVatZone 
{
    [JsonProperty("vatZoneNumber")]
    public int VatZoneNumber { get; set; } 
}
public sealed class EconomicReferences 
{
    [JsonProperty("other")]
    public string Other { get; set; } = string.Empty; 
}

public sealed class EconomicInvoiceLine
{
    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("quantity")]
    public int Quantity { get; set; }

    [JsonProperty("unitPrice")]
    public decimal UnitPrice { get; set; }

    [JsonProperty("vatRate")]
    public decimal VatRate { get; set; }
}

public sealed class EconomicTotals
{
    [JsonProperty("netAmount")]
    public decimal NetAmount { get; set; }

    [JsonProperty("vatAmount")]
    public decimal VatAmount { get; set; }

    [JsonProperty("grossAmount")]
    public decimal GrossAmount { get; set; }
}

public sealed class EconomicRecipient
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("vatZone")]
    public EconomicVatZone VatZone { get; set; } = new();
}

public sealed class EconomicNotes
{
    [JsonProperty("textLine1")]
    public string? TextLine1 { get; set; } = string.Empty;

    [JsonProperty("textLine2")]
    public string? TextLine2 { get; set; } = string.Empty;
}

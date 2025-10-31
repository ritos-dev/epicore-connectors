using System.Text.Json;
using RTS.Service.Connector.Infrastructure.Economic.Models;

namespace RTS.Service.Connector.Infrastructure.Economic
{
    public static class EconomicInvoiceMapper
    {
        public static EconomicInvoiceDraft MapToInvoiceDraft(string orderJson, string orderNumber)
        {
            var orderDoc = JsonDocument.Parse(orderJson);
            var root = orderDoc.RootElement;

            // Minimal required for invoice draft
            var draft = new EconomicInvoiceDraft
            {
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Currency = root.GetProperty("currency").GetString(),

                Customer = new EconomicCustomer
                {
                    CustomerNumber = root.GetProperty("customer").GetProperty("customerNumber").GetInt32()
                },
                PaymentTerms = new EconomicPaymentTerms
                {
                    PaymentTermsNumber = root.GetProperty("paymentTerms").GetProperty("paymentTermsNumber").GetInt32()
                },
                Layout = new EconomicLayout
                {
                    LayoutNumber = root.GetProperty("layout").GetProperty("layoutNumber").GetInt32()
                },
                Recipient = new EconomicRecipient
                {
                    Name = root.GetProperty("recipient").GetProperty("name").GetString(),
                    VatZone = new EconomicVatZone
                    {
                        VatZoneNumber = root.GetProperty("recipient").GetProperty("vatZone").GetProperty("vatZoneNumber").GetInt32()
                    }
                },
                References = new EconomicReferences { Other = "TraceLink Project Name" }
            };

            // Optional fields for invoice
            draft.Notes = new EconomicNotes
            {
                TextLine1 = $"Tracelink order #{orderNumber}",
                TextLine2 = $"Generated on {DateTime.UtcNow:yyyy-MM-dd HH:mm}"
            };

            if (root.TryGetProperty("grossAmount", out var grossProp))
                draft.Totals.GrossAmount = grossProp.GetDecimal();

            if (root.TryGetProperty("netAmount", out var netProp))
                draft.Totals.NetAmount = netProp.GetDecimal();

            if (root.TryGetProperty("vatAmount", out var vatProp))
                draft.Totals.VatAmount = vatProp.GetDecimal();

            if (root.TryGetProperty("lines", out var linesProp) && linesProp.ValueKind == JsonValueKind.Array)
            {
                foreach (var line in linesProp.EnumerateArray())
                {
                    var mappedLine = new EconomicInvoiceLine
                    {
                        Description = line.GetProperty("description").GetString(),
                        Quantity = line.GetProperty("quantity").GetInt32(),
                        UnitPrice = line.GetProperty("unitPrice").GetDecimal(),
                        VatRate = line.GetProperty("vatRate").GetDecimal()
                    };
                    draft.Lines.Add(mappedLine);
                }
            }

            return draft;
        }
    }
}

using Newtonsoft.Json.Linq;
using RTS.Service.Connector.DTO;

namespace RTS.Service.Connector.Infrastructure.Economic
{
    public static class EconomicInvoiceMapper
    {
        public static EconomicInvoiceDraft MapToInvoiceDraft(string orderJson, string orderNumber)
        {
            var root = JObject.Parse(orderJson);

            // Minimal required fields for invoice draft
            var draft = new EconomicInvoiceDraft
            {
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Currency = root["currency"]?.ToString(),

                Customer = new EconomicCustomer
                {
                    CustomerNumber = (int)(root["customer"]?["customerNumber"] ?? 0)
                },

                PaymentTerms = new EconomicPaymentTerms
                {
                    PaymentTermsNumber = (int)(root["paymentTerms"]?["paymentTermsNumber"] ?? 0)
                },

                Layout = new EconomicLayout
                {
                    LayoutNumber = (int)(root["layout"]?["layoutNumber"] ?? 0)
                },

                Recipient = new EconomicRecipient
                {
                    Name = root["recipient"]?["name"]?.ToString(),
                    
                    VatZone = new EconomicVatZone
                    {
                        VatZoneNumber = (int)(root["recipient"]?["vatZone"]?["vatZoneNumber"] ?? 0)
                    }
                },

                References = new EconomicReferences { Other = "Tracelink project (CRM) number/name." }
            };

            // non required fields
            draft.Notes = new EconomicNotes
            {
                TextLine1 = $"Tracelink order #{orderNumber}",
                TextLine2 = $"Generated on {DateTime.UtcNow:yyyy-MM-dd HH:mm}"
            };

            draft.Totals = new EconomicTotals
            {
                GrossAmount = (decimal?)root["grossAmount"] ?? 0,
                NetAmount = (decimal?)root["netAmount"] ?? 0,
                VatAmount = (decimal?)root["vatAmount"] ?? 0,
            };

            var lines = root["lines"] as JArray;
            if (lines != null)
            {
                foreach (var line in lines)
                {
                    draft.Lines.Add(new EconomicInvoiceLine
                    {
                        Description = line["description"]?.ToString(),
                        Quantity = (int)(line["quantity"] ?? 0),
                        UnitPrice = (decimal)(line["unitPrice"] ?? 0),
                        VatRate = (decimal)(line["vatRate"] ?? 0)
                    });
                }
            }

            return draft;
        }
    }
}

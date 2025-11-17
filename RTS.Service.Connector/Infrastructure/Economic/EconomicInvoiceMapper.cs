using Newtonsoft.Json.Linq;

using RTS.Service.Connector.DTOs;
using RTS.Service.Connector.Infrastructure.InvoiceSplit;

namespace RTS.Service.Connector.Infrastructure.Economic
{
    public class EconomicInvoiceMapper
    {
        public EconomicInvoiceDraft MapToInvoiceDraft(string orderJson, CompleteTracelinkDto tracelink, InvoicePart invoicePart)
        {
            var root = JObject.Parse(orderJson);

            // Minimal required fields for invoice draft
            var draft = new EconomicInvoiceDraft
            {
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),

                Currency = root["currency"]?.ToString(),

                Customer = new EconomicCustomer
                {
                    CustomerNumber = (int)(root["customer"]?["customerNumber"] ?? 0),
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
                    Name = tracelink.CustomerName,
                    CustomerAddress = tracelink.CustomerAddress,
                    CustomerZipCode = tracelink.CustomerPostalCode,
                    CustomerCity = tracelink.CustomerCity,

                    VatZone = new EconomicVatZone
                    {
                        VatZoneNumber = (int)(root["recipient"]?["vatZone"]?["vatZoneNumber"] ?? 0)
                    }
                },

                Lines = new List<EconomicInvoiceLine>()
            };

            var referenceText = "Beløbet svarer til " + (invoicePart.Percentage * 100).ToString("0") + "% af det aftalte beløb.";
            draft.Notes = new EconomicNotes
            {
                TextLine1 = $"Komplet ifølge aftale: #{tracelink.CrmNumber} \n{referenceText}",
                TextLine2 = $""
            };

            draft.Lines.Add(new EconomicInvoiceLine
            {
                Description = invoicePart.Description,
                Quantity = 1,
                UnitNetPrice = invoicePart.Amount,
                VatRate = 0,
                Product = new EconomicProducts
                {
                    ProductNumber = invoicePart.ProductNumber
                }
            });

            return draft;
        }
    }
}

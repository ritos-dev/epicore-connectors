using RTS.Service.Connector.DTOs;
using RTS.Service.Connector.Infrastructure.InvoiceSplit;

namespace RTS.Service.Connector.Infrastructure.Economic
{
    public class EconomicInvoiceMapper
    {
        public EconomicInvoiceDraftDto MapToInvoiceDraft(EconomicInvoiceDraftDto dto, CompleteTracelinkDto tracelink, InvoicePart invoicePart)
        {
            // Minimal required fields for an invoice draft to be created in economic
            var draft = new EconomicInvoiceDraftDto
            {
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),

                Currency = dto.Currency,

                Customer = new EconomicCustomer
                {
                    CustomerNumber = dto.Customer.CustomerNumber,
                },

                PaymentTerms = new EconomicPaymentTerms
                {
                    PaymentTermsNumber = dto.PaymentTerms.PaymentTermsNumber,
                },

                Layout = new EconomicLayout
                {
                    LayoutNumber = dto.Layout.LayoutNumber,
                },

                Recipient = new EconomicRecipient
                {
                    Name = tracelink.CustomerName,
                    CustomerAddress = tracelink.CustomerAddress,
                    CustomerZipCode = tracelink.CustomerPostalCode,
                    CustomerCity = tracelink.CustomerCity,

                    VatZone = new EconomicVatZone
                    {
                        VatZoneNumber = dto.Recipient.VatZone.VatZoneNumber,
                    }
                },

                Lines = new List<EconomicInvoiceLine>()
            };

            // Other field(s)

            var referenceText = "Beløbet svarer til " + (invoicePart.Percentage * 100).ToString("0") + "% af det aftalte beløb.";
            draft.Notes = new EconomicNotes
            {
                TextLine1 = $"Komplet ifølge aftale: #{tracelink.CrmNumber} \n{referenceText}",
                TextLine2 = $""
            };

            // Invoice line(s)
            draft.Lines.Add(new EconomicInvoiceLine
            {
                Description = invoicePart.Description,
                Quantity = 1,
                UnitNetPrice = invoicePart.NetPrice,
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

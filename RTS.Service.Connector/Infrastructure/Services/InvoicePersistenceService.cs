using RTS.Service.Connector.DTO;
using RTS.Service.Connector.Domain.Invoices.Entities;

using Newtonsoft.Json;
using RTS.Service.Connector.Domain.Enums;

namespace RTS.Service.Connector.Infrastructure.Services
{
    public sealed class InvoicePersistenceService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<InvoicePersistenceService> _logger;

        public InvoicePersistenceService(AppDbContext context, ILogger<InvoicePersistenceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SaveAsync(string invoiceJson, string orderNumber, string crmNumber, CancellationToken cancellationToken)
        {
            try
            {
                var draft = JsonConvert.DeserializeObject<EconomicInvoiceDraft>(invoiceJson);
                if (draft == null)
                {
                    _logger.LogWarning("[Database] Invoice draft deserialization failed skipping save for order {OrderNumber}.", orderNumber);
                    return;
                }

                var invoice = DbInvoiceMapper.ToEntity(draft, orderNumber, crmNumber);
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("[Database] Saved invoice for order {OrderNumber} ({Lines} lines).", orderNumber, invoice.Lines.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Database] Failed to persist invoice for order {OrderNumber}.", orderNumber);
                throw;
            }
        }
    }

    public static class DbInvoiceMapper
    {
        public static Invoice ToEntity(EconomicInvoiceDraft draft, string orderNumber, string crmNumber)
        {
            var invoice = new Invoice
            {
                OrderNumber = orderNumber,
                CrmId = crmNumber,
                CustomerName = draft.Recipient?.Name ?? "Unknown",
                Currency = Enum.TryParse(draft.Currency, out Currency parsed) ? parsed : Currency.DKK,
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = draft.Totals?.GrossAmount ?? 0,
                Status = "Draft"
            };

            if (draft.Lines != null)
            {
                foreach (var line in draft.Lines)
                {
                    invoice.Lines.Add(new InvoiceLine
                    {
                        Description = line.Description ?? "",
                        Quantity = line.Quantity,
                        UnitPrice = line.UnitPrice,
                        VatRate = line.VatRate,
                        LineTotal = line.Quantity * line.UnitPrice
                    });
                }
            }

            return invoice; 
        }
    }
}

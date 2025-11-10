using RTS.Service.Connector.DTOs;
using RTS.Service.Connector.Domain.Enums;
using RTS.Service.Connector.Domain.Invoices.Entities;

using Newtonsoft.Json;

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

        public async Task SaveInvoiceAsync(string invoiceJson, string orderNumber, string crmNumber, CancellationToken cancellationToken)
        {
            try
            {
                var draft = JsonConvert.DeserializeObject<EconomicInvoiceDraft>(invoiceJson);
                if (draft is null)
                {
                    _logger.LogWarning("[Database] Deserialization failed for order {OrderNumber}.", orderNumber);
                    return;
                }

                var entity = DbInvoiceMapper.ToEntity(draft, orderNumber, crmNumber);

                await _context.Invoices.AddAsync(entity, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("[Database] Invoice saved for order {OrderNumber} with {LineCount} lines.", orderNumber, entity.Lines.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Database] Error saving invoice for order {OrderNumber}.", orderNumber);
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
                TLOrderNumber = orderNumber,
                CrmId = crmNumber,
                CustomerName = draft.Recipient?.Name ?? "Unknown",

                Currency = Enum.TryParse(draft.Currency, true, out Currency parsed)
                    ? parsed
                    : Currency.DKK,

                InvoiceCreateDate = DateTime.UtcNow,
                InvoiceDueDate = CalcDueTime(draft.PaymentTerms),
                InvoiceAmount = draft.Totals?.GrossAmount ?? 0,
                InvoiceNumber = 0, // invoice number out of max invoices for CRM order

                Status = "Draft",
                UpdatedAt = DateTime.UtcNow
            };

            if (draft.Lines != null)
            {
                foreach (var line in draft.Lines)
                {
                    invoice.Lines.Add(new InvoiceLine
                    {
                        Description = line.Description ?? string.Empty,
                        Quantity = line.Quantity,
                        UnitPrice = line.UnitPrice,
                        VatRate = line.VatRate,
                        LineTotal = line.Quantity * line.UnitPrice
                    });
                }
            }

            return invoice;
        }

        private static DateTime? CalcDueTime(EconomicPaymentTerms? terms)
        {
            if (terms == null)
                return DateTime.UtcNow.AddDays(14);

            return DateTime.UtcNow.AddDays(terms.PaymentTermsNumber
            switch
            {
                0 => 8,
                1 => 14,
                2 => 30,
                _ => 14
            });
        }
    }
}

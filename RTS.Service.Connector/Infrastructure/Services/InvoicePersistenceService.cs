using RTS.Service.Connector.DTOs;
using RTS.Service.Connector.Domain.Enums;
using RTS.Service.Connector.Domain.Invoices.Entities;
using RTS.Service.Connector.Domain.SummaryInvoiceReport.Entities;

using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace RTS.Service.Connector.Infrastructure.Services
{
    public class InvoicePersistenceService
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

                var invoice = DbInvoiceMapper.ToEntity(draft, orderNumber, crmNumber);

                var summary = await CreateOrUpdateSummaryReport(crmNumber, invoice.CustomerName, invoice.Currency, invoice.InvoiceAmount, cancellationToken);
                invoice.SummaryReport = summary;

                await _context.Invoices.AddAsync(invoice, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("[Database] Invoice saved for order {OrderNumber} with {LineCount} lines.", orderNumber, invoice.Lines.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Database] Error saving invoice for order {OrderNumber}.", orderNumber);
                throw;
            }
        }

        private async Task<SummaryReport> CreateOrUpdateSummaryReport(string crmNumber, string customerName, Currency currency, decimal invoiceAmount, CancellationToken cancellationToken)
        {
            var summary = await _context.SummaryInvoiceReports.FirstOrDefaultAsync(s => s.CrmId == crmNumber, cancellationToken);


            if (summary == null)
            {
                // Create summary
                summary = new SummaryReport
                {
                    CrmId = crmNumber,
                    CustomerName = customerName,
                    Currency = currency,
                    InvoiceCount = 1,
                    TotalAmount = invoiceAmount,
                    Status = "Not Implemented",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.SummaryInvoiceReports.AddAsync(summary, cancellationToken);
                _logger.LogInformation("[Database] New summary created for CRM {CrmOrderId}.", crmNumber);
            }
            else
            {
                // Update existing summary
                summary.InvoiceCount += 1;
                summary.TotalAmount += invoiceAmount;
                summary.UpdatedAt = DateTime.UtcNow;

                // for later track expected invoices: 
                // if (summary.CreatedInvoices == summary.ExpectedInvoices) summary.Status = "Closed";   Maybe needs to be deleted idk. 
            }

            return summary;
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

                DraftInvoiceNumber = draft.DraftInvoiceNumber,
                InvoiceCreateDate = DateTime.UtcNow,
                InvoiceDueDate = CalcDueTime(draft.PaymentTerms),
                InvoiceAmount = draft.Lines?.Sum(x=>x.Quantity*x.UnitNetPrice) ?? 0,
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
                        UnitPrice = line.UnitNetPrice,
                        VatRate = line.VatRate,
                        LineTotal = line.Quantity * line.UnitNetPrice
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

    

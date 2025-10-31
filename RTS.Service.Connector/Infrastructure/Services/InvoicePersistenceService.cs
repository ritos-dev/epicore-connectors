using RTS.Service.Connector.Domain.Enums;
using RTS.Service.Connector.Domain.Invoices.Entities;

using System.Text.Json;
using RTS.Service.Connector.Infrastructure.Economic.Models;

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

        public async Task SaveAsync(string invoiceJson, string orderNumber, CancellationToken cancellationToken)
        {
            try
            {
                var draft = JsonSerializer.Deserialize<EconomicInvoiceDraft>(invoiceJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (draft == null)
                {
                    _logger.LogWarning("[Database] Invoice draft deserialization failed — skipping save for order {OrderNumber}.", orderNumber);
                    return;
                }

                var invoice = new Invoice
                {
                    OrderNumber = orderNumber,
                    CustomerName = draft.Recipient?.Name ?? "Unknown",
                    CustomerNumber = draft.Customer?.CustomerNumber.ToString() ?? "0",
                    Currency = Enum.TryParse(draft.Currency, out Currency parsed) ? parsed : Currency.DKK,
                    InvoiceDate = DateTime.UtcNow,
                    TotalAmount = draft.Totals.GrossAmount,
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

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("[Database] Saved invoice for order {OrderNumber} ({Lines} lines).",
                    orderNumber, invoice.Lines.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Database] Failed to persist invoice for order {OrderNumber}.", orderNumber);
                throw;
            }
        }
    }
}

using RTS.Service.Connector.DTOs;
using RTS.Service.Connector.Interfaces;
using RTS.Service.Connector.Infrastructure.Economic;
using RTS.Service.Connector.Infrastructure.Tracelink;
using RTS.Service.Connector.Infrastructure.InvoiceSplit;

using Microsoft.Extensions.Options;

namespace RTS.Service.Connector.Infrastructure
{
    public sealed class ConnectorBackgroundWorker : BackgroundService
    {
        private readonly IBackgroundTaskQueue _queue;
        private readonly IEconomicClient _economicClient;
        private readonly ITracelinkClient _tracelinkClient;
        private readonly IOrderSplitToInvoices _split;
        private readonly ILogger<ConnectorBackgroundWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly EconomicOptions _options;
        private readonly EconomicInvoiceMapper _mapper;

        public ConnectorBackgroundWorker(
            IBackgroundTaskQueue queue,
            IEconomicClient economicClient,
            ITracelinkClient tracelinkClient,
            IOrderSplitToInvoices split,
            ILogger<ConnectorBackgroundWorker> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<EconomicOptions> options,
            EconomicInvoiceMapper mapper)
        {
            _queue = queue;
            _economicClient = economicClient;
            _tracelinkClient = tracelinkClient;
            _split = split;
            _logger = logger;
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _mapper = mapper; 
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[Connector] Background Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Get next order number in queue
                    var orderNumber = await _queue.DequeueAsync(stoppingToken);
                    _logger.LogInformation("[Connector] Dequeued order {OrderNumber} fetching from Tracelink...", orderNumber);

                    // Find the order in the list 
                    var listResult = await _tracelinkClient.GetOrderListAsync(orderNumber, stoppingToken);

                    if (!listResult.IsSuccess)
                    {
                        _logger.LogInformation("[Worker] Failed to find order in list");
                        return;
                    }

                    var orderId = listResult.Data!.OrderId;
                    var customerName = listResult.Data!.Name;

                    _logger.LogInformation("[Worker] Order {OrderId} for {CustomerName} found successfully", orderId, customerName);

                    // Get the specific order from the list
                    var orderResult = await _tracelinkClient.GetOrderByIdAsync(orderId, stoppingToken);
                    
                    if(!orderResult.IsSuccess)
                    {
                        _logger.LogInformation("[Worker] Failed to fetch full order.");
                        return;
                    }

                    var orderDetails = orderResult.Data; // for specifics look into TracelinkOrderDto

                    // Find the customer in the list
                    var customerResult = await _tracelinkClient.GetCustomerListAsync(customerName, stoppingToken);

                    if (!customerResult.IsSuccess)
                    {
                        _logger.LogInformation("[Worker] Failed to fetch customer.");
                        return;
                    }
                    _logger.LogInformation("[Worker] Looking up customer with name: '{Name}'", customerName);

                    // Get crm
                    var crmResult = await _tracelinkClient.GetCrmListAsync(customerName, stoppingToken);

                    if (!crmResult.IsSuccess)
                    {
                        _logger.LogInformation("[Worker] Failed to find CRM.");
                        return;
                    }

                    var crmNumber = crmResult.Data!.CrmNumber;
                    _logger.LogInformation("[Worker] Looking up customer with CRM: '{CRM}'", crmNumber);

                    // Combined tracelink dto
                    var combinedDto = TracelinkOrderFactory.Create(listResult.Data!, orderResult.Data!, customerResult.Data!, crmResult.Data!);

                    // Customer type classification
                    var customerType = CustomerTypeClassifier.Classify(combinedDto.CompanyType);
                    _logger.LogInformation("[Worker] Customer type for order {OrderNumber} is {CustomerType}", combinedDto.OrderNumber, customerType);

                    // TEST AMOUNT THIS WILL BE REPLACED!
                    var totalAmount = _options.TestTotalAmount;
                    _logger.LogWarning("[TEST] TEST TOTAL AMOUNT FOR SPLITTING.");

                    // Save order to database
                    /*using (var scope = _scopeFactory.CreateScope())
                    {
                        var persistence = scope.ServiceProvider.GetRequiredService<TracelinkPersistenceService>();
                        await persistence.SaveOrderAsync(combinedDto);
                        _logger.LogInformation("[Worker] Order saved successfully for TraceLink order {OrderNumber}", combinedDto.OrderNumber);
                    }*/

                    // Fetch order draft from Economic
                    var draftResult = await _economicClient.GetOrderDraftIfExistsAsync(combinedDto.OrderNumber, stoppingToken);

                    if (!draftResult.IsSuccess)
                    {
                        _logger.LogInformation("[Worker] Order {OrderNumber} not found or failed: {Error}", orderNumber, draftResult.ErrorMessage);
                        return;
                    }

                    // Split service for invoices
                    var invoiceParts = _split.Split(totalAmount, customerType);
                    _logger.LogInformation("[Split] {Count} invoice parts generated for order {OrderNumber}", invoiceParts.Count, combinedDto.OrderNumber);

                    if (invoiceParts.Count == 0)
                    {
                        _logger.LogInformation("[Split] No invoice parts generated (B2B not implemented yet). Skipping order {OrderNumber}.", combinedDto.OrderNumber);
                        continue;
                    }

                    // Mapping
                    var invoiceDrafts = new List<EconomicInvoiceDraft>();

                    foreach (var part in invoiceParts)
                    {
                        var draft = _mapper.MapToInvoiceDraft(
                            draftResult.Data!,
                            combinedDto,
                            part
                        );

                        invoiceDrafts.Add(draft);

                        _logger.LogInformation("[Mapper] Created invoice draft: '{Desc}' (Amount {Amount}) for order {OrderNumber}", part.Description, part.Amount, combinedDto.OrderNumber);
                    }

                    // Create invoice draft in economic
                    foreach (var draft in invoiceDrafts)
                    {
                        _logger.LogInformation("[Worker] Creating invoice draft for '{Description}' (Amount {Amount})", draft.Lines.First().Description, draft.Lines.First().UnitNetPrice);

                        var invoiceResult = await _economicClient.CreateInvoiceDraftAsync(draft, combinedDto.OrderNumber, combinedDto.CrmNumber!, stoppingToken);

                        if (!invoiceResult.IsSuccess)
                        {
                            _logger.LogError("[Worker] Failed to create invoice for '{Description}' on order {OrderNumber}: {Error}", draft.Lines.First().Description, combinedDto.OrderNumber, invoiceResult.ErrorMessage);
                            continue;
                        }

                        _logger.LogInformation("[Worker] Successfully created invoice for '{Description}' (Order {OrderNumber})", draft.Lines.First().Description, combinedDto.OrderNumber);
                    }

                    // Save invoice to database
                    /*using (var scope = _scopeFactory.CreateScope())
                    {
                        var persistence = scope.ServiceProvider.GetRequiredService<InvoicePersistenceService>();
                        await persistence.SaveInvoiceAsync(invoiceResult.Data!, orderNumber, crmNumber!, stoppingToken);
                        _logger.LogInformation("[Worker] Invoice information saved successfully for order {OrderNumber} with CRM {crmNumber}", orderNumber, crmNumber);
                    }*/
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex, "[Connector] Error while processing Tracelink order from queue.");
                }
            }

            _logger.LogInformation("[Connector] Background Worker stopped.");
        }
    }
}

using RTS.Service.Connector.Interfaces;
using RTS.Service.Connector.Infrastructure.Services;
using RTS.Service.Connector.Infrastructure.Tracelink;

namespace RTS.Service.Connector.Infrastructure
{
    public sealed class ConnectorBackgroundWorker : BackgroundService
    {
        private readonly IBackgroundTaskQueue _queue;
        private readonly IEconomicClient _economicClient;
        private readonly ITracelinkClient _tracelinkClient;
        private readonly ILogger<ConnectorBackgroundWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ConnectorBackgroundWorker(
            IBackgroundTaskQueue queue,
            IEconomicClient economicClient,
            ITracelinkClient tracelinkClient,
            ILogger<ConnectorBackgroundWorker> logger,
            IServiceScopeFactory scopeFactory)
        {
            _queue = queue;
            _economicClient = economicClient;
            _tracelinkClient = tracelinkClient;
            _logger = logger;
            _scopeFactory = scopeFactory;
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

                    var customerId = customerResult.Data!.CustomerId;

                    // Get crm
                    var crmResult = await _tracelinkClient.GetCrmListAsync(customerId, stoppingToken);

                    if (!crmResult.IsSuccess)
                    {
                        _logger.LogInformation("[Worker] Failed to find CRM.");
                        continue;
                    }

                    var crmNumber = crmResult.Data!.CrmNumber;

                    // Combined tracelink dto
                    var combinedDto = TracelinkOrderFactory.Create(listResult.Data!, orderResult.Data!, customerResult.Data, crmResult.Data);

                    // Save order to database
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var persistence = scope.ServiceProvider.GetRequiredService<TracelinkPersistenceService>();
                        await persistence.SaveOrderAsync(combinedDto);
                        _logger.LogInformation("[Worker] Order saved successfully for TraceLink order {OrderNumber}", combinedDto.OrderNumber);
                    }

                    // Fetch order draft from Economic
                    var draftResult = await _economicClient.GetOrderDraftIfExistsAsync(combinedDto.OrderNumber, stoppingToken);
                    if (!draftResult.IsSuccess)
                    {
                        _logger.LogInformation("[Worker] Order {OrderNumber} not found or failed: {Error}", orderNumber, draftResult.ErrorMessage);
                        continue;
                    }

                    _logger.LogInformation("[Worker] Creating invoice draft for Tracelink order {OrderNumber}", orderNumber);

                    // Create invoice draft in Economic
                    var invoiceResult = await _economicClient.CreateInvoiceDraftAsync(draftResult.Data!, combinedDto.OrderNumber, combinedDto.CrmNumber!, stoppingToken);
                    if (!invoiceResult.IsSuccess)
                    {
                        _logger.LogInformation("[Worker] Failed to create invoice draft for order {OrderNumber}", orderNumber);
                        continue;
                    }

                    _logger.LogInformation("[Worker] Invoice draft created successfully for order {OrderNumber}.", orderNumber);

                    // Save invoice to database
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var persistence = scope.ServiceProvider.GetRequiredService<InvoicePersistenceService>();
                        await persistence.SaveInvoiceAsync(invoiceResult.Data!, orderNumber, crmNumber!, stoppingToken);
                        _logger.LogInformation("[Worker] Invoice information saved successfully for order {OrderNumber} with CRM {crmNumber}", orderNumber, crmNumber);
                    }
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

using RTS.Service.Connector.Interfaces;
using RTS.Service.Connector.Infrastructure.Services;

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

                    // Fint the order in the list 
                    var tracelinkResult = await _tracelinkClient.GetOrderListAsync(orderNumber, stoppingToken);
                    if (!tracelinkResult.IsSuccess)
                    {
                        _logger.LogInformation("[Tracelink Worker] Failed to fetch Tracelink order {OrderNumber}: {Error}", orderNumber, tracelinkResult.ErrorMessage);
                        continue;
                    }

                    _logger.LogInformation("[Tracelink Worker] Order {OrderNumber}, {Name}, {CrmId} found and fetched successfully", orderNumber, tracelinkResult.Data!.Name, tracelinkResult.Data.CrmId);

                    // Get the specific order from the list
                    var fullResult = await _tracelinkClient.GetOrderByIdAsync(tracelinkResult.Data!.OrderId, stoppingToken);
                    if(!fullResult.IsSuccess)
                    {
                        _logger.LogInformation("[Tracelink Worker] Failed to fetch full order. {OrderNumber:}, {Error}", orderNumber, fullResult.ErrorMessage);
                        continue;
                    }

                    // Get crm
                    var crmNumber = fullResult.Data?.CrmId;

                    if (string.IsNullOrWhiteSpace(crmNumber))
                    {
                        _logger.LogInformation("[Tracelink Worker] CRM not found for order {OrderNumber}.", orderNumber);
                        continue;
                    }

                    _logger.LogInformation("[Tracelink] Extracted CRM {crmNumber} for order {OrderNumber}.", crmNumber, orderNumber);


                    // Save order to database
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var persistence = scope.ServiceProvider.GetRequiredService<TracelinkPersistenceService>();
                        await persistence.SaveOrderAsync(fullResult.Data!);
                        _logger.LogInformation("[Database] Order information saved successfully for order {order.OrderNumber}", orderNumber);
                    }

                    // Fetch order draft from Economic
                    var draftResult = await _economicClient.GetOrderDraftIfExistsAsync(orderNumber, stoppingToken);
                    if (!draftResult.IsSuccess)
                    {
                        _logger.LogInformation("[Economic] Order {OrderNumber} not found or failed: {Error}", orderNumber, draftResult.ErrorMessage);
                        continue;
                    }

                    _logger.LogInformation("[Economic] Creating invoice draft for Tracelink order {OrderNumber}", orderNumber);

                    // Create invoice draft in Economic
                    var invoiceResult = await _economicClient.CreateInvoiceDraftAsync(draftResult.Data!, orderNumber, crmNumber!, stoppingToken);
                    if (!invoiceResult.IsSuccess)
                    {
                        _logger.LogInformation("[Economic] Failed to create invoice draft for order {OrderNumber}: {Error}", orderNumber, invoiceResult.ErrorMessage);
                        continue;
                    }

                    _logger.LogInformation("[Economic] Invoice draft created successfully for order {OrderNumber}.", orderNumber);

                    // Save invoice to database
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var persistence = scope.ServiceProvider.GetRequiredService<InvoicePersistenceService>();
                        await persistence.SaveInvoiceAsync(invoiceResult.Data!, orderNumber, crmNumber!, stoppingToken);
                        _logger.LogInformation("[Database] Invoice information saved successfully for order {OrderNumber} with CRM {crmNumber}", orderNumber, crmNumber);
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

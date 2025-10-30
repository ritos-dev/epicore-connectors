using RTS.Service.Connector.Interfaces;

namespace RTS.Service.Connector.Infrastructure
{
    public sealed class ConnectorBackgroundWorker : BackgroundService
    {
        private readonly IBackgroundTaskQueue _queue;
        private readonly IEconomicClient _economicClient;
        private readonly ITracelinkClient _tracelinkClient;
        private readonly ILogger<ConnectorBackgroundWorker> _logger;

        public ConnectorBackgroundWorker(
            IBackgroundTaskQueue queue,
            IEconomicClient economicClient,
            ITracelinkClient tracelinkClient,
            ILogger<ConnectorBackgroundWorker> logger)
        {
            _queue = queue;
            _economicClient = economicClient;
            _tracelinkClient = tracelinkClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[Connector] Background Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Get next order number
                    var orderNumber = await _queue.DequeueAsync(stoppingToken);
                    _logger.LogInformation("[Connector] Dequeued order {OrderNumber} fetching from Tracelink...", orderNumber);

                    // Fetch from Tracelink
                    var tracelinkResult = await _tracelinkClient.GetOrderAsync(orderNumber, stoppingToken);
                    if (!tracelinkResult.IsSuccess)
                    {
                        _logger.LogWarning("[Tracelink] Failed to fetch Tracelink order {OrderNumber}: {Error}", orderNumber, tracelinkResult.ErrorMessage);
                        continue;
                    }

                    _logger.LogInformation("[Tracelink] Fetched Tracelink order {OrderId} successfully", tracelinkResult.Data?.OrderId);

                    // Fetch order draft from Economic
                    var draftResult = await _economicClient.GetOrderDraftIfExistsAsync(orderNumber, stoppingToken);
                    if (!draftResult.IsSuccess)
                    {
                        _logger.LogWarning("[Economic] Order {OrderNumber} not found or failed: {Error}", orderNumber, draftResult.ErrorMessage);
                        continue;
                    }

                    _logger.LogInformation("[Economic] Order {OrderNumber} exists — creating invoice draft...", orderNumber);

                    // Create invoice draft in Economic
                    var invoiceResult = await _economicClient.CreateInvoiceDraftAsync(draftResult.Data!, orderNumber, stoppingToken);
                    if (!invoiceResult.IsSuccess)
                    {
                        _logger.LogWarning("[Economic] Failed to create invoice draft for order {OrderNumber}: {Error}", orderNumber, invoiceResult.ErrorMessage);
                        continue;
                    }

                    _logger.LogInformation("[Economic] Invoice draft created successfully for order {OrderNumber}.", orderNumber);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Connector] Error while processing Tracelink order from queue.");
                }
            }

            _logger.LogInformation("[Connector] Background Worker stopped.");
        }
    }
}

using RTS.Service.Connector.Infrastructure.Economic;
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
            IEconomicClient ecClient,
            ITracelinkClient tlClient,
            ILogger<ConnectorBackgroundWorker> logger)
        {
            _queue = queue;
            _economicClient = ecClient;
            _tracelinkClient = tlClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TraceLink Background Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Get next order number from queue
                    var orderNumber = await _queue.DequeueAsync(stoppingToken);
                    _logger.LogInformation("Dequeued order {OrderNumber} — fetching from TraceLink...", orderNumber);

                    var result = await _tracelinkClient.GetOrderAsync(orderNumber, stoppingToken);

                    if (!result.IsSuccess)
                    {
                        _logger.LogWarning("Failed to fetch order {OrderNumber}: {Error}", orderNumber, result.ErrorMessage);
                        continue;
                    }

                    _logger.LogInformation("Fetched TraceLink order {OrderId} successfully", result.Data?.OrderId);

                    // TODO: Save order to database or trigger next process here

                    // Check if order exists in Economic
                    var existsInEc = await _economicClient.OrderExistsAsync(orderNumber, stoppingToken);

                    if (existsInEc)
                    {
                        _logger.LogInformation("[Economic] Order {OrderNumber} exists — ready for invoice draft creation.", orderNumber);
                    }
                    else
                    {
                        _logger.LogInformation("[Economic] Order {OrderNumber} does not exist in Economic.", orderNumber);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing TraceLink order from queue.");
                }
            }

            _logger.LogInformation("TraceLink Background Worker stopped.");
        }
    }
}


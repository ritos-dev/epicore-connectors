using RTS.Service.Connector.Interfaces;

namespace RTS.Service.Connector.Infrastructure.Tracelink
{
    public sealed class TracelinkBackgroundWorker : BackgroundService
    {
        private readonly IBackgroundTaskQueue _queue;
        private readonly ITracelinkClient _client;
        private readonly ILogger<TracelinkBackgroundWorker> _logger;

        public TracelinkBackgroundWorker(
            IBackgroundTaskQueue queue,
            ITracelinkClient client,
            ILogger<TracelinkBackgroundWorker> logger)
        {
            _queue = queue;
            _client = client;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TraceLink Background Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var orderNumber = await _queue.DequeueAsync(stoppingToken);
                    _logger.LogInformation("Dequeued order {OrderNumber} — fetching from TraceLink...", orderNumber);

                    var result = await _client.GetOrderAsync(orderNumber, stoppingToken);

                    if (!result.IsSuccess)
                    {
                        _logger.LogWarning("Failed to fetch order {OrderNumber}: {Error}", orderNumber, result.ErrorMessage);
                        continue;
                    }

                    _logger.LogInformation("Fetched TraceLink order {OrderId} successfully", result.Data?.OrderId);

                    // TODO: Save order to database or trigger next process here
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


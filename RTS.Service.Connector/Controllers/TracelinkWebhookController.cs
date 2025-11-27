using Microsoft.AspNetCore.Mvc;

using RTS.Service.Connector.Interfaces;

namespace RTS.Service.Connector.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TracelinkWebhookController : ControllerBase
    {
        private readonly IBackgroundTaskQueue _queue;
        private readonly ILogger<TracelinkWebhookController> _logger;

        public TracelinkWebhookController(
            IBackgroundTaskQueue queue,
            ILogger<TracelinkWebhookController> logger)
        {
            _queue = queue;
            _logger = logger;
        }

        [HttpGet("webhook")]
        public IActionResult ReceiveOrder([FromQuery]  string orderNumber, CancellationToken token)
        {
            try
            {
                _logger.LogInformation("Received order number: {OrderNumber}", orderNumber);
                _queue.EnqueueAsync(orderNumber, token);
                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to enqueue order number: {OrderNumber}", orderNumber);
                return BadRequest();
            }
        }
    }
}


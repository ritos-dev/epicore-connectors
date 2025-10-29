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
        public IActionResult ReceiveOrder([FromQuery]  string orderNumber)
        {
            _logger.LogInformation("[TraceLink Webhook] Order is ready to be invoiced. OrderNumber: {OrderNumber}", orderNumber);

            _queue.Enqueue(orderNumber);

            return Accepted();
        }
    }
}


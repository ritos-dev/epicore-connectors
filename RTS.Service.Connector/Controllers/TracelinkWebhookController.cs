using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RTS.Service.Connector.API.Contracts;
using RTS.Service.Connector.API.Options;

namespace RTS.Service.Connector.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TracelinkWebhookController : ControllerBase
    {
        private readonly ILogger<TracelinkWebhookController> _logger;
        private readonly TracelinkOptions _options;

        public TracelinkWebhookController(
            ILogger<TracelinkWebhookController> logger,
            IOptions<TracelinkOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        [HttpGet("webhook")]
        public IActionResult ReceiveOrder([FromQuery] string orderNumber)
        {
            _logger.LogInformation("[TraceLink Webhook] Received order update — OrderNumber: {OrderNumber}", orderNumber);

            // fetch full order from TraceLink API here

            return Accepted();
        }
    }
}


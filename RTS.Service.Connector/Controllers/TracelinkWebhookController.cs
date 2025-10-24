using Microsoft.AspNetCore.Mvc;

namespace RTS.Service.Connector.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TracelinkWebhookController : ControllerBase
    {
        private readonly ILogger<TracelinkWebhookController> _logger;

        public TracelinkWebhookController( ILogger<TracelinkWebhookController> logger )
        {
            _logger = logger;
        }

        [HttpGet("webhook")]
        public IActionResult ReceiveOrder([FromQuery] string orderNumber)
        {
            _logger.LogInformation("[TraceLink Webhook] Received order update — OrderNumber: {OrderNumber}", orderNumber);


            return Accepted();
        }
    }
}


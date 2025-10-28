using Microsoft.AspNetCore.Mvc;
using RTS.Service.Connector.Interfaces;

namespace RTS.Service.Connector.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TracelinkOrderController : ControllerBase
    {
        private readonly ITracelinkClient _client;
        private readonly ILogger<TracelinkOrderController> _logger;

        public TracelinkOrderController(
            ITracelinkClient client,
            ILogger<TracelinkOrderController> logger)
        {
            _client = client;
            _logger = logger;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetOrder([FromQuery] string orderNumber, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
                return BadRequest("Order number cannot be empty.");

            _logger.LogInformation("[TraceLink] Searching for order with number {OrderNumber}", orderNumber);

            var result = await _client.GetOrderAsync(orderNumber, token);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("[TraceLink] Could not find order with number {OrderNumber}: {Error}",
                    orderNumber, result.ErrorMessage);
                return BadRequest(result.ErrorMessage);
            }

            var summary = new
            {
                result.Data?.OrderId,
                result.Data?.Number
            };

            _logger.LogInformation("[TraceLink] Found order → Number: {Number}, Id: {Id}",
                summary.Number, summary.OrderId);

            return Ok(summary);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetById(string orderId, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return BadRequest("Order ID cannot be empty.");

            _logger.LogInformation("[TraceLink] Fetching full details for order {OrderId}", orderId);

            var result = await _client.GetOrderByIdAsync(orderId, token);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("[TraceLink] Failed to fetch order {OrderId}: {Error}",
                    orderId, result.ErrorMessage);
                return BadRequest(result.ErrorMessage);
            }

            _logger.LogInformation("[TraceLink] Retrieved order {OrderId} successfully", orderId);
            return Ok(result.Data);
        }
    }
}

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
        [HttpGet("list")]
        public async Task<IActionResult> GetOrder([FromQuery] string orderNumber, CancellationToken token)
        {
            var result = await _client.GetOrderAsync(orderNumber, token);

            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            var summary = new
            {
                result.Data?.OrderId,
                result.Data?.Number
            };

            _logger.LogInformation("[TraceLink] Found order → Number: {Number}, Id: {Id}",
                summary.Number, summary.OrderId);

            return Ok(summary);
        }
    }
}

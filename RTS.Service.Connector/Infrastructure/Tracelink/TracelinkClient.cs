using Microsoft.Extensions.Options;

using RTS.Service.Connector.Application.Contracts;
using RTS.Service.Connector.Infrastructure.Tracelink;
using RTS.Service.Connector.Interfaces;
using RTS.Service.Connector.DTOs;

namespace RTS.Service.Connector.Infrastructure.Tracelink
{
    public sealed class TracelinkClient : ITracelinkClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<TracelinkClient> _logger;
        private readonly TracelinkOptions _options;

        public TracelinkClient(
            IHttpClientFactory factory,
            IOptions<TracelinkOptions> options,
            ILogger<TracelinkClient> logger)
        {
            _client = factory.CreateClient("TraceLink");
            _options = options.Value;
            _logger = logger;
        }
        /* ORDER LIST TO FIND ID */
        public async Task<TracelinkResults<OrderDto>> GetOrderAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"{_options.BaseUrl}/tracelink/order/list?token={_options.ApiToken}";
                _logger.LogInformation("Fetching TraceLink orders from {Url}", url);

                var response = await _client.PostAsync(url, null, cancellationToken);
                if (!response.IsSuccessStatusCode)
                    return await Fail(response, cancellationToken);

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var orders = TracelinkParser.ExtractOrders(json);

                var match = orders.FirstOrDefault(o =>
                    string.Equals(o.Number, orderNumber, StringComparison.OrdinalIgnoreCase));

                if (match is null)
                {
                    _logger.LogWarning("No TraceLink order found for {OrderNumber}", orderNumber);
                    return new TracelinkResults<OrderDto>(false, null, "Order not found");
                }

                _logger.LogInformation("Found order → Number: {Number}, Id: {Id}", match.Number, match.OrderId);
                return new TracelinkResults<OrderDto>(true,
                    new OrderDto { OrderId = match.OrderId, Number = match.Number});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching TraceLink order {OrderNumber}", orderNumber);
                return new TracelinkResults<OrderDto>(false, null, ex.Message);
            }
        }

        /* ORDER BY ID */
        public async Task<TracelinkResults<OrderDto>> GetOrderByIdAsync(string orderId, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"{_options.BaseUrl}/tracelink/order/{orderId}?token={_options.ApiToken}";
                _logger.LogInformation("Fetching TraceLink order by ID from {Url}", url);

                var response = await _client.GetAsync(url, cancellationToken);
                if (!response.IsSuccessStatusCode)
                    return await Fail(response, cancellationToken);

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var dto = TracelinkParser.ExtractSingleOrder(json);

                if (dto == null)
                {
                    _logger.LogWarning("Could not parse TraceLink order {OrderId}", orderId);
                    return new TracelinkResults<OrderDto>(false, null, "Invalid JSON structure");
                }

                _logger.LogInformation("Fetched TraceLink order {OrderId} successfully", orderId);
                return new TracelinkResults<OrderDto>(true, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching TraceLink order {OrderId}", orderId);
                return new TracelinkResults<OrderDto>(false, null, ex.Message);
            }
        }

        private async Task<TracelinkResults<OrderDto>> Fail(HttpResponseMessage response, CancellationToken token)
        {
            var msg = await response.Content.ReadAsStringAsync(token);
            _logger.LogWarning("TraceLink returned {StatusCode}: {Msg}", response.StatusCode, msg);
            return new TracelinkResults<OrderDto>(false, null, $"HTTP {response.StatusCode}");
        }
    }
}


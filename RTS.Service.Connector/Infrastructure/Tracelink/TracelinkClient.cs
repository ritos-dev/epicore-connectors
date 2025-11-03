using Microsoft.Extensions.Options;
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
            _client = factory.CreateClient("Tracelink");
            _options = options.Value;
            _logger = logger;
        }

        // Order list to find order id
        public async Task<ApiResult<TracelinkOrderDto>> GetOrderAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"{_options.BaseUrl}{_options.Endpoints.GetOrderList}?token={_options.ApiToken}";
                _logger.LogInformation("[Tracelink] Fetching TraceLink order {OrderNumber}", orderNumber);

                var response = await _client.PostAsync(url, null, cancellationToken);
                if (!response.IsSuccessStatusCode)
                    return await Fail<TracelinkOrderDto>(response, cancellationToken);

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var orders = TracelinkParser.ExtractOrders(json);

                var match = orders.FirstOrDefault(o =>
                    string.Equals(o.Number, orderNumber, StringComparison.OrdinalIgnoreCase));

                if (match is null)
                {
                    _logger.LogWarning("[Tracelink] No TraceLink order found for {OrderNumber}", orderNumber);
                    return new ApiResult<TracelinkOrderDto>(false, null, "Order not found");
                }

                _logger.LogInformation("[Tracelink] Found order. Number: {Number}, Id: {Id}", match.Number, match.OrderId);
                return new ApiResult<TracelinkOrderDto>(true,
                    new TracelinkOrderDto { OrderId = match.OrderId, Number = match.Number});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Tracelink] Error fetching TraceLink order {OrderNumber}", orderNumber);
                return new ApiResult<TracelinkOrderDto>(false, null, ex.Message);
            }
        }

        // Order by id to get details
        public async Task<ApiResult<TracelinkOrderDto>> GetOrderByIdAsync(string orderId, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"{_options.BaseUrl}{_options.Endpoints.GetOrder}{orderId}?token={_options.ApiToken}";
                _logger.LogInformation("Fetching TraceLink order by ID from {Url}", url);

                var response = await _client.GetAsync(url, cancellationToken);
                if (!response.IsSuccessStatusCode)
                    return await Fail<TracelinkOrderDto>(response, cancellationToken);

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                var dto = TracelinkParser.ExtractSingleOrder(json);

                if (dto == null)
                {
                    _logger.LogWarning("Could not parse TraceLink order {OrderId}", orderId);
                    return new ApiResult<TracelinkOrderDto>(false, null, "Invalid JSON structure");
                }

                _logger.LogInformation("Fetched TraceLink order {OrderId} successfully", orderId);
                return new ApiResult<TracelinkOrderDto>(true, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching TraceLink order {OrderId}", orderId);
                return new ApiResult<TracelinkOrderDto>(false, null, ex.Message);
            }
        }

        private async Task<ApiResult<T>> Fail<T>(HttpResponseMessage response, CancellationToken token)
        {
            var msg = await response.Content.ReadAsStringAsync(token);
            _logger.LogWarning("Request failed with {StatusCode}: {Message}", response.StatusCode, msg);
            return ApiResult<T>.Failure($"HTTP {response.StatusCode}: {msg}");
        }
    }
}


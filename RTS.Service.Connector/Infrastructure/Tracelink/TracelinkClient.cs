using Microsoft.Extensions.Options;

using RTS.Service.Connector.Application.Contracts;
using RTS.Service.Connector.DTOs;
using RTS.Service.Connector.Interfaces;
using System.Text.Json;

namespace RTS.Service.Connector.Infrastructure.Tracelink
{
    public sealed class TracelinkClient : ITracelinkClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<TracelinkClient> _logger;
        private readonly TracelinkOptions _options;

        public TracelinkClient(
            IHttpClientFactory factory,
            IOptions <TracelinkOptions> options,
            ILogger<TracelinkClient> logger)
        {
            _client = factory.CreateClient("TraceLink");
            _options = options.Value;
            _logger = logger;
        }

        public async Task<TracelinkResults<OrderDto>> GetOrderAsync(
        string orderId,
        CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"{_options.BaseUrl}/tracelink/order/list?token={_options.ApiToken}";

                _logger.LogInformation("Fetching TraceLink order from {Url}", url);

                var request = new HttpRequestMessage(HttpMethod.Post, url);
                var response = await _client.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogWarning("TraceLink returned {StatusCode}: {Message}", response.StatusCode, error);

                    return new TracelinkResults<OrderDto>(false, null, $"HTTP {response.StatusCode}");
                }

                // Deserialize JSON
                var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);

                JsonElement arrayElement;

                if (json.ValueKind == JsonValueKind.Array)
                {
                    arrayElement = json;
                }
                else if (json.ValueKind == JsonValueKind.Object)
                {
                    var foundArray = json.EnumerateObject()
                        .FirstOrDefault(p => p.Value.ValueKind == JsonValueKind.Array);

                    if (foundArray.Value.ValueKind != JsonValueKind.Array)
                    {
                        _logger.LogWarning("No array property found in JSON: {Json}", json.ToString());
                        return new TracelinkResults<OrderDto>(false, null, "Unexpected JSON structure");
                    }

                    arrayElement = foundArray.Value;
                }
                else
                {
                    _logger.LogWarning("Unexpected JSON type: {Kind}", json.ValueKind);
                    return new TracelinkResults<OrderDto>(false, null, "Unexpected JSON structure");
                }

                foreach (var order in arrayElement.EnumerateArray())
                {
                    string? id = TryGet(order, "order_id") ?? TryGet(order, "id");
                    string? number = TryGet(order, "number") ?? TryGet(order, "foreigndata_1") ?? TryGet(order, "name");

                    if (string.Equals(number, orderId, StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogInformation("Found order → Number: {Number}, Id: {Id}", number, id);
                        return new TracelinkResults<OrderDto>(true, new OrderDto { OrderId = id!, Number = number! });
                    }
                }

                _logger.LogWarning("No order found matching ID {OrderId}", orderId);
                return new TracelinkResults<OrderDto>(false, null, "Order not found");
            }
                catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching TraceLink order {OrderId}", orderId);
                return new TracelinkResults<OrderDto>(false, null, ex.Message);
            }
        }
        private static string? TryGet(JsonElement element, string name)
        {
            return element.TryGetProperty(name, out var prop) ? prop.ToString() : null;
        }
    }
}

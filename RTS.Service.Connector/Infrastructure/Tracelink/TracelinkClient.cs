using Microsoft.Extensions.Options;

using RTS.Service.Connector.Application.Contracts;
using RTS.Service.Connector.Application.DTOs;
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

        public async Task<TracelinkResults<ExternalOrderDto>> GetOrderAsync(
            string orderId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _client.GetAsync(
                    $"/api/orders/{orderId}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogWarning("TraceLink returned {StatusCode}: {Message}", response.StatusCode, error);

                    return new TracelinkResults<ExternalOrderDto>(
                        false, null, $"HTTP {response.StatusCode}");
                }

                // Deserialize JSON
                var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
                var order = json.GetProperty("order");

                var dto = new ExternalOrderDto
                {
                    OrderId = order.GetProperty("order_id").GetString()!,
                    Company = order.GetProperty("company").GetString()!,
                    Number = order.GetProperty("number").GetString()!,
                    Name = order.GetProperty("name").GetString()!,
                    Description = order.GetProperty("description").GetString(),
                    State = order.GetProperty("state").GetString()!,
                    StartDate = DateTime.Parse(order.GetProperty("start_date").GetString()!),
                    DeadlineDate = DateTime.Parse(order.GetProperty("deadline_date").GetString()!),
                    OrderSourceData = order.GetProperty("order_src_data").GetString(),
                    CustomerId = int.Parse(order.GetProperty("customer_id").GetString()!),
                    UpdatedAt = DateTime.Parse(order.GetProperty("update_date").GetString()!)
                };

                _logger.LogInformation("TraceLink order {OrderId} fetched successfully", dto.OrderId);

                return new TracelinkResults<ExternalOrderDto>(true, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching TraceLink order {OrderId}", orderId);
                return new TracelinkResults<ExternalOrderDto>(false, null, ex.Message);
            }
        }
    }
}

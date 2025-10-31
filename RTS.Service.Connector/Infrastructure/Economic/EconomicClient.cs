using RTS.Service.Connector.Interfaces;

using System.Text;
using System.Text.Json;

namespace RTS.Service.Connector.Infrastructure.Economic
{
    public sealed class EconomicClient : IEconomicClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<EconomicClient> _logger;

        public EconomicClient(IHttpClientFactory factory, ILogger<EconomicClient> logger)
        {
            _client = factory.CreateClient("Economic");
            _logger = logger;
        }

        public async Task<ApiResult<string>> GetOrderDraftIfExistsAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            // FOR TESTING ONLY!
            if (orderNumber == "24056")
            {
                _logger.LogInformation("Overriding TraceLink order nr.");
                orderNumber = "30097";
            }

            _logger.LogInformation("[Economic] Fetching order draft {OrderNumber}...", orderNumber);

            var response = await _client.GetAsync($"orders/drafts/{orderNumber}", cancellationToken);
            if (!response.IsSuccessStatusCode)
                return await Fail<string>(response, cancellationToken);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("[Economic] Order draft {OrderNumber} fetched successfully.", orderNumber);

            return ApiResult<string>.Success(json);
        }

        public async Task<ApiResult<string>> CreateInvoiceDraftAsync(string orderJson, string orderNumber, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[Economic] Building invoice draft payload from order data...");

            var draft = EconomicInvoiceMapper.MapToInvoiceDraft(orderJson, orderNumber);

            var jsonBody = JsonSerializer.Serialize(draft, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
            });

            _logger.LogInformation("[Economic] Sending invoice draft to Economic.");

            var response = await _client.PostAsync(
                "invoices/drafts",
                new StringContent(jsonBody, Encoding.UTF8, "application/json"),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
                return await Fail<string>(response, cancellationToken);

            var result = await response.Content.ReadAsStringAsync(cancellationToken);

            LogDraftInvoiceNumber(result);
            return ApiResult<string>.Success(result);
        }

        private void LogDraftInvoiceNumber(string jsonResponse)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonResponse);
                if (doc.RootElement.TryGetProperty("draftInvoiceNumber", out var draftNumber))
                {
                    _logger.LogInformation("[Economic] Draft invoice created successfully. Invoice number: {DraftNumber}", draftNumber.GetInt32());
                }
                else
                {
                    _logger.LogInformation("[Economic] Invoice number not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Economic] Could not parse invoice draft response.");
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

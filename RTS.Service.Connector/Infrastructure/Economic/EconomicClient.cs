using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RTS.Service.Connector.Application.Contracts;
using RTS.Service.Connector.Interfaces;
using System.Text;
using System.Text.Json;

namespace RTS.Service.Connector.Infrastructure.Economic
{
    public sealed class EconomicClient : IEconomicClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<EconomicClient> _logger;
        private readonly EconomicOptions _options;

        public EconomicClient(IHttpClientFactory factory, IOptions<EconomicOptions> options, ILogger<EconomicClient> logger)
        {
            _client = factory.CreateClient("Economic");
            _logger = logger;
            _options = options.Value;
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

        public async Task<ApiResult<string>> CreateInvoiceDraftAsync(string orderJson, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("[Economic] Building invoice draft payload from order data...");

            using var orderDoc = JsonDocument.Parse(orderJson);
            var root = orderDoc.RootElement;

            var currency = root.GetProperty("currency").GetString();
            var customerNumber = root.GetProperty("customer").GetProperty("customerNumber").GetInt32();
            var paymentTermsNumber = root.GetProperty("paymentTerms").GetProperty("paymentTermsNumber").GetInt32();
            var layoutNumber = root.GetProperty("layout").GetProperty("layoutNumber").GetInt32();

            var recipient = root.GetProperty("recipient");
            var recipientName = recipient.GetProperty("name").GetString();
            var vatZoneNumber = recipient.GetProperty("vatZone").GetProperty("vatZoneNumber").GetInt32();

            var invoice = new
            {
                date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                currency,
                customer = new { customerNumber },
                paymentTerms = new { paymentTermsNumber },
                layout = new { layoutNumber },
                recipient = new
                {
                    name = recipientName,
                    vatZone = new { vatZoneNumber }
                },
                references = new { other = "TraceLink" },
                lines = Array.Empty<object>()
            };

            var jsonBody = JsonSerializer.Serialize(invoice);
            _logger.LogInformation("[Economic] Sending invoice draft to Economic.");

            var response = await _client.PostAsync("invoices/drafts", new StringContent(jsonBody, Encoding.UTF8, "application/json"), cancellationToken);
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
                    _logger.LogInformation("[Economic] Draft invoice created successfully → Invoice number: {DraftNumber}", draftNumber.GetInt32());
                }
                else
                {
                    _logger.LogInformation("[Economic] Draft invoice created successfully (number not found in response).");
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

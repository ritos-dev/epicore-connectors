using RTS.Service.Connector.Interfaces;

using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

namespace RTS.Service.Connector.Infrastructure.Economic
{
    public sealed class EconomicClient : IEconomicClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<EconomicClient> _logger;
        private readonly EconomicOptions _options;

        public EconomicClient(
            IHttpClientFactory factory, 
            ILogger<EconomicClient> logger, 
            IOptions<EconomicOptions> options)
        {
            _client = factory.CreateClient("Economic");
            _logger = logger;
            _options = options.Value;
        }

        public async Task<ApiResult<string>> GetOrderDraftIfExistsAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            // FOR TESTING ONLY! Manual override
            if (orderNumber == "24056")
            {
                _logger.LogInformation("Overriding TraceLink order nr.");
                orderNumber = "1";
            }

            var url = $"{_options.BaseUrl}{_options.Endpoints.GetOrderDraft}{orderNumber}";

            _logger.LogInformation("[Economic] Fetching order draft {OrderNumber}...", orderNumber);

            var response = await _client.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return await Fail<string>(response, cancellationToken);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("[Economic] Order draft {OrderNumber} fetched successfully.", orderNumber);

            return ApiResult<string>.Success(json);
        }

        public async Task<ApiResult<string>> CreateInvoiceDraftAsync(string orderJson, string orderNumber, string crmNumber, CancellationToken cancellationToken = default)
        {
            var url = $"{_options.BaseUrl}{_options.Endpoints.CreateDraft}";
            var draft = EconomicInvoiceMapper.MapToInvoiceDraft(orderJson, orderNumber, crmNumber);
            var jsonBody = JsonConvert.SerializeObject(draft, Formatting.None);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            _logger.LogInformation("[Economic] Building invoice draft payload from order data...");

            _logger.LogInformation("[Economic] Sending invoice draft to Economic.");

            var response = await _client.PostAsync(url, content, cancellationToken);
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
                var root = JObject.Parse(jsonResponse);
                var draftNumber = root["draftInvoiceNumber"]?.Value<int>();

                if (draftNumber != null)
                {
                    _logger.LogInformation("[Economic] Draft invoice created successfully. Invoice number: {DraftNumber}", draftNumber);
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

using RTS.Service.Connector.Interfaces;
using RTS.Service.Connector.DTOs;

using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using RTS.Service.Connector.Infrastructure.BackgroundWorker;


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

        public async Task<ApiResult<EconomicInvoiceDraft>> GetOrderDraftIfExistsAsync(string orderNumber, CancellationToken cancellationToken = default)
        {
            // FOR TESTING ONLY! Manual override
            if (orderNumber != null)
            {
                _logger.LogInformation("Overriding TraceLink order nr.");
                orderNumber = "1";
            }

            var url = $"{_options.BaseUrl}{_options.Endpoints.GetOrderDraft}{orderNumber}";

            _logger.LogInformation("[Economic] Fetching order draft {OrderNumber}...", orderNumber);

            var response = await _client.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return await Fail<EconomicInvoiceDraft>(response, cancellationToken);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("[Economic] Order draft {OrderNumber} fetched successfully.", orderNumber);

            var dto = JsonConvert.DeserializeObject<EconomicInvoiceDraft>(json);

            if (dto == null)
            {
                _logger.LogWarning("[Economic] Failed to deserialize JSON into EconomicInvoiceDraft for order {OrderNumber}.", orderNumber);
                return ApiResult<EconomicInvoiceDraft>.Failure("API returned success, but JSON could not be deserialized into EconomicInvoiceDraft.");
            }

            return ApiResult<EconomicInvoiceDraft>.Success(dto);
        }

        public async Task<ApiResult<EconomicInvoiceDraft>> CreateInvoiceDraftAsync(EconomicInvoiceDraft draft, string orderNumber, string crmNumber, CancellationToken cancellationToken)
        {
            var url = $"{_options.BaseUrl}{_options.Endpoints.CreateDraft}";
            var jsonBody = JsonConvert.SerializeObject(draft, Formatting.None);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            _logger.LogInformation("[Economic] Building invoice draft payload from order data...");

            _logger.LogInformation("[Economic] Sending invoice draft to Economic.");

            var response = await _client.PostAsync(url, content, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return await Fail<EconomicInvoiceDraft>(response, cancellationToken);

            var result = await response.Content.ReadAsStringAsync(cancellationToken);

            // Invoice draft number from economic
            var dto = JsonConvert.DeserializeObject<EconomicInvoiceDraft>(result);
            
            if(dto?.DraftInvoiceNumber != null)
            {
                _logger.LogInformation("[Economic] Draft invoice created successfully. Invoice number: {DraftNumber}", dto.DraftInvoiceNumber);
            }
            else
            {
                _logger.LogInformation("[Economic] Invoice number not found.");
            }

            return ApiResult<EconomicInvoiceDraft>.Success(dto);
        }

        private async Task<ApiResult<T>> Fail<T>(HttpResponseMessage response, CancellationToken token)
        {
            var msg = await response.Content.ReadAsStringAsync(token);
            _logger.LogWarning("Request failed with {StatusCode}: {Message}", response.StatusCode, msg);
            return ApiResult<T>.Failure($"HTTP {response.StatusCode}: {msg}");
        }
    }
}

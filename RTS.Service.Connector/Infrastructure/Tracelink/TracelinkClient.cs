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

        // Get order list
        public async Task<ApiResult<TracelinkOrderListDto>> GetOrderListAsync(string orderNumber, CancellationToken token)
        {
            try
            {
                var url = $"{_options.BaseUrl}{_options.Endpoints.GetOrderList}?token={_options.ApiToken}";
                _logger.LogInformation("[Tracelink Client] Fetching TraceLink order {OrderNumber}", orderNumber);

                var response = await _client.PostAsync(url, null, token);
                if (!response.IsSuccessStatusCode)
                {
                    return await Fail<TracelinkOrderListDto>(response, token);
                }

                var json = await response.Content.ReadAsStringAsync(token);
                var orderList = TracelinkParser.ExtractOrderList(json);
                var match = orderList.FirstOrDefault(o => string.Equals(o.Number, orderNumber, StringComparison.OrdinalIgnoreCase));

                if (match == null)
                {
                    return ApiResult<TracelinkOrderListDto>.Failure("Order not found");
                }

                return new ApiResult<TracelinkOrderListDto>(true, match);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Tracelink Client] Error fetching TraceLink order {OrderNumber}", orderNumber);
                return ApiResult<TracelinkOrderListDto>.Failure(ex.Message);
            }
        }

        // Get order specific data
        public async Task<ApiResult<TracelinkOrderDto>> GetOrderByIdAsync(string orderId, CancellationToken token)
        {
            try
            {
                var url = $"{_options.BaseUrl}{_options.Endpoints.GetOrder}{orderId}?token={_options.ApiToken}";
                var response = await _client.PostAsync(url, null, token);

                if (!response.IsSuccessStatusCode)
                {
                    return await Fail<TracelinkOrderDto>(response, token);
                }

                var json = await response.Content.ReadAsStringAsync(token);
                var dto = TracelinkParser.ExtractSingleOrder(json);

                if (dto == null)
                {
                    _logger.LogWarning("[Tracelink Client] Could not parse TraceLink order {OrderId}", orderId);
                    return ApiResult<TracelinkOrderDto>.Failure("Invalid JSON structure");
                }

                _logger.LogInformation("[Tracelink Client] Fetched TraceLink order {OrderId} successfully", orderId);
                return new ApiResult<TracelinkOrderDto>(true, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Tracelink Client] Error fetching TraceLink order {OrderId}", orderId);
                return ApiResult<TracelinkOrderDto>.Failure(ex.Message);
            }
        }

        // Get customer list 
        public async Task<ApiResult<TracelinkCustomerDto>> GetCustomerListAsync(string customerName, CancellationToken token)
        { 
            try
            {
                var url = $"{_options.BaseUrl}{_options.Endpoints.GetCustomerList}?token={_options.ApiToken}";
                var response = await _client.PostAsync(url, null, token);

                if (!response.IsSuccessStatusCode)
                {
                    return await Fail<TracelinkCustomerDto>(response, token);
                }

                var json = await response.Content.ReadAsStringAsync(token);
                var customerList = TracelinkParser.ExtractCustomerList(json);
                var match = customerList.FirstOrDefault(cl => string.Equals(cl.Name, customerName, StringComparison.OrdinalIgnoreCase));

                if (match == null)
                {
                    return ApiResult<TracelinkCustomerDto>.Failure("Customer not found");
                }

                return new ApiResult<TracelinkCustomerDto>(true, match);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Tracelink Client] Error fetching TraceLink customer {Name}", customerName);
                return ApiResult<TracelinkCustomerDto>.Failure(ex.Message);
            }
        }

        // Get crm list
        public async Task<ApiResult<TracelinkCRMDto>> GetCrmListAsync(string customerName, CancellationToken token)
        {
            try
            {
                var url = $"{_options.BaseUrl}{_options.Endpoints.GetCrmList}?token={_options.ApiToken}";
                var response = await _client.PostAsync(url, null, token);

                if (!response.IsSuccessStatusCode)
                {
                    return await Fail<TracelinkCRMDto>(response, token);
                }

                var json = await response.Content.ReadAsStringAsync(token);
                var crmList = TracelinkParser.ExtractCRM(json);
                var match = crmList.FirstOrDefault(cl => string.Equals(cl.Name, customerName, StringComparison.OrdinalIgnoreCase));

                if (match == null)
                {
                    return ApiResult<TracelinkCRMDto>.Failure("Customer not found");
                }

                return new ApiResult<TracelinkCRMDto>(true, match);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Tracelink Client] Error fetching TraceLink customer {Name}", customerName);
                return ApiResult<TracelinkCRMDto>.Failure(ex.Message);
            }
        }

        public async Task<ApiResult<List<TracelinkItemsDto>>> GetItemsFromCrmAsync(string crmId, CancellationToken token)
        {
            try
            {
                var url = $"{_options.BaseUrl}{_options.Endpoints.GetItemsFromCrm}?token={_options.ApiToken}";
                var response = await _client.PostAsync(url, null, token);

                if (!response.IsSuccessStatusCode)
                {
                    return await Fail<List<TracelinkItemsDto>>(response, token);
                }

                var json = await response.Content.ReadAsStringAsync(token);
                var crmItems = TracelinkParser.ExtractItemsFromCrm(json);
                var match = crmItems.Where(ci => string.Equals(ci.CrmId, crmId, StringComparison.OrdinalIgnoreCase)).ToList();

                if (match == null)
                {
                    return ApiResult<List<TracelinkItemsDto>>.Failure("CRM id not found");
                }

                return new ApiResult<List<TracelinkItemsDto>>(true, match);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Tracelink Client] Error fetching crm id {CRM ID}", crmId);
                return ApiResult<List<TracelinkItemsDto>>.Failure(ex.Message);
            }
        }

        public async Task<ApiResult<TracelinkItemsListDto>> GetItemListAsync(string objectId, CancellationToken token)
        {
            try
            {
                var url = $"{_options.BaseUrl}{_options.Endpoints.GetItemsFromCrm}?token={_options.ApiToken}";
                var response = await _client.PostAsync(url, null, token);

                if (!response.IsSuccessStatusCode)
                {
                    return await Fail<TracelinkItemsListDto>(response, token);
                }

                var json = await response.Content.ReadAsStringAsync(token);
                var item = TracelinkParser.ExtractItems(json);
                var match = item.FirstOrDefault(i => string.Equals(i.ObjectId, objectId, StringComparison.OrdinalIgnoreCase));

                if (match == null)
                {
                    return ApiResult<TracelinkItemsListDto>.Failure("Object id not found");
                }

                return new ApiResult<TracelinkItemsListDto>(true, match);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Tracelink Client] Error fetching object id {OBJ ID}", objectId);
                return ApiResult<TracelinkItemsListDto>.Failure(ex.Message);
            }
        }

        // Helper method for logging fails
        private async Task<ApiResult<T>> Fail<T>(HttpResponseMessage response, CancellationToken token)
        {
            var msg = await response.Content.ReadAsStringAsync(token);
            _logger.LogWarning("Request failed with {StatusCode}: {Message}", response.StatusCode, msg);
            return ApiResult<T>.Failure($"HTTP {response.StatusCode}: {msg}");
        }
    }
}


using RTS.Service.Connector.DTOs;
using RTS.Service.Connector.Interfaces;
using RTS.Service.Connector.Infrastructure.Economic;
using RTS.Service.Connector.Infrastructure.InvoiceSplit;
using RTS.Service.Connector.Infrastructure.Services;
using RTS.Service.Connector.Infrastructure.Tracelink;

using System.Globalization;
using Microsoft.Extensions.Options;

namespace RTS.Service.Connector.Infrastructure.BackgroundWorker
{
    public sealed class ConnectorBackgroundWorker : BackgroundService
    {
        private readonly IBackgroundTaskQueue _queue;
        private readonly IEconomicClient _economicClient;
        private readonly ITracelinkClient _tracelinkClient;
        private readonly IOrderSplitToInvoices _split;
        private readonly ILogger<ConnectorBackgroundWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly EconomicInvoiceMapper _mapper;

        public ConnectorBackgroundWorker(
            IBackgroundTaskQueue queue,
            IEconomicClient economicClient,
            ITracelinkClient tracelinkClient,
            IOrderSplitToInvoices split,
            ILogger<ConnectorBackgroundWorker> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<EconomicOptions> options,
            EconomicInvoiceMapper mapper)
        {
            _queue = queue;
            _economicClient = economicClient;
            _tracelinkClient = tracelinkClient;
            _split = split;
            _logger = logger;
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[Connector] Background Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var data = new CombinedOrderData();

                try
                {
                    // dequeue and find order in a list
                    if (!await DequeueAndFindOrderListAsync(data, stoppingToken))
                    {
                        continue;
                    }

                    // fetch all core tracelink data
                    if (!await FetchCoreTracelinkDataAsync(data, stoppingToken)) 
                    { 
                        continue; 
                    }

                    // calculate total net price
                    if (!await CalculateTotalNetPriceAsync(data, stoppingToken))
                    {
                        continue;
                    }

                    // final tracelink dto
                    CreateCombinedOrderDto(data);

                    // save tracelink data to db
                    await PersistTracelinkDataAsync(data, stoppingToken);

                    // fetch economic order
                    if (!await CheckEconomicDraftAsync(data, stoppingToken))
                    {
                        continue;
                    }

                    // split an order into parts
                    var invoiceParts = SplitOrderIntoParts(data);
                    if (invoiceParts.Count == 0)
                    {
                        continue;
                    }

                    // create invoice drafts
                    foreach (var part in invoiceParts)
                    {
                        var createdDraft = await CreateInvoiceDraftAndHandleFailureAsync(data, part, stoppingToken);

                        if (createdDraft == null)
                        {
                            continue;
                        }

                        // save invoice to db
                        await PersistSingleInvoiceAsync(data.OrderNumber, data.CombinedDto!.CrmNumber!, createdDraft, stoppingToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Expected during graceful shutdown
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex, "[Connector] Error while processing Tracelink order from queue.");
                }
            }
            _logger.LogInformation("[Connector] Background Worker stopped.");
        }

        private async Task<bool> DequeueAndFindOrderListAsync(CombinedOrderData data, CancellationToken stoppingToken)
        {
            data.OrderNumber = await _queue.DequeueAsync(stoppingToken);
            _logger.LogInformation("[Connector] Dequeued order {OrderNumber} fetching from Tracelink...", data.OrderNumber);

            var listResult = await _tracelinkClient.GetOrderListAsync(data.OrderNumber, stoppingToken);

            if (!listResult.IsSuccess)
            {
                _logger.LogInformation("[Worker] Failed to find order in list");
                return false;
            }

            data.ListData = listResult.Data!;
            _logger.LogInformation("[Worker] Order {OrderId} for {CustomerName} found successfully", data.ListData.OrderId, data.ListData.Name);
            return true;
        }

        private async Task<bool> FetchCoreTracelinkDataAsync(CombinedOrderData data, CancellationToken stoppingToken)
        {
            var orderResult = await _tracelinkClient.GetOrderByIdAsync(data.ListData!.OrderId, stoppingToken);
            var customerResult = await _tracelinkClient.GetCustomerListAsync(data.ListData.Name, stoppingToken);
            var crmResult = await _tracelinkClient.GetCrmListAsync(data.ListData.Name, stoppingToken);

            if (!orderResult.IsSuccess || !customerResult.IsSuccess || !crmResult.IsSuccess || crmResult.Data == null)
            {
                _logger.LogInformation("[Worker] Failed to fetch essential Tracelink details. Notifying user.");
                return false;
            }

            if (string.IsNullOrEmpty(crmResult.Data.CrmNumber))
            {
                _logger.LogError("[Worker] CRM number is null for order {OrderNumber} (Tracelink CrmId: {CrmId}). Skipping invoice creation.", data.OrderNumber, crmResult.Data.CrmId);
                return false;
            }

            // Fetch items attached to CRM
            var crmItemsResult = await _tracelinkClient.GetItemsFromCrmAsync(crmResult.Data!.CrmId, stoppingToken);

            data.OrderData = orderResult.Data;
            data.CustomerData = customerResult.Data;
            data.CrmData = crmResult.Data;
            data.CrmItems = crmItemsResult.IsSuccess ? crmItemsResult.Data : new List<TracelinkItemsDto>();

            return true;
        }

        private async Task<bool> CalculateTotalNetPriceAsync(CombinedOrderData data, CancellationToken stoppingToken)
        {
            data.TotalNetPrice = 0m;
            var combinedItems = new List<TracelinkCombinedItemsDto>();

            if (data.CrmItems != null && data.CrmItems.Count > 0)
            {
                foreach (var crmItem in data.CrmItems)
                {
                    var itemDetailsResult = await _tracelinkClient.GetItemListAsync(crmItem.GenObjectId, stoppingToken);
                    decimal price;

                    if (!decimal.TryParse(itemDetailsResult.Data!.ItemPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out price))
                    {
                        _logger.LogWarning("[Worker] Failed to parse price '{Price}' for genobj_id {Id}", itemDetailsResult.Data!.ItemPrice, crmItem.GenObjectId);
                        continue; 
                    }

                    data.TotalNetPrice += price * (decimal)crmItem.ItemAmount;

                    combinedItems.Add(new TracelinkCombinedItemsDto
                    {
                        ObjectId = crmItem.GenObjectId,
                        ItemAmount = crmItem.ItemAmount,
                        Price = price
                    });
                }
            }
            else
            {
                _logger.LogWarning("[Worker] No CRM items found for order {OrderNumber}. Total is 0.", data.OrderNumber);
            }

            data.CombinedDto = new CompleteTracelinkDto 
            { 
                Items = combinedItems 
            };

            _logger.LogInformation("[Worker] Final calculated net price for order {OrderNumber} is {Price:}.", data.OrderNumber, data.TotalNetPrice);
            return true;
        }

        private void CreateCombinedOrderDto(CombinedOrderData data)
        {
            var finalDto = TracelinkOrderFactory.Create(
                data.ListData!,
                data.OrderData!,
                data.CustomerData!,
                data.CrmData!
            );

            finalDto.Items = data.CombinedDto!.Items;
            data.CombinedDto = finalDto;

            data.CustomerType = CustomerTypeClassifier.Classify(data.CombinedDto.CompanyType);
        }

        private async Task PersistTracelinkDataAsync(CombinedOrderData data, CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var persistence = scope.ServiceProvider.GetRequiredService<TracelinkPersistenceService>();
                await persistence.SaveOrderAsync(data.CombinedDto!);
                _logger.LogInformation("[Worker] Combined order saved successfully for order {OrderNumber}.", data.OrderNumber);
            }
        }

        private async Task<bool> CheckEconomicDraftAsync(CombinedOrderData data, CancellationToken stoppingToken)
        {
            var draftResult = await _economicClient.GetOrderDraftIfExistsAsync(data.OrderNumber, stoppingToken);

            if (!draftResult.IsSuccess || draftResult.Data == null)
            {
                _logger.LogInformation("[Worker] Order {OrderNumber} not found or failed to fetch in Economic. Skipping invoice creation: {Error}", data.OrderNumber, draftResult.ErrorMessage);
                return false;
            }

            ApiResult<string> draftresult = data.EconomicDraft = draftResult.Data;
            _logger.LogInformation("[Worker] Economic draft found for order {OrderNumber}.", data.OrderNumber);
            return true;
        }

        private List<InvoicePart> SplitOrderIntoParts(CombinedOrderData data)
        {
            var invoiceParts = _split.Split(data.TotalNetPrice, data.CustomerType);

            if (invoiceParts.Count == 0)
            {
                _logger.LogInformation("[Worker] Split resulted in 0 parts for order {OrderNumber}. Skipping invoice creation.", data.OrderNumber);
            }
            else
            {
                _logger.LogInformation("[Worker] Split order {OrderNumber} into {Count} invoice parts.", data.OrderNumber, invoiceParts.Count);
            }

            return invoiceParts;
        }

        private async Task<EconomicInvoiceDraft?> CreateInvoiceDraftAndHandleFailureAsync(
            CombinedOrderData data,
            InvoicePart part,
            CancellationToken stoppingToken)
        {
            var combinedDto = data.CombinedDto!;
            var crmNumber = combinedDto.CrmNumber!;

            var draft = _mapper.MapToInvoiceDraft(data.EconomicDraft!, combinedDto, part);
            var invoiceResult = await _economicClient.CreateInvoiceDraftAsync(draft, data.OrderNumber, crmNumber, stoppingToken);

            if (!invoiceResult.IsSuccess)
            {
                _logger.LogError("[Worker] Failed to create invoice for '{Description}' on order {OrderNumber}: {Error}", draft.Lines.First().Description, data.OrderNumber, invoiceResult.ErrorMessage);
                return null;
            }

            _logger.LogInformation("[Worker] Successfully created invoice for '{Description}' (Order {OrderNumber})", draft.Lines.First().Description, data.OrderNumber);
            return invoiceResult.Data;
        }

        private async Task PersistSingleInvoiceAsync(
            string orderNumber,
            string crmNumber,
            EconomicInvoiceDraft invoiceDraft,
            CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var persistence = scope.ServiceProvider.GetRequiredService<InvoicePersistenceService>();
                await persistence.SaveInvoiceAsync(invoiceDraft, orderNumber, crmNumber, stoppingToken);
                _logger.LogInformation("[Worker] Invoice information saved successfully for order {OrderNumber} with CRM {crmNumber}", orderNumber, crmNumber);
            }
        }
    }
}
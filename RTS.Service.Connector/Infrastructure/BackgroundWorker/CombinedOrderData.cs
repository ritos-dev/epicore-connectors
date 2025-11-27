using RTS.Service.Connector.Domain.Enums;
using RTS.Service.Connector.DTOs;

namespace RTS.Service.Connector.Infrastructure.BackgroundWorker
{
    // The purpose of this class is to manage the flow of data for the different methods in BackgroundWorker. 
    // It is build as an internal state container used to pass data and intermediate results, between different sequential steps of BackgroundWorker.
    internal class CombinedOrderData
    {
        public string OrderNumber { get; set; } = string.Empty;
        public TracelinkOrderListDto? ListData { get; set; }

        public TracelinkOrderDto? OrderData { get; set; }
        public TracelinkCustomerDto? CustomerData { get; set; }
        public TracelinkCRMDto? CrmData { get; set; }
        public List<TracelinkItemsDto>? CrmItems { get; set; }

        public CompleteTracelinkDto? CombinedDto { get; set; }
        public EconomicInvoiceDraftDto? EconomicDraft { get; set; }
        public CustomerType CustomerType { get; set; }
        public decimal TotalNetPrice { get; set; }
    }
}

using RTS.Service.Connector.DTOs;

namespace RTS.Service.Connector.Infrastructure.Tracelink
{
    public static class TracelinkOrderFactory
    {
        public static CompleteTracelinkDto Create(
            TracelinkOrderListDto orderList, 
            TracelinkOrderDto orderDetails, 
            TracelinkCustomerDto customer, 
            TracelinkCRMDto crm)
        {
            return new CompleteTracelinkDto
            {
                // order list
                OrderId = orderList.OrderId,
                OrderNumber = orderList.Number,
                CustomerName = orderList.Name,

                // order details
                Description = orderDetails.Description,
                State = orderDetails.State,
                StartDate = orderDetails.StartDate,
                DeadlineDate = orderDetails.DeadlineDate,
                UpdatedAt = orderDetails.UpdatedAt,

                // customer list
                CustomerId = customer.CustomerId,
                CustomerAddress = customer.CustomerAddress,
                CustomerCity = customer.CustomerCity,
                CustomerPostalCode = customer.CustomerPostalcode,
                CompanyType = customer.CompanyType,

                // crm list
                CrmNumber = crm.CrmNumber,
                CrmId = crm.CrmId,

                // crm items
                Items = new List<TracelinkCombinedItemsDto>()
            };
        }
    }
}
